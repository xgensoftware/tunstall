using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallDAL;
using TunstallBL.Models;
using TunstallBL.Helpers;

namespace TunstallBL.Services
{
    public class EventService : BaseService<EventService>, IDisposable
    {
        public static EventService Instance
        {
            get
            {
                return new EventService();
            }
        }

        EventService()
        {
            _db = new TunstallDatabaseContext();
        }


        #region Public Methods

        public bool ProcessEventQueue(int serviceId)
        {
            var isDemoMode = ConfigurationManager.AppSettings["IsDemoMode"].Parse<bool>();
            var logFile = ConfigurationManager.AppSettings["LogFile"];
            var logger = new LogHelper(logFile);
            bool processEvents = true;

            logger.LogMessage(LogMessageType.INFO, string.Format("****** Processing events for service id {0} ******", serviceId));

            var events = _db.Events.Where(e => e.IsProcessed == false && e.ServiceId == serviceId).AsParallel().ToList();

            var eventModels = events
                                .Select(e => new EventModel()
                                {
                                    Id = e.Id,
                                    AccountCode = e.AccountCode,
                                    CallerId = e.CallerId,
                                    EventCode = e.EventCode,
                                    Qualifier = e.Qualifier,
                                    Zone = e.Zone,
                                    LineId = e.LineId,
                                    UnitModel = e.UnitModel
                                })
                                .ToList();


            try
            {
                events.ForEach(e => e.IsProcessed = true);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }

            if(processEvents)
            {
                //call CallRaiser.exe for each transaction
                Parallel.ForEach(eventModels, e => {

                    using (var db = new TunstallDatabaseContext())
                    {
                        var eventMapping = db.EventCodeMappings.Where(m => m.ExternalEventCode == e.EventCode).FirstOrDefault();
                        if(eventMapping != null)
                        {
                            string cmd = string.Format("CallRaiser.exe u:{0};c:{1};p:{2};n:{3}", e.AccountCode, eventMapping.InternalEventCode, e.LineId, e.CallerId);
                            logger.LogMessage(LogMessageType.INFO, cmd);

                            if(!isDemoMode)
                                Process.Start(cmd);
                        }
                    }

                });
            }

            logger.LogMessage(LogMessageType.INFO, string.Format("****** Completed events for service id {0} ******", serviceId));
            return processEvents;
        }

        #endregion
    }
}

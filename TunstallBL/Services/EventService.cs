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

        public bool ProcessEventQueue()
        {
            var isDemoMode = ConfigurationManager.AppSettings["IsDemoMode"].Parse<bool>();
            var logFile = ConfigurationManager.AppSettings["LogFile"];
            var logger = new LogHelper(logFile);
            bool processEvents = true;

            logger.LogMessage(LogMessageType.INFO, "****** Processing events ******");

            var events = _db.Events.Where(e => e.IsProcessed == false).AsParallel().ToList();

            var eventModels = events
                                .Select(e => new EventModel()
                                {
                                    Id = e.Id,
                                    AccountCode = e.AccountCode,
                                    CallerId = e.CallerId,
                                    EventCode = e.EventZone,
                                    Qualifier = e.Qualifier,
                                    Zone = e.Zone,
                                    LineId = e.LineId,
                                    UnitModel = e.UnitModel,
                                    ServiceId = e.ServiceId
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
                processEvents = false;
            }

            if(processEvents)
            {
                //call CallRaiser.exe for each transaction
                foreach (var e in eventModels)
                {
                    int i = 0;
                    bool result = int.TryParse(e.AccountCode, out i);
                    if (result)
                    {
                        try
                        {
                            using (var db = new TunstallDatabaseContext())
                            {
                                var eventMapping = db.EventCodeMappings.Where(m => m.ExternalEventCode == e.EventCode).FirstOrDefault();
                                if (eventMapping != null)
                                {
                                    string cmd = string.Format("CallRaiser.exe u:{0};c:{1};p:{2};n:{3}", e.AccountCode, eventMapping.InternalEventCode, e.LineId, StripPhoneNumber(e.CallerId));
                                    logger.LogMessage(LogMessageType.INFO, cmd);

                                    if (!isDemoMode)
                                    {
                                        //C:\PNC4\runs
                                        var p = new Process();
                                        p.StartInfo.WorkingDirectory = @"C:\PNC4\runs\";
                                        p.StartInfo.FileName = @"C:\PNC4\runs\CallRaiser.exe";
                                        p.StartInfo.Arguments = string.Format("u:{0};c:{1};p:{2};n:{3}", e.AccountCode, eventMapping.InternalEventCode, e.LineId, StripPhoneNumber(e.CallerId));
                                        p.StartInfo.RedirectStandardOutput = true;
                                        p.StartInfo.CreateNoWindow = true;
                                        p.StartInfo.UseShellExecute = false;
                                        p.Start();
                                        p.WaitForExit();
                                        logger.LogMessage(LogMessageType.INFO, string.Format("Call raised to PNC for account {0}, phone: {1}", e.AccountCode, e.CallerId));
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogMessage(LogMessageType.INFO, string.Format("Failed to process event Id {0}, {1}. ERROR:{2} ", e.Id, e.ToString(), ex.Message));
                        }
                    }
                    else
                        logger.LogMessage(LogMessageType.ERROR, string.Format("{0} is not a valid account code", e.AccountCode));
                   
                }
            }

            logger.LogMessage(LogMessageType.INFO, "****** Completed events ******");
            return processEvents;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallDAL;
using TunstallBL.Models;
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

        public async Task<bool> ProcessEventQueue(int serviceId)
        {
            bool processEvents = true;
            var events = await _db.Events.Where(e => e.IsProcessed == false && e.ServiceId == serviceId)
                                        .ToListAsync();

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
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                processEvents = false;
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
                            Process.Start(cmd);
                        }
                    }

                });
            }


            return processEvents;
        }

        #endregion
    }
}

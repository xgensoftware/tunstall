using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using MytrexAPI.Models;
using TunstallDAL;
using Newtonsoft.Json;

namespace MytrexAPI.Controllers
{
    [RoutePrefix("api/mytrex")]
    public class MytrexController : ApiController
    {
        [Route("submitevent")]
        [HttpPost]
        public async Task<IHttpActionResult> SubmitEvent()
        {
            try
            {
                Random r = new Random();
                var db = new TunstallDatabaseContext();
                var modelData = await Request.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<EventModel>(modelData);

                var newEvent = new TunstallDAL.Entities.Event();
                newEvent.AccountCode = model.AccountCode;
                newEvent.CallerId = model.CallerId;
                newEvent.EventCode = model.EventCode;
                newEvent.Qualifier = model.Qualifier;
                newEvent.Zone = model.Zone;
                newEvent.LineId = model.LineId;
                newEvent.TestMode = model.TestMode;
                newEvent.UnitModel = model.UnitModel;
                newEvent.EventTimeStamp = DateTime.Now;
                db.Events.Add(newEvent);
                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}

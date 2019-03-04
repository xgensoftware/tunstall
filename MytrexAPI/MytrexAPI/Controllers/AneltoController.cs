using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using TunstallBL;
using TunstallBL.Models;
using TunstallDAL;
using Newtonsoft.Json;

namespace MytrexAPI.Controllers
{
    [RoutePrefix("api/anelto")]
    public class AneltoController : ApiController
    {
        [Route("submitevent")]
        [HttpPost]
        public async Task<IHttpActionResult> SubmitEvent()
        {
            try
            {
                var db = new TunstallDatabaseContext();
                var modelData = await Request.Content.ReadAsStringAsync();
                var model = modelData.FromXML<Alarm>();

                var newEvent = new TunstallDAL.Entities.Event();
                newEvent.AccountCode = model.AccountNumber;
                newEvent.CallerId = model.Ani;
                newEvent.EventCode = model.Zone;
                newEvent.Zone = model.Zone;
                newEvent.LineId = "C4";
                newEvent.VerificationURL = model.VerificationUrl;
                newEvent.Latitude = double.Parse(model.Location.Latitude);
                newEvent.Longitude = double.Parse(model.Location.Longitude);
                newEvent.EventTimeStamp = DateTime.Now;
                newEvent.ServiceId = (int)External_Service.ANELTO;
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

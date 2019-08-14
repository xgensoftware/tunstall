using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Twilio.AspNet.Mvc;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using TunstallBL;
using TunstallBL.Services;
using TunstallBL.Helpers;
using TunstallBL.Models;
using TunstallBL.API;
using TunstallBL.API.Model;
namespace MytrexAPI.Controllers
{
    [RoutePrefix("api/Activation")]
    public class ActivationController : TwilioController
    {
        #region Member Variables
        enum TESTMODE
        {
            ON,
            OFF
        }
        #endregion

        #region Private Methods 
        bool SendToAnelto(TESTMODE mode, string unitId)
        {
            var model = new AneltoSubscriberOverrideRequest();
            model.accounts = unitId;

            switch (mode)
            {
                case TESTMODE.ON:
                    model.number = AppConfigurationHelper.AneltoTestNumber;
                    break;

                case TESTMODE.OFF:
                    model.number = AppConfigurationHelper.AneltoProdNumber;
                    break;
            }

            try
            {
                AneltoAPI api = new AneltoAPI();
                var response = api.SubscriberCCOverride(model);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        bool SendToMytrex(TESTMODE mode, string unitId, string serialNum)
        {
            try
            {
                string testFileName = AppConfigurationHelper.MytrextTestEvents;
                if (mode == TESTMODE.OFF)
                {
                    testFileName = AppConfigurationHelper.MytrexProdEvents;
                }
                // read in the correct file based on mode
                string path = System.Web.HttpContext.Current.Request.MapPath(string.Format("~\\Templates\\{0}", testFileName));
                List<MytrexUnitEvent> eventList = JsonConvert.DeserializeObject<List<MytrexUnitEvent>>(System.IO.File.ReadAllText(path));

                //Change the serialnumber for the events
                eventList.ForEach(e => e.UnitSerNum = serialNum);

                var api = new TunstallBL.API.MytrexAPI();
                var response = api.UpdateEvents(eventList);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        void SetUnitTestMode(CellDeviceModel cellDevice)
        {
            if (cellDevice != null)
            {
                //change the unit to the opposite of currnet status
                var mode = TESTMODE.OFF;
                switch (cellDevice.TEST)
                {
                    case true:
                        mode = TESTMODE.OFF;
                        break;

                    case false:
                        mode = TESTMODE.ON;
                        var isExisting = HomeService.Instance.SearchExistingUnit(cellDevice);
                        if (isExisting)
                        {
                            return;
                        }
                        break;
                }



                bool isSuccess = false;
                switch (cellDevice.OTHER.ToLower())
                {
                    case "mytrex lte":
                        isSuccess = SendToMytrex(mode, cellDevice.UNIT_ID.ToString(), cellDevice.SERIALNO);
                        break;

                    case "anelto lte":
                    case "anelto otg":
                        isSuccess = SendToAnelto(mode, cellDevice.UNIT_ID.ToString());
                        break;

                    default:

                        break;
                }

                if (isSuccess)
                {
                    CellDeviceService.Instance.UpdateCellDeviceStatus(cellDevice.ID, mode == TESTMODE.ON ? true : false);
                }
            }
        }

        #endregion

        [Route("setstatus/{digits}")]
        [HttpPost]
        public IHttpActionResult SetUnitOnTest(string digits)
        {
            var response = new VoiceResponse();

            if (!string.IsNullOrEmpty(digits))
            {
                //search the phone number
                CellDeviceSearchModel model = new CellDeviceSearchModel();
                model.PhoneNumber = digits;
                var result = CellDeviceService.Instance.SearchCellDevice(model);
                if (result.Count > 0)
                {
                    var unit = result[0];
                    SetUnitTestMode(unit);
                }
                else
                {
                    response.Append(new Say(string.Format("No device found for {0}",digits)));
                }
                //string newString = "";
                //char[] cArray = digits.ToCharArray();
                //foreach (char c in cArray)
                //{
                //    newString = newString + c + " ";
                //}
                //response.Say("You inputted: " + newString);
            }
            else
            {
                response.Append(new Gather(numDigits: 10).Say("Enter phone number"));
            }

            return (IHttpActionResult)TwiML(response);
        }
    }
}

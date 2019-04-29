using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TunstallBL;
using TunstallBL.Models;
using TunstallBL.Helpers;
using TunstallBL.Services;
using TunstallBL.API;
using TunstallBL.API.Model;

namespace AneltoIntegrationBrowser
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //args = new string[] { "2158033100" };
            if (args.Count() > 0)
            {
                var log = new LogHelper(AppConfigurationHelper.LogFile);
                string phoneNumber = args[0];
                

                //Search using phone with no 1 if no result search with the 1
                log.LogMessage(LogMessageType.INFO, string.Format("Searching for Phone {0}", phoneNumber));
                var home = HomeService.Instance.GetHomeByWorkPhone(phoneNumber);
                if (home == null)
                {
                    log.LogMessage(LogMessageType.INFO, string.Format("No home info found for {0}.", phoneNumber));

                    try
                    {
                        home = HomeService.Instance.GetHomeByWorkPhone(phoneNumber);
                    }
                    catch(Exception ex)
                    {
                        log.LogMessage(LogMessageType.INFO, string.Format("Failed to find home for {0}. ERROR: {1}", phoneNumber,ex.Message));
                    }
                }

                if (home == null)
                {
                    MessageBox.Show(string.Format("No information found for {0}", phoneNumber));
                    log.LogMessage(LogMessageType.INFO, string.Format("No home info found for {0}.", phoneNumber));
                }
                else
                {
                    log.LogMessage(LogMessageType.INFO, string.Format("Home found for {0}.", phoneNumber));
                    var model = new AneltoSubscriberUpdateRequest();
                    try
                    {
                        model.ani = AppConfigurationHelper.StripPhoneNumberField ? home.OTHER_PHONE.Remove(0, 1) : home.OTHER_PHONE;
                        var resident = home.Residents.OrderBy(r => r.PRIORITY).First();
                        model.firstname = resident.FIRST_NAME;
                        model.lastname = resident.LAST_NAME;
                        model.address = home.ADDRESS_STREET;
                        model.address1 = home.ADDRESS_AREA;
                        model.city = home.ADDRESS_TOWN;
                        model.state = home.ADDRESS_COUNTY;
                        model.zip = home.ADDRESS_POSTCODE;

                        AneltoAPI api = new AneltoAPI();
                        var response = api.SubscriberCreateUpdate(model);
                        log.LogMessage(LogMessageType.INFO, string.Format("Sent to Anelto with resposne {0} for phone number {1}. Data: {2}", response,phoneNumber, model.ToJson()));
                    }
                    catch (Exception ex)
                    {
                        log.LogMessage(LogMessageType.ERROR, string.Format("Failed to send to Anelto API for phone {0}. DATA: {1}. ERROR: {2}", phoneNumber,model.ToJson(), ex.Message));
                    }

                    // get the latest signal from the event table for the phone number
                    string url = string.Empty;

                    try
                    {
                        var strip = AppConfigurationHelper.StripPhoneNumberField ? phoneNumber.Remove(0, 1) : phoneNumber;
                        url = EventService.Instance.GetUrlBy(strip);
                    }
                    catch(Exception ex)
                    {
                        log.LogMessage(LogMessageType.ERROR, string.Format("Failed to get URL from Event database. ERROR: {0}", ex.Message));
                    }

                    if (!string.IsNullOrEmpty(url))
                    {
                        log.LogMessage(LogMessageType.INFO, string.Format("Found url from event table for {0}",url));
                        try
                        {
                            Process.Start("chrome.exe", url);
                            log.LogMessage(LogMessageType.INFO, string.Format("Opening chrome browser for url {0}", url));
                        }
                        catch
                        {
                            using (WebBrowser browser = new WebBrowser())
                            {
                                browser.Navigate(url, "_blank", null, null);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Phone number required.");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TunstallBL;
using TunstallBL.Helpers;
namespace MytrexIntegrationBrowser
{

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var secret = ConfigurationManager.AppSettings["Secret"].ToString();
            var dealerKey = ConfigurationManager.AppSettings["DealerKey"].ToString();
            var username = ConfigurationManager.AppSettings["Username"].ToString();
            var url = ConfigurationManager.AppSettings["MytrexUrl"].ToString();
            var log = new LogHelper(AppConfigurationHelper.LogFile);

            //args = new string[] { "7472759063" };
            if(args != null)
            {
                if(args.Count() > 0)
                {
                    string phoneNumber = args[0];
                    if(phoneNumber.Substring(0,1) != "1")
                    {
                        phoneNumber = string.Format("1{0}", phoneNumber);
                    }
                    var token = JWTHelper.GetToken(secret, username);
                    url = string.Format("{0}?username={1}&phonenumber={2}&dealerkey={3}&token={4}", url, username, phoneNumber, dealerKey,token);

                    try
                    {
                        Process.Start("chrome.exe", url);
                        log.LogMessage(LogMessageType.INFO, $"Opening URL: {url}");
                    }
                    catch(Exception e) {

                        log.LogMessage(LogMessageType.ERROR, $"Failed to open URL {url}. ERROR: {e.Message}");
                        using (WebBrowser browser = new WebBrowser())
                        {
                            browser.Navigate(url, "_blank", null, null);
                        }
                    }
                }
            }
        }
    }
}

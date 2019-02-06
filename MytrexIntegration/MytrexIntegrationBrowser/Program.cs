﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TunstallBL;
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
            
            if(args != null)
            {
                if(args.Count() > 0)
                {
                    string phoneNumber = args[0];
                    var token = JWTHelper.GetToken(secret, username);
                    url = string.Format("{0}?username={1}&phonenumber={2}&dealerkey={3}&token={4}", url, username, phoneNumber, dealerKey,token);

                    //string headers = string.Format("Authorization: 0 {0} \r\n", token); 
                    using (WebBrowser browser = new WebBrowser())
                    {
                        browser.Navigate(url, "_blank", null,null);
                    }
                }
            }
        }
    }
}

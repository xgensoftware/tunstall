using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;

namespace PNC_Kshema
{
    public static class AppConfiguration
    {
        public static string PNC_Connection
        {
            get
            {
                return ConfigurationManager.AppSettings["PNC"].ToString();
            }
        }

        public static string Kshema_Connection
        {
            get
            {
                return ConfigurationManager.AppSettings["Kshema"].ToString();
            }
        }

        public static string PNCStaging_Connection
        {
            get
            {
                return ConfigurationManager.AppSettings["PNC_Staging"].ToString();
            }
        }

        public static double ServiceTimerInterval
        {
            get
            {
                double timer = 0;
                try
                {
                    timer = TimeSpan.FromMinutes(ConfigurationManager.AppSettings["ServiceTimer"].Parse<double>()).TotalMilliseconds;
                }
                catch { timer = 1200000; }

                return timer;
            }
        }

        public static string Query_Files_Path
        {
            get { return ConfigurationManager.AppSettings["Query_Files_Path"].ToString(); }
        }

        public static string Email_Server
        {
            get { return ConfigurationManager.AppSettings["Email_Server"].ToString(); }
        }

        public static string Email_User
        {
            get { return ConfigurationManager.AppSettings["Email_User"].ToString(); }
        }

        public static string Email_Password
        {
            get { return ConfigurationManager.AppSettings["Email_Password"].ToString(); }
        }

        public static string Email_ToAddress
        {
            get { return ConfigurationManager.AppSettings["Email_ToAddress"].ToString(); }
        }
    
    }
}

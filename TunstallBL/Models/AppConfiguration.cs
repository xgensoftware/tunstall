using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.Models
{
    public class AppConfiguration
    {
        public static string MytrexSecret
        {
            get { return ConfigurationManager.AppSettings["MytrexSecret"]; }
        }

        public static string MytrexDealerKey
        {
            get { return ConfigurationManager.AppSettings["MytrexDealerKey"]; }
        }

        public static string AneltoAPIUsername
        {
            get { return ConfigurationManager.AppSettings["AneltoAPIUsername"]; }
        }

        public static bool StripPhoneNumberField
        {
            get { return ConfigurationManager.AppSettings["StripPhoneNumberField"].Parse<bool>(); }
        }

        public static string PNCConnection
        {
            get { return ConfigurationManager.AppSettings["PNC"].ToString(); }
        }

        public static string LogFile
        {
            get { return ConfigurationManager.AppSettings["LogFile"]; }
        }
       
        public static string AneltoAPIPassword
        {
            get { return ConfigurationManager.AppSettings["AneltoAPIPassword"]; }
        }
    }
}

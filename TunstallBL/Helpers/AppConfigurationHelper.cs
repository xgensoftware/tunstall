using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.Helpers
{
    public static class AppConfigurationHelper
    { 
        public static string CallRaiserCommand
        {
            get { return ConfigurationManager.AppSettings["CallRaiserCommand"]; }
        }


        public static string MytrexSecret
        {
            get { return ConfigurationManager.AppSettings["MytrexSecret"]; }
        }

        public static string MytrexDealerKey
        {
            get { return ConfigurationManager.AppSettings["MytrexDealerKey"]; }
        }

        public static string MytrexUsername
        {
            get { return ConfigurationManager.AppSettings["MytrexUsername"]; }
        }

        public static string MytrexUrl
        {
            get { return ConfigurationManager.AppSettings["MytrexUrl"]; }
        }

        public static string MytrexProdEvents
        {
            get { return ConfigurationManager.AppSettings["MytrexProdEvents"]; }
        }

        public static string MytrextTestEvents
        {
            get { return ConfigurationManager.AppSettings["MytrextTestEvents"]; }
        }

        public static string AneltoAPIUsername
        {
            get { return ConfigurationManager.AppSettings["AneltoAPIUsername"]; }
        }
        
        public static string AneltoAPIPassword
        {
            get { return ConfigurationManager.AppSettings["AneltoAPIPassword"]; }
        }

        public static string AneltoURL
        {
            get { return ConfigurationManager.AppSettings["AneltoURL"]; }
        }

        public static string AneltoTestNumber
        {
            get { return ConfigurationManager.AppSettings["AneltoTestNumber"]; }
        }

        public static string AneltoProdNumber
        {
            get { return ConfigurationManager.AppSettings["AneltoProdNumber"]; }
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

    }
}

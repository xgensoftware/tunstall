using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;

namespace Agency_Import
{
    public static class AppConfiguration
    {
        public static string PNCStaging_Connection
        {
            get
            {
                return ConfigurationManager.AppSettings["PNC_Staging"].ToString();
            }
        }
    }
}

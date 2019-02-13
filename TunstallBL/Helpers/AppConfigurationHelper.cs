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

    }
}

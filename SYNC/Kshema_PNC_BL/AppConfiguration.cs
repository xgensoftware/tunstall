using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
namespace Kshema_PNC.BL
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

        public static string PNCStaging_Connection
        {
            get
            {
                return ConfigurationManager.AppSettings["PNC_Staging"].ToString();
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

using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace MytrexAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var migrator = new DbMigrator(new TunstallDAL.Migrations.Configuration());
            migrator.Update();
        }
    }
}

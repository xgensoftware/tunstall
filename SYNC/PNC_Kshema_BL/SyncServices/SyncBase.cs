using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.SyncServices
{
    public class SyncBase
    {
        protected Timer _ActiveTimer = null;
        protected IDataProvider _pncProvider = null;
        protected IDataProvider _stageProvider = null;
        protected IDataProvider _pl_provider = null;

        protected void InitializeProvider()
        {
            _pncProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfiguration.PNC_Connection);
            _stageProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
            _pl_provider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.Kshema_Connection);
        }

        protected void LogServiceMessage(string serviceName,string log_type, string message)
        {
            try
            {
                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@service_name", serviceName));
                parms.Add(SQLParam.GetParam("@log_type", log_type));
                parms.Add(SQLParam.GetParam("@message", message));
                _stageProvider.ExecuteNonQuery("SYNC_Insert_Sync_Log", parms);
            }
            catch { }
        }

        protected void Update_Sync_Step(string step_name)
        {
            try
            {
                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@step_name", step_name));
                _stageProvider.ExecuteNonQuery("SYNC_Update_Sync_Step", parms);
            }
            catch { }
        }


        //protected void Reset_Sync_Steps()
        //{
        //    try
        //    {
        //        _stageProvider.ExecuteNonQuery("SYNC_Reset_Sync_Steps", null);

               
        //    }
        //    catch { }
        //}

        protected void DisposeProvider()
        {
            if (_pncProvider != null)
                _pncProvider = null;

            if (_stageProvider != null)
                _stageProvider = null;
        }
    }
}

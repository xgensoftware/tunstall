using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace Agency_Import
{
    public class Agency_Sync
    {
        string _group_record;
        IDataProvider _stageProvider = null;

        protected void LogServiceMessage(string log_type, string message)
        {
            try
            {
                Console.WriteLine(string.Format("{0}    {1}", DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"), message));

                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@service_name", "Agency_Import"));
                parms.Add(SQLParam.GetParam("@log_type", log_type));
                parms.Add(SQLParam.GetParam("@message", message));
                _stageProvider.ExecuteNonQuery("SYNC_Insert_Sync_Log", parms);
            }
            catch { }
        }
        private bool Run_Agency_Import()
        {
            bool canRun = false;

            try
            {
                canRun = this._stageProvider.ExecuteScalar<bool>("SYNC_Can_Run_Agency_Import", null);
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Failed to check for agency imports. {0}",ex.Message));
            }

            return canRun;
        }

        public Agency_Sync(string group_record)
        {
            _stageProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
            _group_record = group_record;
        }
        public void Run_Sync()
        {
            this.LogServiceMessage("INFO", "Starting Agency Import");

            try
            {
                if(Run_Agency_Import())
                    this._stageProvider.ExecuteNonQuery("SYNC_Agency_Import", null);
            }
            catch (Exception ex)
            {
                this.LogServiceMessage("ERROR", string.Format("Agency Import had a fatal exception. ERROR: {0}",ex.Message));
            }

            this.LogServiceMessage("INFO", "Agency Import Complete");
        }
    }
}

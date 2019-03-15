using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;
namespace Kshema_PNC.BL.Repository
{
    class ServiceRepository : BaseRepository
    {
        #region " Constructor "
        public ServiceRepository()
        {
            this._stageProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
            this._pncProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfiguration.PNC_Connection);
        }
        #endregion

        #region " Public Methods "
        
        public void Reset_Sync_Steps()
        {
            try
            {
                _stageProvider.ExecuteNonQuery("SYNC_Reset_Sync_Steps", null);
            }
            catch { }
        }

        public void Update_Sync_Step(string step_name)
        {
            try
            {
                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@step_name", step_name));
                _stageProvider.ExecuteNonQuery("SYNC_Update_Sync_Step", parms);
            }
            catch { }
        }

        public void Move_Change_Log_Transactions(string group_record)
        {
            List<SQLParam> parms = new List<SQLParam>();
            parms.Add(SQLParam.GetParam("@group_record", group_record));
            _stageProvider.ExecuteNonQuery("SYNC_Move_EtlChangeLog_Stage", parms);
        }

        public void Insert_Temp_Map_Equip_Id()
        {
            _stageProvider.ExecuteNonQuery("SYNC_Insert_Temp_Map_Equip_Id", null);
        }

        public DataSet Run_Query_File(string sql)
        {
            return _stageProvider.ExecuteDataSetQuery(sql, null);
        }

        public DataSet Run_Query_SP(string spName)
        {
            return _stageProvider.ExecuteDataSet(spName, null);
        }

        public void Update_Log_Row(int id)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat("Update [ks_ETLChangeLog] set Is_Processed = 1, Processed_time = getdate() where Id = {0}", id.ToString());
                this._stageProvider.ExecuteNonSPQuery(sql.ToString(), null);
            }
            catch { }
        }

        public void Save_Transaction_Record(string group_record,string entity,string transaction, string err_message)
        {
            try
            {
                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@group_record", group_record));
                parms.Add(SQLParam.GetParam("@entity", entity));
                parms.Add(SQLParam.GetParam("@transaction", transaction));
                parms.Add(SQLParam.GetParam("@err_message", err_message));

                _stageProvider.ExecuteNonQuery("SYNC_Insert_Transaction_Log", parms);

            }
            catch { }
        }
        #endregion
    }
}

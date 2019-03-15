using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;
namespace Kshema_PNC.BL.Repository
{
    class BaseRepository : IDisposable
    {
        protected IDataProvider _stageProvider = null;
        protected IDataProvider _pncProvider = null;

        public void Log_Service_Message(string log_type, string message)
        {
            try
            {
                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@service_name", "Kshema_PNC_Sync"));
                parms.Add(SQLParam.GetParam("@log_type", log_type));
                parms.Add(SQLParam.GetParam("@message", message));
                _stageProvider.ExecuteNonQuery("SYNC_Insert_Sync_Log", parms);
            }
            catch { }
        }

        public virtual void Dispose()
        {
            if (_stageProvider != null)
                _stageProvider = null;
        }
    }
}

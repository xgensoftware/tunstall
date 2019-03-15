using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;
using Kshema_PNC.BL.Repository;

namespace Kshema_PNC.BL.Entity
{
    class EntityBase
    {
        #region " Event "
        public event EntityMessage_Handler OnEntityMessage;
        #endregion

        #region " Member Variables "
        protected string _group_record = "-1";

        protected StringBuilder _sql = null;

        protected System.Data.DataRow _dataToProcess;

        protected List<SQLParam> _sqlParms = null;
        protected IDataProvider _stageProvider = null;
        protected IDataProvider _pncProvider = null;
        protected ServiceRepository _serviceRepository = null;

        #endregion

        #region " Methods "

        protected bool Check_Agency_Mapping(string agency_id)
        {
            bool ismapped = false;

            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@agency_id", agency_id));
                ismapped = _stageProvider.ExecuteDataSet("SYNC_Is_Mapped_Agency ", this._sqlParms).Tables[0].Rows[0]["Is_Mapped"].Parse<bool>();
                
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Fatal exception in Check_Agency_Mapping for agency {0}. Error: {1}", agency_id, ex.Message), "ERROR");
            }

            return ismapped;
        }

        protected void InitializeProviders()
        {
            _stageProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
            _pncProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfiguration.PNC_Connection);
            _serviceRepository = new ServiceRepository();
        }

        protected void EntityMessage(string message,string log_type)
        {
            if (OnEntityMessage != null)
            {
                OnEntityMessage(message, log_type);

                if (log_type.ToLower() == "error")
                    Send_Error_Mail(message);
            }
        }

        protected bool IsSubscriberMapped(string subscriberId)
        {
            bool isMapped = false;

            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", subscriberId));
                DataTable dtMapping = this._stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Mapping", this._sqlParms).Tables[0];

                if (dtMapping.Rows.Count >= 1)
                    isMapped = true;
            }
            catch { }


            return isMapped;
        }

        private void Send_Error_Mail(string email_message)
        {
            try
            {
                SendMailHelper mail = new SendMailHelper(AppConfiguration.Email_Server, AppConfiguration.Email_User, AppConfiguration.Email_Password);
                mail.SendEmail("Server_VMTCMSSQL1@tunstallamac.com"
                    , AppConfiguration.Email_ToAddress
                    , "Tunstall Kshema to PNC Sync Error"
                    , email_message);
            }
            catch { }
        }
        #endregion
    }
}

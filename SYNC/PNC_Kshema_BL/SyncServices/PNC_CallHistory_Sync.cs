using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using com.Xgensoftware.Core;
using PNC_Kshema.BL.Entity;

namespace PNC_Kshema.BL.SyncServices
{
    public class PNC_CallHistory_Sync : SyncBase
    {
        #region " Member Variabels "
        string _group_record;
        #endregion

        #region " Constructor "
        public PNC_CallHistory_Sync(string group_record)
        {
            this._group_record = group_record;
        }
        #endregion

        #region " Private Methods "

        void LogMessage(string message, string log_type)
        {
            string msg = string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
            Console.WriteLine(msg);

            this.LogServiceMessage("PNC_CallHistory_Sync", log_type, message);
        }

        void ch_OnEntityMessage(string message, string message_type)
        {
            LogMessage(message, message_type);
        }
        
        #endregion

        #region " Public Methods "
        public void RunSync()
        {
            string sql = string.Empty;
            DataTable dtCallsHistory = null;
            
            this.InitializeProvider();
            
            try
            {
                sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, "Get_PNC_Call_History.sql" ));
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("ProcessEntityFlow failed to retrieve query file Get_PNC_Call_History.sql. ERROR: {0}",ex.Message), "ERROR");
            }

            if (!string.IsNullOrEmpty(sql))
            {
                try
                {
                    #region OLD
                    //StringBuilder sql = new StringBuilder();
                    //sql.Append("select ");
                    //sql.Append("ch.call_def ");
                    //sql.Append(",ch.arrival_time ");
                    //sql.Append(",case ch.call_entity_type");
                    //sql.Append(" when 2 then 'Home'");
                    //sql.Append(" else 'Unknown'");
                    //sql.Append(" end as [Type] ");
                    //sql.Append(",el.location_def ");
                    //sql.Append(",el.Equip_Id ");
                    //sql.Append(",el.equip_phone ");
                    //sql.Append(",ch.call_code ");
                    //sql.Append(",case ch.call_info");
                    //sql.Append(" when 4002 then (select resident_def from Resident where location_ref = el.location_def  and primary_YN = 'Y')");
                    //sql.Append(" else (select resident_def from Resident where location_ref = el.location_def and primary_YN = 'Y')");
                    //sql.Append(" end as [Resident_def] ");
                    //sql.Append(",ch.meaning ");
                    //sql.Append(",ch.protocol_tag ");
                    //sql.Append(",r.reason_def ");
                    //sql.Append(",r.text as [reason] ");
                    //sql.Append("from CALLS_HISTORY ch ");
                    //sql.Append("join EPEC_LOCATION el on ch.call_entity_ref = el.location_def ");
                    //sql.Append("join REASONS r on ch.reason_ref = r.reason_def ");
                    //sql.Append("where ch.call_entity_type = 2 ");
                    //sql.Append("and ch.Incoming_YN = 'Y' ");
                    //sql.Append("and ch.call_code not in (' P') ");
                    //sql.AppendFormat("and cast(arrival_time as date) between cast('{0}' as date) and cast('{1}' as date) ", from_date.ToString("yyyy-MM-dd"), to_date.ToString("yyyy-MM-dd"));
                    //sql.Append("order by arrival_time asc");                
                    #endregion

                    dtCallsHistory = _pncProvider.GetData(sql, null);
                }
                catch (Exception ex)
                {
                    LogMessage(string.Format("RunSync failed to get calls history. ERROR: {0}",  ex.Message), "ERROR");
                }

                if (dtCallsHistory != null || dtCallsHistory.Rows.Count > 0)
                {
                    List<Call_History> entityList = new List<Call_History>();
                    foreach (DataRow dr in dtCallsHistory.Rows)
                    {
                        Call_History ch = new Call_History(dr, _group_record);
                        ch.OnEntityMessage += new EntityMessage_Handler(ch_OnEntityMessage);
                        entityList.Add(ch);
                    }

                    if (entityList.Count > 0)
                    {
                        var tasks = entityList.Select(entity => Task.Factory.StartNew(() => entity.ProcessDataFlow())).ToArray();
                        Task.WaitAll(tasks);
                    }
                    else
                    {
                        LogMessage("No records to process for PNC Calls History", "INFO");
                    }
                }
            }
        }
        #endregion
    }
}

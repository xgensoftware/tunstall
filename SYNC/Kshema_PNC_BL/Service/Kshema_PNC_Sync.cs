using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.Xgensoftware.Core;
using Kshema_PNC.BL.Repository;
using Kshema_PNC.BL.Entity;

namespace Kshema_PNC.BL.Service
{
    public class Kshema_PNC_Sync
    {
        #region " Member Variables "
        ServiceRepository _service_repository = null;
        string _group_record;
        #endregion

        #region " Private Methods "

        void e_OnEntityMessage(string message,string log_type)
        {
            _service_repository.Log_Service_Message(log_type, message);
            Console.WriteLine(string.Format("{0}    {1}: {2}", DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"), log_type, message));
        }

        void Parallel_Log_Update(DataTable dtLog)
        {          

            int max_parallelism = 4;
            Parallel.ForEach(dtLog.AsEnumerable(), new ParallelOptions { MaxDegreeOfParallelism = max_parallelism }, dr =>
            {
                ProcessLogUpdate(dr);
            });
        }

        void ProcessLogUpdate(DataRow dr)
        {
            try
            {
                int Id = dr["Id"].Parse<int>();
                this._service_repository.Update_Log_Row(Id);
            }
            catch { }
        }

        void Reset_Sync_Steps()
        {
            //this will reset the table 
            _service_repository.Reset_Sync_Steps();
            _service_repository.Log_Service_Message("INFO", "Resetting Sync Steps");
        }

        void Move_ChangeLog_From_Stage()
        {
            _service_repository.Update_Sync_Step("Move Change Log from Stage");

            try
            {
                _service_repository.Move_Change_Log_Transactions(this._group_record);
            }
            catch (Exception ex)
            {
                _service_repository.Log_Service_Message("ERROR", string.Format("Failed to move transaction for the EtlChangeLog_Stage to EtlChangeLog. ERROR: {0}",ex.Message));
            }

        }

        void Insert_Map_Temp_Equip_Id()
        {
            _service_repository.Update_Sync_Step("Insert Temp Equip Id");

            try
            {
                _service_repository.Insert_Temp_Map_Equip_Id();
            }
            catch (Exception ex)
            {
                _service_repository.Log_Service_Message("ERROR", string.Format("Failed to insert Map_Temp_Equip_Id. ERROR: {0}",ex.Message));
            }
        }

        void ProcessEntityFlow<T>(string spName, string process)
        {
            //string sql = string.Empty;
            List<IEntity> entityList = new List<IEntity>();

            DataSet dsChangeLog = null;
            try
            {
                dsChangeLog = this._service_repository.Run_Query_SP(spName);
            }
            catch (Exception ex)
            {
                _service_repository.Log_Service_Message("ERROR", string.Format("ProcessEntityFlow failed to get change log for query {0}. ERROR: {1}", spName, ex.Message));
            }


            if (dsChangeLog != null)
            {
                DataTable dtChangeLog = dsChangeLog.Tables[0];
                foreach (DataRow dr in dtChangeLog.Rows)
                {
                    IEntity e = (IEntity)Activator.CreateInstance(typeof(T), dr, _group_record);
                    e.OnEntityMessage += new EntityMessage_Handler(e_OnEntityMessage);
                    entityList.Add(e);
                }

                if (entityList.Count > 0)
                {
//#if !DEBUG
                    int max_parallelism = 4;

                    Parallel.ForEach(entityList, new ParallelOptions { MaxDegreeOfParallelism = max_parallelism }, entity =>
                    {
                        entity.ProcessDataFlow(process);
                    });
//#else
//                    foreach (IEntity entity in entityList)
//                    {
//                        entity.ProcessDataFlow(process);
//                    }
//#endif

                    _service_repository.Log_Service_Message("INFO", string.Format("Process update log for {0}", spName));
                    this.Parallel_Log_Update(dsChangeLog.Tables[1]);
                }
                else
                {

                    _service_repository.Log_Service_Message("INFO", string.Format("No records to process for {0}", spName));

                }
            }

            #region OLD
            //try
            //{
            //    sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, queryFile));
            //}
            //catch (Exception ex)
            //{
            //    _service_repository.Log_Service_Message("ERROR", string.Format("ProcessEntityFlow failed to retrieve query file {0}. ERROR: {1}", queryFile, ex.Message));
            //}

            //if (!string.IsNullOrEmpty(sql))
            //{
            //    DataSet dsChangeLog = null;
            //    try
            //    {
            //        dsChangeLog = this._service_repository.Run_Query_File(sql);
            //    }
            //    catch (Exception ex)
            //    {
            //        _service_repository.Log_Service_Message("ERROR", string.Format("ProcessEntityFlow failed to get change log for query {0}. ERROR: {1}", queryFile, ex.Message));
            //    }


            //    if (dsChangeLog != null)
            //    {
            //        DataTable dtChangeLog = dsChangeLog.Tables[0];
            //        foreach (DataRow dr in dtChangeLog.Rows)
            //        {
            //            IEntity e = (IEntity)Activator.CreateInstance(typeof(T), dr,_group_record);
            //            e.OnEntityMessage += new EntityMessage_Handler(e_OnEntityMessage);
            //            entityList.Add(e);
            //        }

            //        if (entityList.Count > 0)
            //        {
            //            int max_parallelism = 4;

            //            Parallel.ForEach(entityList, new ParallelOptions{MaxDegreeOfParallelism = max_parallelism},entity =>
            //                {
            //                    entity.ProcessDataFlow(process);
            //                });

            //            _service_repository.Log_Service_Message("INFO", string.Format("Process update log for {0}", queryFile));
            //            this.Parallel_Log_Update(dsChangeLog.Tables[1]);
            //        }
            //        else
            //        {

            //            _service_repository.Log_Service_Message("INFO",string.Format("No records to process for {0}", queryFile));

            //        }
            //    }
            //}
            #endregion
        }

        void ProcessSubscriber(string spName, string process)
        {
            // use this method for subscriber, since processing them in parallel causes to 
            // many issues with Provider Link, and join/unjoin.

            DataSet dsChangeLog = null;
            try
            {
                dsChangeLog = this._service_repository.Run_Query_SP(spName);
            }
            catch (Exception ex)
            {
                _service_repository.Log_Service_Message("ERROR", string.Format("ProcessSubscriber failed to get change log for query {0}. ERROR: {1}", spName, ex.Message));
            }


            if (dsChangeLog != null)
            {
                DataTable dtSubsToProcess = dsChangeLog.Tables[0];
                foreach (DataRow dr in dtSubsToProcess.Rows)
                {
                    Subscriber e = new Subscriber(dr, _group_record);
                    e.OnEntityMessage += new EntityMessage_Handler(e_OnEntityMessage);
                    e.ProcessDataFlow(process);
                }

                _service_repository.Log_Service_Message("INFO", string.Format("Process update log for {0}", spName));
                this.Parallel_Log_Update(dsChangeLog.Tables[1]);
            }
            else
                _service_repository.Log_Service_Message("INFO", string.Format("No records to process for {0}", spName));

            #region OLD
            //string sql = string.Empty;

            //try
            //{
            //    sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, queryFile));
            //}
            //catch (Exception ex)
            //{
            //    _service_repository.Log_Service_Message("ERROR", string.Format("ProcessSubscriber failed to retrieve query file {0}. ERROR: {1}", queryFile, ex.Message));
            //}

            //if (!string.IsNullOrEmpty(sql))
            //{
            //    DataSet dsChangeLog = null;
            //    try
            //    {
            //        dsChangeLog = this._service_repository.Run_Query_File(sql);
            //    }
            //    catch (Exception ex)
            //    {
            //        _service_repository.Log_Service_Message("ERROR", string.Format("ProcessSubscriber failed to get change log for query {0}. ERROR: {1}", queryFile, ex.Message));
            //    }


            //    if (dsChangeLog != null)
            //    {
            //        DataTable dtSubsToProcess = dsChangeLog.Tables[0];
            //        foreach (DataRow dr in dtSubsToProcess.Rows)
            //        {
            //            Subscriber e = new Subscriber(dr, _group_record);
            //            e.OnEntityMessage += new EntityMessage_Handler(e_OnEntityMessage);                        
            //            e.ProcessDataFlow(process);
            //        }

            //        _service_repository.Log_Service_Message("INFO", string.Format("Process update log for {0}", queryFile));
            //        this.Parallel_Log_Update(dsChangeLog.Tables[1]);
            //    }
            //    else
            //        _service_repository.Log_Service_Message("INFO", string.Format("No records to process for {0}", queryFile));

            //}
            #endregion
        }

        //void ProcessTest(string queryFile, string process)
        //{
        //    // use this method for subscriber, since processing them in parallel causes to 
        //    // many issues with Provider Link, and join/unjoin.

        //    string sql = string.Empty;
        //    List<IEntity> entityList = new List<IEntity>();

        //    try
        //    {
        //        sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, queryFile));
        //    }
        //    catch (Exception ex)
        //    {
        //        _service_repository.Log_Service_Message("ERROR", string.Format("ProcessSubscriber failed to retrieve query file {0}. ERROR: {1}", queryFile, ex.Message));
        //    }

        //    if (!string.IsNullOrEmpty(sql))
        //    {
        //        DataTable dtChangeLog = null;
        //        try
        //        {
        //            dtChangeLog = this._service_repository.Run_Query_File(sql);
        //        }
        //        catch (Exception ex)
        //        {
        //            _service_repository.Log_Service_Message("ERROR", string.Format("ProcessSubscriber failed to get change log for query {0}. ERROR: {1}", queryFile, ex.Message));
        //        }


        //        if (dtChangeLog != null)
        //        {
        //            foreach (DataRow dr in dtChangeLog.Rows)
        //            {
        //                _service_repository.Log_Service_Message("TEST", string.Format("Subscriber {0}", dr["Subscriber_Id"].ToString()));
        //            }
        //        }
        //    }
        //}
        #endregion

        #region " Constructor "
        public Kshema_PNC_Sync(string group_record)
        {
            _service_repository = new ServiceRepository();
            _group_record = group_record;
        }
        #endregion

        #region " Public Methods "
        public void Run_Sync()
        {

            Reset_Sync_Steps();

            // move from stage to log
            Move_ChangeLog_From_Stage();

            _service_repository.Log_Service_Message("INFO", "Kshema to PNC Sync Started");

            //Process the Subscriber Records
            this.ProcessSubscriber("SYNC_Get_Subscribers_Insert", "INSERT");
            _service_repository.Update_Sync_Step("Processed Subscriber Inserts to PNC");

            this.ProcessSubscriber("SYNC_Get_Subscribers_Update", "UPDATE");
            _service_repository.Update_Sync_Step("Processed Subscriber updates to PNC");

            //this.ProcessEntityFlow<Subscriber>("Get_Subscribers_Delete.sql","DELETE");
            //this.ProcessSubscriber("Get_Subscribers_Delete.sql","DELETE");
            //_service_repository.Update_Sync_Step("Processed Subscriber deletes to PNC");

            //process the SubscriberResponder Records
            this.ProcessEntityFlow<SubscriberResponder>("SYNC_Get_SubscriberResponders", "NONE");
            _service_repository.Update_Sync_Step("Processed SubscriberResponders to PNC");

            //process keyword inserts
            this.ProcessEntityFlow<SubscriberMedicalInfo>("SYNC_Get_SubscriberMedicalInfo", "NONE");
            _service_repository.Update_Sync_Step("Processed Subscriber keywords Inserts to PNC");

            _service_repository.Log_Service_Message("INFO", "Kshema to PNC Sync Completed");

        }
        #endregion
    }
}

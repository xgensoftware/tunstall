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
    public class PNC_Kshema_Sync : SyncBase, ISync
    {
        string _group_record;
        #region " Constructor "
        public PNC_Kshema_Sync(string group_record)
        {
            _group_record = group_record;
        }
        #endregion

        #region " Private Methods "
        void _ActiveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.RunSync();
        }

        void LogMessage(string message, string log_type)
        {
            string msg = string.Format("{0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
            Console.WriteLine(msg);

            this.LogServiceMessage("PNC_Kshema_Sync", log_type, message);
        }

        void ProcessEntityFlow<T>(string queryFile)
        {
            string sql = string.Empty;
            List<IEntity> entityList = new List<IEntity>();

            try
            {
                sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, queryFile));
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("ProcessEntityFlow failed to retrieve query file {0}. ERROR: {1}", queryFile,ex.Message),"ERROR");
            }

            if (!string.IsNullOrEmpty(sql))
            {
                DataTable dtChangeLog = null;

                try
                {
                    dtChangeLog = _pncProvider.GetData(sql, null);
                }
                catch (Exception ex) { LogMessage(string.Format("ProcessEntityFlow failed to get change log for query {0}. ERROR: {1}", queryFile,ex.Message),"ERROR"); }


                if (dtChangeLog != null || dtChangeLog.Rows.Count >0)
                {
                    foreach (DataRow dr in dtChangeLog.Rows)
                    {
                        IEntity e = (IEntity)Activator.CreateInstance(typeof(T), dr,_group_record);
                        e.OnEntityMessage += new EntityMessage_Handler(e_OnEntityMessage);
                        entityList.Add(e);
                    }


                    if (entityList.Count > 0)
                    {
#if !DEBUG
                        var tasks = entityList.Select(entity => Task.Factory.StartNew(() => entity.ProcessDataFlow())).ToArray();
                        Task.WaitAll(tasks);
#else
                        foreach (IEntity e in entityList)
                            e.ProcessDataFlow();
#endif

                    }
                    else
                    {
                        LogMessage(string.Format("No records to process for {0}", queryFile), "INFO");
                    }
                }
            }
        }

        void ProcessEntityDelete<T>(string queryFile)
        {
            string sql = string.Empty;
            List<IEntity> entityList = new List<IEntity>();

            try
            {
                sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, queryFile));
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("ProcessEntityDelete failed to retrieve query file {0}. ERROR: {1}", queryFile, ex.Message),"ERROR");
            }

            if (!string.IsNullOrEmpty(sql))
            {
                DataTable dtChangeLog = null;

                try
                {
                    dtChangeLog = _pncProvider.GetData(sql, null);
                }
                catch (Exception ex) { LogMessage(string.Format("ProcessEntityFlow failed to get change log for query {0}. ERROR: {1}", queryFile, ex.Message),"ERROR"); }


                if (dtChangeLog != null || dtChangeLog.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtChangeLog.Rows)
                    {
                        IEntity e = (IEntity)Activator.CreateInstance(typeof(T), dr, _group_record);
                        e.OnEntityMessage += new EntityMessage_Handler(e_OnEntityMessage);
                        entityList.Add(e);
                    }


                    if (entityList.Count > 0)
                    {
                        var tasks = entityList.Select(entity => Task.Factory.StartNew(() => entity.ProcessDataDelete())).ToArray();
                        Task.WaitAll(tasks);
                    }
                }
            }
        }

        //void ProcessEpecFlow()
        //{
        //    string sql = string.Empty;
        //    List<Epec> epec_To_Process = new List<Epec>();
            
        //    try
        //    {
        //        sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, "EPEC_Query.sql"));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage(string.Format("ProcessEpecFlow failed to retrieve query file. ERROR: {0}", ex.Message));
        //    }

        //    if (!string.IsNullOrEmpty(sql))
        //    {
        //        DataTable dtChangeLog = null;

        //        try
        //        {
        //            dtChangeLog = _pncProvider.GetData(sql, null);
        //        }
        //        catch (Exception ex) { LogMessage(string.Format("ProcessEpecFlow failed to get change log. ERROR: {0}", ex.Message)); }

        //        if (dtChangeLog != null)
        //        {
        //            foreach (DataRow dr in dtChangeLog.Rows)
        //            {
        //                Epec e = new Epec(dr);
        //                e.OnEpecMessage += new EntityBase.EntityMessage_Handler(e_OnEntityMessage);
        //                epec_To_Process.Add(e);
        //            }


        //            if (epec_To_Process.Count > 0)
        //            {
        //                var tasks = epec_To_Process.Select(epec => Task.Factory.StartNew(() => epec.ProcessDataFlow())).ToArray();
        //                Task.WaitAll(tasks);
        //            }
        //        }
        //    }
        //}

        //void ProcessResidentFlow()
        //{
        //    string sql = string.Empty;
        //    List<Resident> resident_To_Process = new List<Resident>();

        //    try
        //    {
        //        sql = FileReader.GetFileData(string.Format("{0}{1}", AppConfiguration.Query_Files_Path, "EPEC_Query.sql"));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogMessage(string.Format("ProcessResidentFlow failed to retrieve query file. ERROR: {0}", ex.Message));
        //    }

        //    if (!string.IsNullOrEmpty(sql))
        //    {
        //        DataTable dtChangeLog = null;

        //        try
        //        {
        //            dtChangeLog = _pncProvider.GetData(sql, null);
        //        }
        //        catch (Exception ex) { LogMessage(string.Format("ProcessResidentFlow failed to get change log. ERROR: {0}", ex.Message)); }

        //        if (dtChangeLog != null)
        //        {
        //            foreach (DataRow dr in dtChangeLog.Rows)
        //            {
        //                Resident e = new Resident(dr);
        //                e.OnResidentMessage += new EntityBase.EntityMessage_Handler(e_OnEntityMessage);
        //                resident_To_Process.Add(e);
        //            }


        //            if (resident_To_Process.Count > 0)
        //            {
        //                var tasks = resident_To_Process.Select(resident => Task.Factory.StartNew(() => resident.ProcessDataFlow())).ToArray();
        //                Task.WaitAll(tasks);
        //            }
        //        }
        //    }
        //}

        void e_OnEntityMessage(string message,string message_type)
        {
            LogMessage(message,message_type);
        }
        #endregion

        #region " Interface Implementation "
        public void Start()
        {
            this.LogMessage("PNC_Kshema_Sync Starting....", "INFO");

            _ActiveTimer = new Timer();
            _ActiveTimer.Interval = AppConfiguration.ServiceTimerInterval;
            _ActiveTimer.Elapsed += new ElapsedEventHandler(_ActiveTimer_Elapsed);
            _ActiveTimer.Start();

        }

        public void RunSync()
        {
            this.InitializeProvider();

            //TODO: move to the KSHEMA_PNC sync since it runs first
            //base.Reset_Sync_Steps();
            this.LogMessage("PNC to Kshema Sync Started","INFO");

            //1. run the epec sync
            this.ProcessEntityFlow<Epec>("EPEC_Query.sql");
            this.Update_Sync_Step("Processing Epec Log");

            // process the Subscriber Queue
            Epec.Process_Epec_Stage_Queue();
            this.Update_Sync_Step("Processing Epec Stage Queue");

            //2. run the resident sync
            this.ProcessEntityFlow<Resident>("Get_Resident_Log.sql");
            this.Update_Sync_Step("Processing Resident Log");

            //3. Process the subscriber queue
            Epec.Process_Epec_Stage_Queue();
            this.Update_Sync_Step("Processing Epec Stage Queue");

            //4. Process Contacts
            this.ProcessEntityFlow<Contact>("Get_Contact_Log.sql");
            this.Update_Sync_Step("Processing Contact Log");

            Contact.Process_Contact_Temp_Queue();
            this.Update_Sync_Step("Processing Contact Stage Queue");


            //5. Language
            this.ProcessEntityFlow<Language>("Get_Language_Log.sql");
            this.Update_Sync_Step("Processing Language Log");

            //6. Keywords
            this.ProcessEntityFlow<Keyword>("Get_Keyword_Log.sql");
            this.Update_Sync_Step("Processing Keyword Log");

            Keyword.Delete_KS_SubscriberMedicalInfo_2();
            this.Update_Sync_Step("Deleting Subscriber Medical Info"); 

            Keyword.Process_SubscriberMedicalInfo();
            this.Update_Sync_Step("Processing Subscriber Medical Info");                      

            //7. Notes
            this.ProcessEntityFlow<Note>("Get_Notes_Log.sql");
            this.Update_Sync_Step("Processing Notes Log");

            //8. Contact Relation
            this.ProcessEntityFlow<ContactRelation>("Get_ContactRelation_Log.sql");
            this.Update_Sync_Step("Processing Contact Relation Log");

            //9. Responder Call Order
            Contact.Update_Responder_Call_Order();
            this.Update_Sync_Step("Updating Responder Call Order");

            //10. Contact Delete
            this.ProcessEntityDelete<Contact>("Contact_Delete_Log.sql");
            this.Update_Sync_Step("Processing Contact Delete Log");

            //11. Note Delete
            this.ProcessEntityDelete<Note>("Note_Delete_Log.sql");
            this.Update_Sync_Step("Processing Note Delete Log");

            this.Update_Sync_Step("Deleting duplicate contacts");
            Contact.Delete_Duplicate_Contacts();

            this.LogMessage("PNC to Kshema Sync Completed", "INFO");

            this.DisposeProvider();
        }

        public void Stop()
        {
            this.LogMessage("PNC_Kshema_Sync Stopping....", "INFO");

            if (_ActiveTimer != null)
            {
                _ActiveTimer.Stop();

                if (_ActiveTimer.Enabled == false)
                    _ActiveTimer.Dispose();
            }

            base.DisposeProvider();
        }
        #endregion
    }
}

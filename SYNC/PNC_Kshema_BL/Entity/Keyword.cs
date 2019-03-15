using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class Keyword : EntityBase, IEntity
    {
        #region " Member Variables "
        int _keyword_no;
        int _keyword_ref;

        string _keyword_text;
        #endregion

        #region " Constructor "

        public Keyword(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();
        }
        #endregion

        #region " Private Methods "
        private bool Map_Keyword_Fields()
        {
            bool isMapped = true;

            try
            {
                _residentDef = _dataToMapp["resident_ref"].Parse<int>();
                _keyword_no = _dataToMapp["keyword_no"].Parse<int>();
                _keyword_ref= _dataToMapp["keyword_ref"].Parse<int>();
                _keyword_text = _dataToMapp["keyword_text"].ToString();
            }
            catch { }

            return isMapped;
        }

        private bool Keyword_Map_Subscriber_Resident()
        {
            bool isMapped = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@locdef", _locationDef));
                this._sqlParms.Add(SQLParam.GetParam("@resdef", _residentDef));
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", "-1"));

                _Subscriber_ID = _stageProvider.ExecuteScalar<string>("SYNC_Map_Subscriber_Resident", _sqlParms);
                
                if (_Subscriber_ID == "-1")
                    isMapped = false;
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Keyword_Map_Subscriber_Resident error: {0} on resident_def {1}.", ex.Message, _residentDef.ToString()),"ERROR");
                isMapped = false;
            }

            //this.EntityMessage(string.Format("Keyword_Map_Subscriber_Resident is_mapped: {0} on resident_def {1}.", isMapped.ToString(), _residentDef.ToString()));
            return isMapped;
        }

        private bool Map_SubMedInfo_To_Keyword()
        {
            bool isMapped = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@keyword_ref", _keyword_ref));
                DataRow drKeyword = _stageProvider.ExecuteDataSet("SYNC_Map_SubMedInfo_Keyword", _sqlParms).Tables[0].Rows[0];

                _type = drKeyword["Type"].ToString();
                _Code = drKeyword["Code"].ToString();

                this.EntityMessage(string.Format("Map_SubMedInfo_To_Keyword found Type {0}, Code {1} for keyword_ref {2}.", _type,_Code,_keyword_ref.ToString()),"INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("No mapping found for keyword_ref {0} for resident_def {1}.",   _keyword_ref.ToString(),_residentDef.ToString()),"INFO");
                isMapped = false;
            }
            return isMapped;
        }

        private bool Subscriber_Medical_Info_Lookup()
        {
            bool isMapped = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@subscriber_ID", _Subscriber_ID));
                _sqlParms.Add(SQLParam.GetParam("@id", _Code));
                _sqlParms.Add(SQLParam.GetParam("@type", _type));
                _SubscriberMedicalInfo = _stageProvider.ExecuteScalar<int>("SYNC_Subscriber_Medical_Info_Lookup", _sqlParms);
                if (_SubscriberMedicalInfo == -1)
                    isMapped = false;

            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Map_SubMedInfo_To_Keyword error: {0} on resident_def {1}, keyword_ref {2}.", ex.Message, _residentDef.ToString(), _keyword_ref.ToString()),"INFO");
                isMapped = false;
            }

            //this.EntityMessage(string.Format("Subscriber_Medical_Info_Lookup is_mapped {0} for Subscriber_ID {1} Type {2} Code {3}.",isMapped.ToString(),_Subscriber_ID,_type,_Code));
            return isMapped;
        }

        private void Insert_Keyword_To_Queue()
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@subscriber_ID", _Subscriber_ID));
                _sqlParms.Add(SQLParam.GetParam("@Id", _Code));
                _sqlParms.Add(SQLParam.GetParam("@type", _type));
                _stageProvider.ExecuteNonQuery("SYNC_Insert_Keyword_SSIS_Q",_sqlParms);

                this.EntityMessage(string.Format("Insert_Keyword_To_Queue successfully inserted Type {0} Code {1} for Subscriber_Id {2}", _type,_Code,_Subscriber_ID),"INFO");
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Insert_Keyword_To_Queue failed to insert Type {0} Code {1} for Subscriber_ID {2}. ERROR: {3}", _type, _Code, _Subscriber_ID,ex.Message),"ERROR");
            }
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow()
        {
            bool processKeywords = this.Map_Keyword_Fields();
            if (Keyword_Map_Subscriber_Resident())
                if (Map_SubMedInfo_To_Keyword())
                    Insert_Keyword_To_Queue();

                    //if (!Subscriber_Medical_Info_Lookup())
                    //    Insert_Keyword_To_Queue();
                        
        }

        public static void Process_SubscriberMedicalInfo()
        {
            IDataProvider pncStage = null;

            try
            {
                pncStage = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
                pncStage.ExecuteNonQuery("TEMP_KEYWORD_INSERT_SUBSCRIBERMEDICALINFO", null);
            }
            catch { }

            if (pncStage != null)
                pncStage = null;
        }

        public static void Delete_KS_SubscriberMedicalInfo_2()
        {
            IDataProvider pncStage = null;

            try
            {
                pncStage = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
                pncStage.ExecuteNonQuery("SYNC_Delete_SubscriberMedInfo2", null);
            }
            catch { }

            if (pncStage != null)
                pncStage = null;
        }
        #endregion
    }
}

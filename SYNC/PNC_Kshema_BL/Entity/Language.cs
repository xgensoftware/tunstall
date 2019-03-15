using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class Language : EntityBase, IEntity
    {
        #region " Member Variables "

        int _entity_ref;
        int _attr_category_def;
        int _attr_choice_def;

        string _attr_category_text;
        string _attr_choice_text;
        #endregion

        #region " Private Methods "
        private bool Map_Language_Fields()
        {
            bool isMapped = true;

            try
            {
                _entity_ref = _dataToMapp["entity_ref"].Parse<int>();
                _attr_category_def = _dataToMapp["attr_category_def"].Parse<int>();
                _attr_choice_def = _dataToMapp["attr_choice_def"].Parse<int>();
                _attr_category_text = _dataToMapp["attr_category_text"].ToString();
                _attr_choice_text = _dataToMapp["attr_choice_text"].ToString();
            }
            catch { }

            return isMapped;
        }

        private bool Map_Language_Code_Attr()
        {
            bool isMapped = true;
            string msg;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@attr_category_ref", _attr_category_def));
                _sqlParms.Add(SQLParam.GetParam("@attr_choice_ref", _attr_choice_def));
                DataRow drAgency = _stageProvider.GetData("SYNC_Map_Language_Code_Attr", _sqlParms).Rows[0];

                _Code = drAgency["Code"].ToString();
                _type = drAgency["Type"].ToString();

                EntityMessage(string.Format("Successfully mapped Type {0} and Code {1}.", _type, _Code), "INFO");
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Error in Map_Langauge_Code_Attr. Error: {0}",  ex.Message),"ERROR");
            }

            return isMapped;
        }

        private bool Language_Map_Subscriber_Resident()
        {
            bool isMapped = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@locdef", -1));
                this._sqlParms.Add(SQLParam.GetParam("@resdef", _entity_ref));
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", "-1"));
                _Subscriber_ID = _stageProvider.ExecuteScalar<string>("SYNC_Map_Subscriber_Resident", _sqlParms);

                if (_Subscriber_ID == "-1")
                    isMapped = false;               
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Language_Map_Subscriber_Resident error: {0} on entity_ref {1}.",ex.Message,_entity_ref.ToString()),"ERROR");
                isMapped = false;
            }
            return isMapped;
        }

        private string Subscriber_Language_Lookup()
        {
            string process = "UPDATE";

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@SubscriberId", _Subscriber_ID));
                int count = _stageProvider.ExecuteScalar<int>("SYNC_SubscriberLanguage_Lookup", _sqlParms);
                if (count == 0)
                    process = "INSERT";
            }
            catch(Exception ex)
            { 
                process = "ERROR";
                this.EntityMessage(string.Format("Subscriber_Language_Lookup failed. Subscriber_Id {0}, process {1}. Error: {2} on entity_ref {1}.", _Subscriber_ID,process,ex.Message),"ERROR");
            }
            
            return process;
        }

        private void Save_Subscriber_Language(string command)
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@subscriberId", _Subscriber_ID));
                _sqlParms.Add(SQLParam.GetParam("@type", _type));
                _sqlParms.Add(SQLParam.GetParam("@code", _Code));
                _sqlParms.Add(SQLParam.GetParam("@command",command));
                _stageProvider.ExecuteNonQuery("SYNC_Save_SubscriberLanguage", _sqlParms);
            }
            catch { }
        }

        private void Update_Subscriber_Gender()
        {
            try
            {
                string gender = "M";
                if (_attr_choice_def == 100000051)
                    gender = "F";



                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@Subscriber_Id", _Subscriber_ID));
                _sqlParms.Add(SQLParam.GetParam("@Gender", gender));
                _stageProvider.ExecuteNonQuery("SYNC_Update_Subscriber_Gender ", _sqlParms);

                this.EntityMessage(string.Format("Subscriber Id {0} gender updated to {1}", _Subscriber_ID, gender),"INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Error updating Subscriber gender. Subscriber_Id: {0}. ERROR: {1}.", _Subscriber_ID,ex.Message),"ERROR");
            }
        }
        #endregion

        #region " Constructor "
        public Language(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow()
        {
            bool processKeywords = this.Map_Language_Fields();
            if (processKeywords)
            {
                if (Language_Map_Subscriber_Resident())
                {
                    // check for a gender change and hard code it
                    //else run the normal procedure
                    if (_attr_category_def == 4)
                    {
                        this.Update_Subscriber_Gender();
                    }
                    else
                    {
                        if (Map_Language_Code_Attr())
                        {
                            string process = Subscriber_Language_Lookup();
                            this.Save_Subscriber_Language(process);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

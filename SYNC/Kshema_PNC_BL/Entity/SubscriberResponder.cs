using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace Kshema_PNC.BL.Entity
{
    class SubscriberResponder : EntityBase, IEntity
    {
        #region " Member Variables "
        
        string _subscriber_id;
        string _relation_code;
        string _first_name;
        string _last_name;
        string _phone_1;
        string _phone_2;
        string _phone_3;
        string _notes_yn;
        string _keyholder_yn;
        string _nok_yn;
        string _availability;
        string _email_address;
        string _agency_id;

        bool _is_deleted = false;

        int _subscriberresponder_id;
        long _location_def;
        long _resident_def;
        long _contact_def;
        int _priority;
        int _type_ref;
        int _authority_ref;
        int _entity_type;
        long _relation_def;
        #endregion

        #region " Constructor "
        public SubscriberResponder(DataRow dr, string group_record)
        {
            this._dataToProcess = dr;
            this._group_record = group_record;

            this.InitializeProviders();
        }
        #endregion

        #region " Private Methods "
        bool Map_Properties()
        {
            bool isSuccess = true;

            try
            {                
                this._subscriberresponder_id = _dataToProcess["SubscriberResponder_Id"].Parse<int>();
                this._location_def = _dataToProcess["LOCATION_DEF"].Parse<long>();
                this._resident_def = _dataToProcess["RESIDENT_DEF"].Parse<int>();
                this._type_ref = _dataToProcess["TYPE_REF"].Parse<int>();
                this._contact_def = _dataToProcess["Contact_Def"].Parse<long>();
                this._authority_ref = _dataToProcess["AUTHORITY_REF"].Parse<int>();
                this._entity_type = _dataToProcess["Entity_Type"].Parse<int>();

                this._subscriber_id = _dataToProcess["Subscriber_Id"].ToString();
                this._first_name = _dataToProcess["FirstName"].ToString().Replace("'", "");
                this._last_name = _dataToProcess["LastName"].ToString().Replace("'", "");
                this._relation_code = _dataToProcess["RelationCode"].ToString();
                this._notes_yn = _dataToProcess["Notes_YN"].ToString();
                this._keyholder_yn = _dataToProcess["Keyholder_YN"].ToString();
                this._nok_yn = _dataToProcess["NOK_YN"].ToString();
                this._availability = _dataToProcess["Availability"].ToString();
                this._email_address = _dataToProcess["Email_Address"].ToString();
                this._agency_id = _dataToProcess["Agency_Id"].ToString();
                
                if (!string.IsNullOrEmpty(_dataToProcess["Phone_1"].ToString()))
                    this._phone_1 = _dataToProcess["Phone_1"].ToString();

                if (!string.IsNullOrEmpty(_dataToProcess["Phone_2"].ToString()))
                    this._phone_2 = _dataToProcess["Phone_2"].ToString();

                if (!string.IsNullOrEmpty(_dataToProcess["Phone_3"].ToString()))
                    this._phone_3 = _dataToProcess["Phone_3"].ToString();

                if (!string.IsNullOrEmpty(_dataToProcess["AUTHORITY_REF"].ToString()))
                    this._authority_ref = _dataToProcess["AUTHORITY_REF"].Parse<int>();

                if (_dataToProcess["Deleted"].ToString() == "Y")
                    _is_deleted = true;

                this._priority = _dataToProcess["Priority"].Parse<int>();

            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.EntityMessage(string.Format("Failed to map properties for Subscriber {0}. Error: {1}", _dataToProcess["Subscriber_Id"].ToString(), ex.Message),"ERROR");
            }

            return isSuccess;
        }

        bool Save_Contact(string process_type)
        {
            string err_message;
            bool isSuccess = true;

            if (process_type == "INSERT")
            {
                try
                {
                    #region " Old - does not work when processed in parallel "
                    //this._sqlParms = new List<SQLParam>();
                    //this._sqlParms.Add(SQLParam.GetParam("@type_ref", this._type_ref));
                    //this._sqlParms.Add(SQLParam.GetParam("@first_name", this._first_name));
                    //this._sqlParms.Add(SQLParam.GetParam("@last_name", this._last_name));
                    //this._sqlParms.Add(SQLParam.GetParam("@phone_1", this._phone_1));
                    //this._sqlParms.Add(SQLParam.GetParam("@phone_2", this._phone_2));
                    //this._sqlParms.Add(SQLParam.GetParam("@phone_3", this._phone_3));
                    //this._sqlParms.Add(SQLParam.GetParam("@email_address", this._email_address));
                    //this._sqlParms.Add(SQLParam.GetParam("@subscriber_responder", this._subscriberresponder_id));
                    //this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                    //this._sqlParms.Add(SQLParam.GetParam("@notes_yn", this._notes_yn));
                    //this._sqlParms.Add(SQLParam.GetParam("@keyholder_yn", this._keyholder_yn));
                    //this._sqlParms.Add(SQLParam.GetParam("@authority_ref", this._authority_ref));
                    //this._sqlParms.Add(SQLParam.GetParam("@availability", this._availability));
                    //this._contact_def = _stageProvider.ExecuteDataSet("SYNC_Insert_Contact", this._sqlParms).Tables[0].Rows[0]["CONTACT_DEF"].Parse<long>();
                    #endregion

                    this._sql = new StringBuilder();
                    this._sql.Append("insert into STANDARD.CONTACTS(CONTACT_TYPE_REF,FIRST_NAME,LAST_NAME");

                    if (!string.IsNullOrEmpty(this._phone_1))
                        this._sql.Append(",PHONE_1");

                    if (!string.IsNullOrEmpty(this._phone_2))
                        this._sql.Append(",PHONE_2");

                    if (!string.IsNullOrEmpty(this._phone_3))
                        this._sql.Append(",PHONE_3");

                    this._sql.Append(",EMAIL_ADDRESS,NOTES_YN,AUTHORITY_REF,AVAILABILITY)");
                    this._sql.AppendFormat("values({0}, '{1}', '{2}'", this._type_ref.ToString(), this._first_name, this._last_name);

                    if (!string.IsNullOrEmpty(this._phone_1))
                        this._sql.AppendFormat(", {0}", this._phone_1);

                    if (!string.IsNullOrEmpty(this._phone_2))
                        this._sql.AppendFormat(", {0}", this._phone_2);

                    if (!string.IsNullOrEmpty(this._phone_3))
                        this._sql.AppendFormat(", {0}", this._phone_3);

                    this._sql.AppendFormat(", '{0}', '{1}', {2}, '{3}')", this._subscriberresponder_id.ToString(), this._notes_yn, this._authority_ref.ToString(), this._availability);                    
                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                    this.EntityMessage(string.Format("Performing an insert on Contact. QUERY: {0}",this._sql.ToString()), "INFO");

                    //get the contact def
                    this._sql = new StringBuilder();
                    this._sql.AppendFormat("select CONTACT_DEF from STANDARD.CONTACTS where EMAIL_ADDRESS = '{0}'",this._subscriberresponder_id.ToString());
                    this._contact_def = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0]["CONTACT_DEF"].Parse<int>();
                    this.EntityMessage(string.Format("Found contact_def {0} after insert contact.",this._contact_def.ToString()), "INFO");

                    //update the email address
                    this._sql = new StringBuilder();
                    this._sql.AppendFormat("update STANDARD.CONTACTS set EMAIL_ADDRESS = '{0}' where CONTACT_DEF = {1}",this._email_address,this._contact_def.ToString());
                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                    this.EntityMessage(string.Format("Update the email to {0} for contact_def {1} afte insert", this._email_address,this._contact_def.ToString()), "INFO");
                    

                    this.EntityMessage(string.Format("Successfully created contact_def {0} for subscriber_responder_Id {1}", this._contact_def.ToString(), this._subscriberresponder_id),"INFO");
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    err_message = string.Format("Failed to Insert Contact for Subscriber_Responder_Id {0}. ERROR: {1}", this._subscriberresponder_id.ToString(), ex.Message);
                    this._serviceRepository.Save_Transaction_Record(this._group_record, "SubscriberResponder", this._sql.ToString(), err_message);
                    this.EntityMessage(err_message,"ERROR");
                }

                if (isSuccess)
                {
                    // set the mappings
                    try
                    {
                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@contact_def", this._contact_def.ToString()));
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber_responder", this._subscriberresponder_id));
                        this._sqlParms.Add(SQLParam.GetParam("@type_ref", this._type_ref));
                        this._sqlParms.Add(SQLParam.GetParam("@keyholder_yn", this._keyholder_yn));
                        _stageProvider.ExecuteNonQuery("SYNC_Save_Contact_Mapping", this._sqlParms);
                    }
                    catch (Exception ex)
                    {
                        err_message = string.Format("Failed to save contact mapping for Subscriber_Responder_Id {0}. ERROR: {1}", this._subscriberresponder_id.ToString(), ex.Message);
                        this._serviceRepository.Save_Transaction_Record(this._group_record, "SubscriberResponder", "SYNC_Save_Contact_Mapping", err_message);
                        this.EntityMessage(err_message, "ERROR");
                    }
                }
            }
            else if(process_type == "UPDATE")
            {
                bool updateMapping = true;
                this._sql = new StringBuilder();
                this._sql.Append("Update STANDARD.CONTACTS ");
                this._sql.AppendFormat("set contact_type_ref = {0}, first_name = '{1}', last_name = '{2}'", this._type_ref.ToString(), this._first_name, this._last_name);
                this._sql.AppendFormat(",notes_yn = '{0}', email_address = '{1}'", this._notes_yn, this._email_address);
                this._sql.AppendFormat(",phone_1 = '{0}', phone_2 = '{1}', phone_3 = '{2}', availability = '{3}'", this._phone_1, this._phone_2, this._phone_3, this._availability);
                this._sql.AppendFormat(" where contact_def = {0}", this._contact_def.ToString());

                try
                {
                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                    this.EntityMessage(string.Format("Successfully update Contact {0} for Subscriber_Responder_Id {1}.", this._contact_def.ToString(),this._subscriberresponder_id.ToString()),"INFO");
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    err_message = string.Format("Failed to update Contact {0} for Subscriber_Responder_Id {1}.ERROR: {2}", this._contact_def.ToString(), this._subscriberresponder_id.ToString(), ex.Message);
                    this.EntityMessage(err_message,"ERROR");
                    this._serviceRepository.Save_Transaction_Record(this._group_record, "SubscriberResponder", this._sql.ToString(), err_message);
                    updateMapping = false;
                }


                // update the mapping 
                if (updateMapping)
                {
                    try
                    {
                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber_responder", this._subscriberresponder_id.ToString()));
                        this._sqlParms.Add(SQLParam.GetParam("@type_ref", this._type_ref));
                        this._sqlParms.Add(SQLParam.GetParam("@keyholder_yn", this._keyholder_yn));
                        this._stageProvider.ExecuteNonQuery("SYNC_Update_Map_Responder_Contact", this._sqlParms);
                        this.EntityMessage(string.Format("Successfully updated Map_Responder_Contact for SubscriberResponder_Id {0}", this._subscriberresponder_id.ToString()),"INFO");
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to update Map_Responder_Contact for SubscriberResponder_Id {0}. ERROR: {1}",this._subscriberresponder_id.ToString(),ex.Message),"ERROR");
                    }
                }
            }
            else if (process_type == "DELETE")
            {
                try
                {
                    // delete the contact relation
                    this._sql = new StringBuilder();
                    this._sql.AppendFormat("delete from CONTROLCENTRE.CONTACT_RELATION where CONTACT_REF = {0}", this._contact_def.ToString());
                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                    this._sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber_responder", this._subscriberresponder_id.ToString()));
                    this._stageProvider.ExecuteNonQuery("SYNC_Delete_Contact_Mappings", this._sqlParms);

                    this.EntityMessage(string.Format("Successfully deleted contact for SubscriberResponder_Id {0}", this._subscriberresponder_id.ToString()),"INFO");
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    err_message = string.Format("Failed to delete contact for SubscriberResponder_Id {0}. ERROR: {1}", this._subscriberresponder_id.ToString(), ex.Message);
                    this.EntityMessage(err_message, "ERROR");
                    this._serviceRepository.Save_Transaction_Record(this._group_record, "SubscriberResponder", this._sql.ToString(), err_message);
                    
                }
            }

            return isSuccess;
        }

        void Process_Contact_Relation()
        {
            StringBuilder sql = null;
            string process = "Inserted";
            DataRow drCR = null;
            string err_message;
            bool processMapping = true;
            this._relation_def = -1;  

            try
            {
                //this._sqlParms = new List<SQLParam>();
                //this._sqlParms.Add(SQLParam.GetParam("@location_ref", this._location_def));
                //this._sqlParms.Add(SQLParam.GetParam("@resident_ref", this._resident_def));
                //this._sqlParms.Add(SQLParam.GetParam("@contact_ref", this._contact_def));
                //DataTable dtCR = this._stageProvider.ExecuteDataSet("SYNC_Get_Contact_Relation", this._sqlParms).Tables[0];


                /*
                 * 
                 * select * 
                    from PNC_KSHEMA..CONTROLCENTRE.CONTACT_RELATION
                    where location_ref = @location_ref
                    and resident_ref = @resident_ref
                    and contact_ref = @contact_ref
                 * 
                 * */
                sql = new StringBuilder();
                sql.Append("select * from CONTROLCENTRE.CONTACT_RELATION");
                sql.AppendFormat(" where location_ref = {0}", this._location_def.ToString());
                sql.AppendFormat(" and resident_ref = {0}", this._resident_def.ToString());
                sql.AppendFormat(" and contact_ref = {0}", this._contact_def.ToString());
                DataTable dtCR = this._pncProvider.ExecuteDataSetQuery(sql.ToString(), null).Tables[0];
                
                if (dtCR.Rows.Count > 0)
                {
                    drCR = dtCR.Rows[0];
                    this._relation_def = drCR["RELATION_DEF"].Parse<long>();
                }
            }
            catch (Exception ex)
            {
                err_message = string.Format("Failed to get contact relation for Subscriber_Responder_Id {0}. Location_Def:{1} Resident_Def:{2} Contact_Def:{3}. ERROR: {4}"
                    , this._subscriberresponder_id.ToString()
                    , this._location_def.ToString()
                    , this._resident_def.ToString()
                    , this._contact_def.ToString()
                    , ex.Message);

                this.EntityMessage(err_message, "ERROR");
            }

            if (this._relation_def != -1)
            {
                sql = new StringBuilder();
                sql.Append("Update CONTROLCENTRE.CONTACT_RELATION");
                sql.AppendFormat(" set Priority = {0}, NOK_YN = '{1}', Keyholder_YN = '{2}'", this._priority.ToString(), this._nok_yn, this._keyholder_yn);
                sql.AppendFormat(", Location_Ref = {0}, Resident_Ref = {1}", this._location_def.ToString(), this._resident_def.ToString());
                sql.AppendFormat(" where RELATION_DEF = {0}", this._relation_def.ToString());
                process = "Updated";

                err_message = string.Format("Successfully updated relation_def {0} for subscriber_responder_Id {1}, subscriber id {2}. QUERY: {3}"
                    , this._relation_def.ToString(), this._subscriberresponder_id, this._subscriber_id, sql.ToString());
            }
            else
            {
                //insert the relation
                sql = new StringBuilder();
                sql.Append("insert into CONTROLCENTRE.CONTACT_RELATION(ENTITY_TYPE,LOCATION_REF,RESIDENT_REF,CONTACT_REF,PRIORITY,NOK_YN,KEYHOLDER_YN)");
                sql.AppendFormat("values({0}, {1}, {2}, {3}, {4}, '{5}', '{6}')"
                    , this._entity_type.ToString()
                    , this._location_def.ToString()
                    , this._resident_def.ToString()
                    , this._contact_def.ToString()
                    , this._priority.ToString()
                    , this._nok_yn
                    , this._keyholder_yn);

                err_message = string.Format("Successfully inserted new relation_def for subscriber_responder_Id {0}, subscriber id {1}. Statement: {2}. QUERY: {3}"
                    , this._subscriberresponder_id,this._subscriber_id,sql.ToString(), sql.ToString());
            }

            try
            {

                this._pncProvider.ExecuteNonSPQuery(sql.ToString(), null);

                this.EntityMessage(err_message, "INFO");
            }
            catch (Exception ex)
            {
                err_message = string.Format("Failed to {0} Contact_Relation {1} for Subscriber_Responder_Id {2}. ERROR: {3}"
                    , process
                    , this._relation_def.ToString()
                    , this._subscriberresponder_id.ToString()
                    , ex.Message);

                this._serviceRepository.Save_Transaction_Record(this._group_record, "SubscriberResponder", sql.ToString(), err_message);
                this.EntityMessage(err_message, "ERROR");
                processMapping = false;
            }

            if (processMapping)
            {
                try
                {
                    this._sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@process_type", "KSHEMA"));
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber_responder", this._subscriberresponder_id.ToString()));
                    this._sqlParms.Add(SQLParam.GetParam("@location_ref", this._location_def));
                    this._sqlParms.Add(SQLParam.GetParam("@resident_ref", this._resident_def));
                    this._sqlParms.Add(SQLParam.GetParam("@contact_ref", this._contact_def));
                    this._sqlParms.Add(SQLParam.GetParam("@type_ref", this._type_ref.ToString()));
                    this._sqlParms.Add(SQLParam.GetParam("@priority", this._priority.ToString()));
                    this._stageProvider.ExecuteNonQuery("SYNC_Process_ContactRelation", this._sqlParms);

                    this.EntityMessage(string.Format("Succesfully processed contact_relation mapping for SubscriberResponder_Id {0}.", this._subscriberresponder_id.ToString()), "INFO");
                }
                catch (Exception ex)
                {
                    err_message = string.Format("Failed to process contact_relation mapping for SubscriberResponder_Id {0}. ERROR: {1}", this._subscriberresponder_id.ToString(), ex.Message);
                    this.EntityMessage(err_message, "ERROR");
                }
            }
        }

        void Process_Contact_Notes()
        {
            string err_message;
            DataTable dtNotes = null;
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_responder", this._subscriberresponder_id));
                dtNotes = this._stageProvider.ExecuteDataSet("SYNC_SubscriberResponder_Notes", this._sqlParms).Tables[0];
            }
            catch { }

            if (dtNotes != null)
            {
                foreach (DataRow drNote in dtNotes.Rows)
                {
                    string process_type = drNote["process_type"].ToString();
                    string title = drNote["note_title"].ToString();
                    string text = string.Empty;
                    if (!string.IsNullOrEmpty(drNote["note_text"].ToString()))
                        text = drNote["note_text"].ToString().Replace("'", "''");

                    int notes_def = drNote["notes_def"].Parse<int>();
                    int access_ref = drNote["access_ref"].Parse<int>();

                    switch (process_type.ToLower())
                    {
                        case "insert":

                            if (!string.IsNullOrEmpty(text))
                            {
                                this._sql = new StringBuilder();
                                this._sql.Append("insert into CONTROLCENTRE.NOTES(ACCESS_REF,ENTITY_TYPE,ENTITY_REF,TITLE,TEXT)");
                                this._sql.AppendFormat("values({0},{1},{2},'{3}','{4}')"
                                    , access_ref.ToString()
                                    , "8"
                                    , this._contact_def
                                    , title
                                    , text);
                            }
                            break;


                        case "update":
                            if (!string.IsNullOrEmpty(text))
                            {
                                this._sql = new StringBuilder();
                                this._sql.Append("UPDATE CONTROLCENTRE.NOTES");
                                this._sql.AppendFormat(" SET TEXT = '{0}'", text);
                                this._sql.AppendFormat(" WHERE NOTES_DEF = {0}", notes_def.ToString());

                            }

                            break;
                    }

                    if (this._sql != null)
                    {
                        try
                        {
                            this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                            this.EntityMessage(string.Format("Successfully processed a contact note {0}: {1}: {2}, subscriber responder Id {3}"
                                , process_type.ToLower()
                                ,title
                                ,text
                                ,this._subscriberresponder_id.ToString()),"INFO");
                        }
                        catch (Exception ex)
                        {
                            err_message = string.Format("Failed to process a contact note {0}: {1}: {2} subscriber responder Id {3}.ERROR: {4}"
                                , process_type.ToLower()
                                , title
                                , text
                                , this._subscriberresponder_id.ToString()
                                ,ex.Message);

                            this.EntityMessage(err_message,"ERROR");
                            this._serviceRepository.Save_Transaction_Record(this._group_record, "SubscriberResponder", this._sql.ToString(), err_message);
                        }
                    }
                }
            }
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow(string process)
        {
            bool isSuccess = this.Map_Properties();
            if (isSuccess)
            {
                isSuccess = this.Check_Agency_Mapping(this._agency_id);
                if (isSuccess)
                {
                    string process_type = "UPDATE";

                    //determine if its an insert or update
                    if (this._contact_def == -1 && !this._is_deleted)
                        process_type = "INSERT";

                    if (this._is_deleted && this._contact_def != -1)
                        process_type = "DELETE";

                    isSuccess = this.Save_Contact(process_type);
                    if (isSuccess)
                    {
                        // check contact relation
                        if (!this._is_deleted)
                        {
                            this.Process_Contact_Relation();

                            this.Process_Contact_Notes();
                        }
                    }
                }
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class Contact : EntityBase, IEntity
    {
        #region " Member Variables "
        
        
        int _NVIOrder;
        int _priority;
        int _relation_def;

        string _responder_name;
        string _home_phone;
        string _work_phone;
        string _cell_phone;

        bool _hasKey = false;
        bool _NVI = false;

        #endregion

        #region " Constructor "
        public Contact(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();
        }
        #endregion

        #region " Private Methods "
        private bool MapContactFields()
        {
            bool isMapped = true;

            _locationDef = _dataToMapp["location_ref"].Parse<int>();
            _residentDef = _dataToMapp["resident_ref"].Parse<int>();
            _contact_def = _dataToMapp["contact_def"].Parse<int>();
            _contact_type_ref = _dataToMapp["contact_type_ref"].Parse<int>();
            _relation_def = _dataToMapp["relation_def"].Parse<int>();
            _entity_type = _dataToMapp["entity_type"].Parse<int>();
            _responder_name = _dataToMapp["ResponderName"].ToString();

            if(_dataToMapp["HasKey"].ToString() == "Y")
                _hasKey = true;

            _NVIOrder = _dataToMapp["NVIOrder"].Parse<int>();

            if (_dataToMapp["NVI"].ToString() == "Y")
                _NVI = true;

            _priority = _dataToMapp["Priority"].Parse<int>();

            try
            {
                _home_phone = _dataToMapp["HomePhone"].ToString();
            }
            catch { }

            try
            {
               _work_phone = _dataToMapp["WorkPhone"].ToString();
            }
            catch { }

            try
            {
               _cell_phone  = _dataToMapp["CellPhone"].ToString();
            }
            catch { }

            return isMapped;
        }

        private bool Map_Responder_Contact()
        {
            string msg;
            bool isMatch = false;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@contact_def", _contact_def));
                // DataRow drAgency = _stageProvider.GetData("SYNC_Map_Responder_Contact", _sqlParms).Rows[0];

                DataTable dtMapContact = _stageProvider.GetData("SYNC_Map_Responder_Contact", _sqlParms);
                if (dtMapContact.Rows.Count > 0)
                {
                    DataRow dr = dtMapContact.Rows[0];
                    _SubscriberResponder_ID = dr["SubscriberResponder_ID"].Parse<int>();
                    _Subscriber_ID = dr["Subscriber_ID"].ToString();

                    isMatch = true;
                    EntityMessage(string.Format("SubscriberResponder Id {0} found for contact_def {1}", _SubscriberResponder_ID.ToString(), _contact_def.ToString()), "INFO");
                }
                else
                    EntityMessage(string.Format("No mapping found for contact_def {0}. Row Count: {1}", _contact_def.ToString(),dtMapContact.Rows.Count.ToString()), "INFO");
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Map_Responder_Contact had a fatal error mapping contact_def {0}. ERROR: {1}", _contact_def.ToString(),ex.Message), "ERROR");
            }


            return isMatch;
        }

        private void UpdateSubscriberResponder()
        {
            string msg = string.Empty;
            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE [SubscriberResponder] "); 
	        sql.Append("SET [CallOrder] = @call_order ");
	        sql.Append(",[CellPhone] = '@cell_phone' ");
	        sql.Append(",[HasKey] = '@has_key' ");
	        sql.Append(",[HomePhone] = '@home_phone' ");
	        sql.Append(",[NVI] = '@nvi' ");
	        sql.Append(",[NVIOrder] = @order ");
	        sql.Append(",[ResponderName] = '@repsonder_name' ");
	        sql.Append(",[WorkPhone] = '@work_phone' ");
            sql.Append("WHERE [SubscriberResponder_ID] = @subresp_id");
            try
            {
                //_sqlParms = new List<SQLParam>();
                //_sqlParms.Add(SQLParam.GetParam("@subresp_id	", _SubscriberResponder_ID));
                //_sqlParms.Add(SQLParam.GetParam("@call_order", _priority ));
                //_sqlParms.Add(SQLParam.GetParam("@has_key", _hasKey));
                //_sqlParms.Add(SQLParam.GetParam("@home_phone", _home_phone));
                //_sqlParms.Add(SQLParam.GetParam("@work_phone", _work_phone));
                //_sqlParms.Add(SQLParam.GetParam("@cell_phone", _cell_phone));
                //_sqlParms.Add(SQLParam.GetParam("@nvi", _NVI));
                //_sqlParms.Add(SQLParam.GetParam("@nvi_order", _NVIOrder));
                //_sqlParms.Add(SQLParam.GetParam("@repsonder_name", _responder_name));

                //_stageProvider.ExecuteNonQuery("SYNC_Update_SubscriberResponder ", _sqlParms);

                sql.Replace("@call_order", _priority.ToString());
                sql.Replace("@cell_phone", _cell_phone);
                sql.Replace("@has_key", _hasKey ? "Y":"N");
                sql.Replace("@home_phone", _home_phone);
                sql.Replace("@nvi", _NVI ? "Y" : "N");
                sql.Replace("@order", _NVIOrder.ToString());
                sql.Replace("@repsonder_name", _responder_name);
                sql.Replace("@work_phone", _work_phone);
                sql.Replace("@subresp_id", _SubscriberResponder_ID.ToString());
                _kshemaProvider.ExecuteNonSPQuery(sql.ToString(), null);

                EntityMessage(string.Format("Subscriber responder {0} updated", _responder_name),"INFO");
            }
            catch (Exception ex)
            {
                string err_message = string.Format("Failed to update subscriber responder Id {0}. ERROR: {1}", _SubscriberResponder_ID.ToString(), ex.Message);
                EntityMessage(err_message, "ERROR");
                this.Log_Transaction("PNC", "CONTACT", sql.ToString(), err_message);
            }
        }

        private void Insert_Temp_Repsonder_Q()
        {
            string msg = string.Empty;
            try
            {
                if (_Subscriber_ID != "-1")
                {
                    _sqlParms = new List<SQLParam>();
                    _sqlParms.Add(SQLParam.GetParam("@subscriber_id", _Subscriber_ID));
                    _sqlParms.Add(SQLParam.GetParam("@responder_name", _responder_name));
                    _sqlParms.Add(SQLParam.GetParam("@work_phone", _work_phone));
                    _sqlParms.Add(SQLParam.GetParam("@cell_phone", _cell_phone));
                    _sqlParms.Add(SQLParam.GetParam("@home_phone", _home_phone));
                    _sqlParms.Add(SQLParam.GetParam("@has_key", _hasKey == false ? "N" : "Y"));
                    _sqlParms.Add(SQLParam.GetParam("@nvi_order", _NVIOrder));
                    _sqlParms.Add(SQLParam.GetParam("@nvi", _NVI == false ? "N" : "Y"));
                    _sqlParms.Add(SQLParam.GetParam("@relation_type", _type));
                    _sqlParms.Add(SQLParam.GetParam("@relation_code", _Code));
                    _sqlParms.Add(SQLParam.GetParam("@contact_def", _contact_def));
                    _sqlParms.Add(SQLParam.GetParam("@contact_type_def", _contact_type_ref));
                    _sqlParms.Add(SQLParam.GetParam("@priority", _priority));
                    _sqlParms.Add(SQLParam.GetParam("@location_def", _locationDef));
                    _sqlParms.Add(SQLParam.GetParam("@resident_def", _residentDef));
                    _sqlParms.Add(SQLParam.GetParam("@entity_type", _entity_type));

                    _stageProvider.ExecuteNonQuery("SYNC_Insert_Responder_Q", _sqlParms);

                    EntityMessage(string.Format("New contact record inserted into TEMP_PNC_CONTACT_SSIS_Q. Subscriber Id: {0}", _Subscriber_ID),"INFO");
                }
                else
                    throw new Exception("The process failed to map a Subscriber Id for the contact.");
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Failed to insert contact_def {0} for subscriber id {1} into TEMP_PNC_CONTACT_SSIS_Q. ERROR:{2}", _contact_def.ToString(), _Subscriber_ID, ex.Message),"ERROR");
            }
        }

        private bool Map_Contact_Type()
        {
            bool isMapped = true;
            string msg;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@contact_type_ref", _contact_type_ref));
                DataRow drAgency = _stageProvider.GetData("SYNC_Map_Contact_Type", _sqlParms).Rows[0];
                _Code = drAgency["Code"].ToString();
                _type = drAgency["Type"].ToString();

                EntityMessage(string.Format("Map_Contact_Type: Successfully mapped Type {0} and Code {1} for contact_def {2}.", _type, _Code, _contact_def.ToString()),"INFO");
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Map_Contact_Type: Failed to map contact type for contact_def {0}. Error: {1}", _contact_def.ToString(), ex.Message), "ERROR");
            }

            return isMapped;
        }

        private bool Map_Resident_SubscriberId()
        {
            bool isMatch = false;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@subscriber", -1));
                _sqlParms.Add(SQLParam.GetParam("@locdef", _locationDef));
                _sqlParms.Add(SQLParam.GetParam("@resdef", _residentDef));
                _Subscriber_ID = _stageProvider.ExecuteScalar<string>("SYNC_Map_Subscriber_Resident", _sqlParms);

                isMatch = true;
            }
            catch(Exception ex)
            {
                EntityMessage(string.Format("Map_Resident_SubscriberId had a fatal error. ERROR: {0}", ex.Message), "ERROR");
            }

            return isMatch;
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow()
        {
            bool processContact = this.MapContactFields();

            if (processContact)
            {
                if (this.Map_Responder_Contact())
                {
                    if (this.Map_Contact_Type())
                        this.UpdateSubscriberResponder();
                }
                else  
                {
                    if (this.Map_Resident_SubscriberId())
                        if (this.Map_Contact_Type())
                            this.Insert_Temp_Repsonder_Q();                    
                }
            }

        }

        public override void ProcessDataDelete()
        {
            bool isMapped = true;
            // map the fields
            try
            {
                _contact_def = this._dataToMapp["contact_def"].Parse<int>();
                //_contact_type_ref = this._dataToMapp["contact_type_ref"].Parse<int>();
                _residentDef = this._dataToMapp["resident_ref"].Parse<int>();
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Failed to map contact_def {0} in Contact.ProcessDataDelete. ERROR: {1}", _contact_def.ToString(), ex.Message),"ERROR");
                isMapped = false; 
            }

            if (isMapped)
            {
                try
                {
                    _sqlParms = new List<SQLParam>();
                    _sqlParms.Add(SQLParam.GetParam("@contact_def", _contact_def));
                    _sqlParms.Add(SQLParam.GetParam("@resident_def", _residentDef));
                    _stageProvider.ExecuteNonQuery("SYNC_Process_Responder_Delete", _sqlParms);
                    this.EntityMessage(string.Format("Processed responder delete for contact_def {0}", _contact_def),"INFO");
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Unable to process responder delete for contact_def: {0}. ERROR: {1}", _contact_def.ToString(),ex.Message),"ERROR");

                }
            }
        }

        public static void Process_Contact_Temp_Queue()
        {
            //exec TEMP_CONTACT_INSERT_SUBSCRIBER_RESPONDER

            IDataProvider pncStage = null;

            try
            {
                pncStage = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
                pncStage.ExecuteNonQuery("TEMP_CONTACT_INSERT_SUBSCRIBER_RESPONDER", null);
            }
            catch { }

            
            if (pncStage != null)
                pncStage = null;

        }

        public static void Update_Responder_Call_Order()
        {
            IDataProvider pncStage = null;

            try
            {
                pncStage = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
                pncStage.ExecuteNonQuery("SYNC_Update_Responder_Call_Order", null);
            }
            catch { }

            if (pncStage != null)
                pncStage = null;
        }

        public static void Delete_Duplicate_Contacts()
        {
            IDataProvider pnc = null;
            try
            {
                StringBuilder sql = new StringBuilder();
                pnc = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfiguration.PNC_Connection);

                sql.Append("with dupes(Contact_ref, relation_def,rn) as");
                sql.Append(" (select Contact_ref, relation_def, rn=row_number() over (partition by RESIDENT_REF, contact_ref order by relation_def)");
                sql.Append(" from  CONTROLCENTRE.CONTACT_RELATION  where ENTITY_TYPE = 2 AND location_ref in");
                sql.Append(" (select location_ref from STANDARD.RESIDENT GROUP BY LOCATION_REF HAVING COUNT(RESIDENT_DEF) =1))");
                sql.Append(" select ROW_NUMBER() OVER ( ORDER BY relation_def),Contact_ref, relation_def, rn from dupes where rn > 1");

                DataTable dtDelete = pnc.ExecuteDataSetQuery(sql.ToString(), null).Tables[0];
                Parallel.ForEach(dtDelete.AsEnumerable(), dr =>
                    {
                        IDataProvider tmppnc = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfiguration.PNC_Connection);
                        try
                        {
                            StringBuilder tmpsql = new StringBuilder();
                            tmpsql.AppendFormat("delete from CONTROLCENTRE.CONTACT_RELATION where relation_def = {0}", dr["relation_def"].ToString());
                            tmppnc.ExecuteNonSPQuery(tmpsql.ToString(), null);
                        }
                        catch { }
                        finally
                        {
                            if (tmppnc != null)
                                tmppnc = null;
                        }
                    });
                
            }
            catch { }
            finally
            {
                if (pnc != null)
                    pnc = null;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace Kshema_PNC.BL.Entity
{
    class Subscriber : EntityBase, IEntity
    {
        #region " Member Variables "        

        long _location_def = -1;
        long _resident_def = -1;

        bool _new_home_created = false;
        bool _store_unit_id = false;

        string _subscriber_id;
        string _masterreference;
        string _first_name = string.Empty;
        string _middle_initial = string.Empty;
        string _last_name = string.Empty;
        string _agency_id = string.Empty;        
        string _address_1 = string.Empty;
        string _address_2 = string.Empty;
        string _zip = string.Empty;
        string _city = string.Empty;
        string _state = string.Empty;
        string _phone = string.Empty;
        string _other_phone = string.Empty;
        string _status = string.Empty;
        string _language = string.Empty;
        string _gender = string.Empty;
        string _sub_type = string.Empty;
        string _ipAddress = "0";
        string _nearest_intersection = string.Empty;
        string _equip_id;
        string _hex_unit_id = string.Empty;

        //string _notes_YN = "Y";
        string _ssn_restrict_YN = "Y";
        string _primary_YN = "N";
        string _resident_note_YN = "Y";
        string _epec_note_YN = "Y";
        string _deleted = "N";
        string _second_user = "N";

        string _dob = null;
        string _install_date = null;
        string _activation_date = null;
        string _termination_date = null;
        string _activation_def = null;

        long _authority_ref;        
        long _colour_ref;
        long _equip_model_ref;

        enum ATTRIBUTE_TYPE
        {
            STATUS
            ,LANGUAGE
            ,GENDER
            ,SUBSCRIBER_TYPE
            ,RF_CODE
        }
        #endregion

        #region " Constructor "
        public Subscriber(DataRow dr, string group_record)
        {
            this._dataToProcess = dr;
            this._group_record = group_record;

            this.InitializeProviders();
        }
        #endregion

        #region " Private Methods "
        string Check_Link_Unlink()
        {
            string next_step = "PROCESS";

            // get the mapping for this sub
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_def));
                this._sqlParms.Add(SQLParam.GetParam("@master_reference", this._masterreference));
                next_step = this._stageProvider.ExecuteScalar<string>("SYNC_Check_Sub_Link_Unlink", this._sqlParms);
            }
            catch { }

            return next_step;
        }

        bool Map_Properties()
        {
            bool isSuccess = true;

            try
            {
                this._subscriber_id = _dataToProcess["Subscriber_Id"].ToString();
                this._masterreference = _dataToProcess["MasterReference"].ToString();
                this._agency_id = _dataToProcess["Agency_Id"].ToString();
                this._first_name = _dataToProcess["FirstName"].ToString().Replace("'","");
                this._last_name = _dataToProcess["LastName"].ToString().Replace("'", "");
                this._status = _dataToProcess["Status"].ToString();
                this._gender = _dataToProcess["Gender"].ToString();
                this._sub_type = _dataToProcess["Type"].ToString().Trim();

                if (!string.IsNullOrEmpty(_dataToProcess["IPAddress"].ToString()))
                    this._ipAddress = _dataToProcess["IPAddress"].ToString(); 

                if(!string.IsNullOrEmpty(_dataToProcess["Language_ID"].ToString()))
                    this._language = _dataToProcess["Language_ID"].ToString();                


                if(!string.IsNullOrEmpty(_dataToProcess["MiddleInitial"].ToString()))
                    this._middle_initial = _dataToProcess["MiddleInitial"].ToString();

                this._address_1 = _dataToProcess["Address1"].ToString().Replace("'", "");

                if (!string.IsNullOrEmpty(_dataToProcess["Address2"].ToString()))
                    this._address_2 = _dataToProcess["Address2"].ToString().Replace("'", "");

                this._city = _dataToProcess["City"].ToString();
                this._state = _dataToProcess["State"].ToString();
                this._zip = _dataToProcess["ZIP"].ToString();
                this._phone = _dataToProcess["Phone"].ToString();
                this._other_phone = _dataToProcess["OTHER_PHONE"].ToString();

                if(!string.IsNullOrEmpty(_dataToProcess["NearIntersection"].ToString()))
                    this._nearest_intersection = _dataToProcess["NearIntersection"].ToString().Replace("'", "");

                //check for a record in the map_joined_subscriber_authority table for joined subs in 
                // different hoomes.
                long auth_ref = this.Check_Map_Subscriber_Authority();
                if (auth_ref == -1)
                {
                    if (!string.IsNullOrEmpty(_dataToProcess["AUTHORITY_REF"].ToString()))
                        this._authority_ref = _dataToProcess["AUTHORITY_REF"].Parse<int>();
                }
                else
                    this._authority_ref = auth_ref;

                if (!string.IsNullOrEmpty(_dataToProcess["Equip_Id"].ToString()))
                {
                    this._equip_id = _dataToProcess["Equip_Id"].ToString();
                }
                else
                    this._equip_id = "-1";

                this._colour_ref = _dataToProcess["COLOUR_REF"].Parse<int>();

                #region v 1.1
                if (!string.IsNullOrEmpty(_dataToProcess["Equip_Model_Ref"].ToString()))
                    this._equip_model_ref = _dataToProcess["Equip_Model_Ref"].Parse<int>();
                else
                    this._equip_model_ref = 150;
                #endregion


                if (!string.IsNullOrEmpty(_dataToProcess["DOB"].ToString()))
                    this._dob = _dataToProcess["DOB"].Parse<DateTime>().ToString("yyyy-MM-dd");

                if (!string.IsNullOrEmpty(_dataToProcess["InstallDate"].ToString()))
                    this._install_date = _dataToProcess["InstallDate"].Parse<DateTime>().ToString("yyyy-MM-dd HH:mm:ss");

                if (!string.IsNullOrEmpty(_dataToProcess["TerminationDate"].ToString()))
                    this._termination_date = _dataToProcess["TerminationDate"].Parse<DateTime>().ToString("yyyy-MM-dd HH:mm:ss");

                if (!string.IsNullOrEmpty(_dataToProcess["ActivationDate"].ToString()))
                    this._activation_date = _dataToProcess["ActivationDate"].Parse<DateTime>().ToString("yyyy-MM-dd HH:mm:ss");

                this._ssn_restrict_YN = _dataToProcess["SS_RESTRICT_YN"].ToString();
                this._primary_YN = _dataToProcess["PRIMARY_YN"].ToString();
                this._resident_note_YN = _dataToProcess["Resident_Notes_YN"].ToString();
                this._epec_note_YN = _dataToProcess["Epec_Notes_YN"].ToString();
                this._deleted = _dataToProcess["Deleted"].ToString();
                this._second_user = _dataToProcess["SecondUser"].ToString();

                if (!string.IsNullOrEmpty(_dataToProcess["HEX_Unit_Id"].ToString()))
                {
                    this._store_unit_id = true;
                    this._hex_unit_id = _dataToProcess["HEX_Unit_Id"].ToString();
                }
                else
                    this._hex_unit_id = "-1";
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.EntityMessage(string.Format("Failed to map properties for Subscriber {0}. Error: {1}", _dataToProcess["Subscriber_Id"].ToString(), ex.Message),"ERROR");
            }

            return isSuccess;
        }

        //bool Map_Subscriber_Location()
        //{
        //    bool isSuccess = true;
        //    try
        //    {
        //        this._sqlParms = new List<SQLParam>();
        //        this._sqlParms.Add(SQLParam.GetParam("@locdef", this._location_def));
        //        this._sqlParms.Add(SQLParam.GetParam("@resdef", this._resident_def));
        //        this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));

        //        DataRow dr = _stageProvider.ExecuteDataSet("SYNC_Map_Subscriber_Location", this._sqlParms).Tables[0].Rows[0];
        //        this._location_def = dr["Location_Def"].Parse<int>();      

        //    }
        //    catch (Exception ex)
        //    {
        //        isSuccess = false;
        //        EntityMessage(string.Format("Failed to map subsciber to location for subscriber Id {0}. Error: {1}", this._subscriber_id, ex.Message), "ERROR");
        //    }

        //    return isSuccess;
        //}
        //bool Map_Subscriber_Resident()
        //{
        //    bool isSuccess = true;

        //    try
        //    {
        //        this._sqlParms = new List<SQLParam>();
        //        this._sqlParms.Add(SQLParam.GetParam("@locdef", -1));
        //        this._sqlParms.Add(SQLParam.GetParam("@resdef", -1));
        //        this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));

        //        DataRow dr = _stageProvider.ExecuteDataSet("SYNC_Map_Subscriber_Resident", this._sqlParms).Tables[0].Rows[0];
        //        this._resident_def = dr["Resident_Def"].Parse<int>();
        //    }
        //    catch (Exception ex)
        //    {
        //        isSuccess = false;
        //        EntityMessage(string.Format("Failed to map subsciber to location for subscriber Id {0}. Error: {1}", this._subscriber_id, ex.Message), "ERROR");
        //    }

        //    return isSuccess;
        //}

        long Check_Map_Subscriber_Authority()
        {
            long authority_ref = -1;

            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                DataTable dtAuth = this._stageProvider.ExecuteDataSet("SYNC_Get_Sub_Mapped_Authority", this._sqlParms).Tables[0];
                if (dtAuth.Rows.Count > 0)
                {
                    authority_ref = dtAuth.Rows[0]["AUTHORITY_REF"].Parse<long>();
                }
            }
            catch (Exception ex)
            {

            }
            return authority_ref;
        }
        long Get_MasterReference_Authority()
        {
            long authority_ref = -1;

            try
            {
                this._sql = new StringBuilder();
                this._sql.Append("select a.AUTHORITY_REF from dbo.ks_Subscriber s with (nolock) ");
                this._sql.Append("left join [dbo].[MAP_AGENCY_AUTHORITY] a with (nolock) on s.[Agency_ID] = a.AGENCY_ID ");
                this._sql.AppendFormat("where s.Subscriber_Id = '{0}'", this._masterreference);
                DataTable dtAuth = this._stageProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0];

                if (dtAuth.Rows.Count > 0)
                {
                    authority_ref = dtAuth.Rows[0]["AUTHORITY_REF"].Parse<long>();
                }
            }
            catch (Exception ex)
            {

            }
            return authority_ref;
        }
        bool Map_Subscriber_Location()
        {
            bool isMapped = true;
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@locdef", -1));
                this._sqlParms.Add(SQLParam.GetParam("@resdef", -1));
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));

                DataRow dr = _stageProvider.ExecuteDataSet("SYNC_Map_Subscriber_Location", this._sqlParms).Tables[0].Rows[0];
                this._location_def = dr["Location_Def"].Parse<int>();

                if (this._location_def != -1)
                {                    
                    EntityMessage(string.Format("Location_def {0} found for Subscriber_Id {1}", this._location_def, this._subscriber_id), "INFO");
                }
                else
                {
                    isMapped = false;
                    EntityMessage(string.Format("No Location_def found for Subscriber Id {0}", this._subscriber_id), "INFO");
                }

            }
            catch (Exception ex)
            {
                isMapped = false;
                EntityMessage(string.Format("Failed to map subsciber to location for subscriber Id {0}. Error: {1}", this._subscriber_id, ex.Message), "ERROR");
            }

            return isMapped;
        }
        bool Map_Subscriber_Resident()
        {
            bool isMapped = true;
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@locdef", -1));
                this._sqlParms.Add(SQLParam.GetParam("@resdef", -1));
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));

                DataRow dr = _stageProvider.ExecuteDataSet("SYNC_Map_Subscriber_Resident", this._sqlParms).Tables[0].Rows[0];
                
                this._resident_def = dr["Resident_Def"].Parse<int>();

                if (this._location_def != -1 && this._resident_def != -1)
                {
                    EntityMessage(string.Format("Location_def {0} and Resident_Def {1} found for Subscriber_Id {2}", this._location_def, this._resident_def, this._subscriber_id), "INFO");
                }
                else
                {
                    isMapped = false;
                    EntityMessage(string.Format("No Location_def or Resident_def found for Subscriber Id {0}", this._subscriber_id), "INFO");
                }

            }
            catch (Exception ex)
            {
                isMapped = false;
                EntityMessage(string.Format("Failed to map subsciber to location for subscriber Id {0}. Error: {1}", this._subscriber_id, ex.Message), "ERROR");
            }

            return isMapped;
        }
        

        bool Save_Epec_Location(string process_type)
        {
            string err_message;
            bool isSuccess = true;           

            if (process_type == "INSERT")
            {               

                // if its the primary create the home
                if (_primary_YN == "Y")
                {
                    #region New
                    // add new logic to check for a MSL mapping prior to inserting
                    bool hasMapping = this.IsSubscriberMapped(this._subscriber_id);
                    if (!hasMapping)
                    {

                        try
                        {
                            this._sql = new StringBuilder();

                            this._sql.AppendLine("insert into STANDARD.EPEC_LOCATION(");
                            this._sql.Append(" SCHEME_REF");
                            this._sql.Append(",SCHEME_ID");
                            this._sql.Append(",EQUIP_ID");
                            this._sql.Append(",EQUIP_MODEL_REF");
                            this._sql.Append(",EQUIP_PHONE");
                            this._sql.Append(",ADDRESS_STREET");
                            this._sql.Append(",ADDRESS_AREA");
                            this._sql.Append(",ADDRESS_TOWN");
                            this._sql.Append(",ADDRESS_COUNTY");
                            this._sql.Append(",ADDRESS_POSTCODE");
                            this._sql.Append(",OTHER_PHONE");
                            this._sql.Append(",NOTES_YN");
                            this._sql.Append(",ACCESS_REF");
                            this._sql.Append(",AUTHORITY_REF");
                            this._sql.Append(",SS_RESTRICT_YN");
                            this._sql.Append(",COLOUR_REF");

                            if (!string.IsNullOrEmpty(this._activation_date))
                                this._sql.Append(",ACTIVATION_DATE");

                            if (!string.IsNullOrEmpty(this._termination_date))
                                this._sql.Append(",TERMINATION_DATE");

                            if (!string.IsNullOrEmpty(this._install_date))
                                this._sql.Append(",INSTALLATION_DATE");

                            this._sql.Append(")");
                            this._sql.AppendFormat("values(	0,-1,{0}", this._equip_id.ToString());
                            this._sql.AppendFormat(",{0}", this._equip_model_ref.ToString());
                            this._sql.AppendFormat(",'{0}'", this._phone);
                            this._sql.AppendFormat(",'{0}','{1}','{2}','{3}','{4}'", this._address_1, this._address_2, this._city, this._state, this._zip);
                            this._sql.AppendFormat(",'{0}','{1}'", this._subscriber_id, this._epec_note_YN);
                            this._sql.AppendFormat(",100000061,{0},'Y',{1}", this._authority_ref.ToString(), this._colour_ref.ToString());

                            if (!string.IsNullOrEmpty(this._activation_date))
                                this._sql.AppendFormat(",cast('{0}' as date)", this._activation_date);

                            if (!string.IsNullOrEmpty(this._termination_date))
                                this._sql.AppendFormat(",cast('{0}' as date)", this._termination_date);

                            if (!string.IsNullOrEmpty(this._install_date))
                                this._sql.AppendFormat(",cast('{0}' as date)", this._install_date);

                            this._sql.Append(")");
                            this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                            this.EntityMessage(string.Format("Successfully inserted epec_location for subscriber Id {0}. STATEMENT: {1}", this._subscriber_id, this._sql.ToString()), "INFO");
                        }
                        catch (Exception ex)
                        {
                            err_message = string.Format("Failed to insert epec_location for subscriber Id {0}.SQL: {1}. ERROR: {2}", this._subscriber_id, this._sql.ToString(), ex.Message);
                            this.EntityMessage(err_message, "ERROR");
                            this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                            isSuccess = false;
                        }

                        if (isSuccess)
                        {
                            try
                            {
                                this._sql = new StringBuilder();
                                this._sql.AppendFormat(" select top 1 cast(location_def as bigint) as [Location_def] from STANDARD.EPEC_LOCATION where other_phone = '{0}';", this._subscriber_id);
                                this._location_def = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0]["Location_def"].Parse<int>();
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                err_message = string.Format("Failed to get epec_location.location_def for subscriber Id {0} on insert.SQL: {1}. ERROR: {2}", this._subscriber_id, this._sql.ToString(), ex.Message);
                                this.EntityMessage(err_message, "ERROR");
                                this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                                isSuccess = false;
                            }

                            try
                            {

                                this._sqlParms = new List<SQLParam>();
                                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                                this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def));
                                this._sqlParms.Add(SQLParam.GetParam("@master_reference", this._masterreference));
                                this._stageProvider.ExecuteNonQuery("SYNC_Save_Subscriber_Location", this._sqlParms);
                                this.EntityMessage(string.Format("Successfully created subscriber location mapping for subscriber Id {0}, location_def {1}.", this._subscriber_id, this._location_def), "INFO");
                                isSuccess = true;
                            }
                            catch (Exception ex)
                            {
                                err_message = string.Format("Failed to create subscriber location mapping for subscriber Id {0}, location_def {1}. ERROR: {2}", this._subscriber_id, this._location_def, ex.Message);
                                this.EntityMessage(err_message, "ERROR");
                                this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                                isSuccess = false;
                            }

                            try
                            {
                                this._sql = new StringBuilder();

                                if (!string.IsNullOrEmpty(this._other_phone) & _sub_type != "MSD")
                                {
                                    this._sql.AppendFormat(" update STANDARD.EPEC_LOCATION set OTHER_PHONE = '{0}' where location_Def = {1};", this._other_phone, this._location_def.ToString());
                                }
                                else
                                    this._sql.AppendFormat(" update STANDARD.EPEC_LOCATION set OTHER_PHONE = '' where location_Def = {0};", this._location_def.ToString());

                                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                            }
                            catch (Exception ex)
                            {
                                err_message = string.Format("Failed to update other_phone on insert to epec_location for subscriber Id {0}.SQL: {1}. ERROR: {2}", this._subscriber_id, this._sql.ToString(), ex.Message);
                                this.EntityMessage(err_message, "INFO");
                            }
                        }
                    }
                    else
                        isSuccess = false;
                    #endregion
                }
                else
                {
                    // this sub is going into a home where there exists a primary, and the contacts for the primary
                    // 1.find the mappings for the secondary based on the master reference
                    // 2. set the location to the location_def of the master
                    // 3.create the mappings
                    // 4.Since the home already has a primary, we can remove the scondaries contacts from the log., then 
                   
                    try
                    {
                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._masterreference));
                        DataRow drMaster = _stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Mapping", this._sqlParms).Tables[0].Rows[0];

                        this._location_def = drMaster["LOCATION_REF"].Parse<long>();

                        // create the mappings                      
                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                        this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def.ToString()));
                        this._sqlParms.Add(SQLParam.GetParam("@master_ref", this._masterreference));
                        this._stageProvider.ExecuteNonQuery("SYNC_Save_Secondary_Subscriber_Mapping", this._sqlParms);


                        this._sql = new StringBuilder();
                        this._sql.Append("Update [ks_ETLChangeLog] set is_processed = 1");
                        this._sql.Append(" where table_name = 'SubscriberResponder'");
                        this._sql.Append(" and is_processed = 0");
                        this._sql.AppendFormat(" and table_id in(Select SubscriberResponder_ID from [ks_SubscriberResponder] where Subscriber_Id = '{0}')",this._subscriber_id);
                        this._stageProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                    }
                    catch (Exception ex)
                    {
                        isSuccess = false;
                        this.EntityMessage(string.Format("Failed to get master reference mappings for subscriber Id {0}. ERROR: {1}", this._subscriber_id, ex.Message), "ERROR");
                    }
                }
            }
            else
            {
                //TODO: UPDATE THE EQUIPMENT MODEL - PNC EQUIPMENT TABLE

                this._sql = new StringBuilder();

                this._sql.Append("UPDATE EPEC_LOCATION ");
                this._sql.AppendFormat("SET ADDRESS_STREET ='{0}', NOTES_YN = '{1}' ", this._address_1,this._epec_note_YN);
                this._sql.AppendFormat(",ADDRESS_AREA = '{0}', ADDRESS_TOWN = '{1}', ADDRESS_COUNTY = '{2}', ADDRESS_POSTCODE = '{3}' ",this._address_2,this._city,this._state,this._zip);
                this._sql.AppendFormat(",EQUIP_PHONE = '{0}', AUTHORITY_REF = {1}, COLOUR_REF = {2}", this._phone, this._authority_ref.ToString(), this._colour_ref.ToString());

                if (_sub_type != "MSD")
                {
                    if (!string.IsNullOrEmpty(this._other_phone))
                        this._sql.AppendFormat(", OTHER_PHONE = '{0}'", this._other_phone);
                }

                if (!string.IsNullOrEmpty(this._termination_date))
                    this._sql.AppendFormat(",TERMINATION_DATE = cast('{0}' as date)", this._termination_date);
                else
                    this._sql.Append(",TERMINATION_DATE = NULL");

                if (!string.IsNullOrEmpty(this._activation_date))
                    this._sql.AppendFormat(",ACTIVATION_DATE = cast('{0}' as date)", this._activation_date);
                else
                    this._sql.Append(",ACTIVATION_DATE =NULL");

                if (!string.IsNullOrEmpty(this._install_date))
                    this._sql.AppendFormat(",INSTALLATION_DATE = cast('{0}' as date)", this._install_date);
                else
                    this._sql.Append(",INSTALLATION_DATE = NULL");

                this.EntityMessage(string.Format("Sub Type {0} for Subscriber {1}", this._sub_type, this._subscriber_id), "INFO");

                #region V 1.1
                if(this._equip_id != "-1")
                    this._sql.AppendFormat(",EQUIP_ID = {0}", this._equip_id.ToString());

                if(this._equip_model_ref != 150)
                    this._sql.AppendFormat(",EQUIP_MODEL_REF = {0}", this._equip_model_ref.ToString());

                //store the hex id in the EPEC_LOCATION.Property_ref column
                if(this._store_unit_id)
                    this._sql.AppendFormat(", WEBCAM_IP_ADDRESS = '{0}'", this._hex_unit_id);
                #endregion

                this._sql.AppendFormat(" WHERE LOCATION_DEF = {0}",this._location_def.ToString());

                try
                {
                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);                    
                    this.EntityMessage(string.Format("Successfully update location_def {0} for subscriber Id {1}. {2}", this._location_def.ToString(), this._subscriber_id,this._sql.ToString()),"INFO");
                }
                catch (Exception ex)
                {
                    err_message = string.Format("Failed to update location_def {0} for subscriber Id {1}.ERROR: {2}. SQL: {3}", 
                        this._location_def.ToString(),this._subscriber_id, ex.Message,this._sql.ToString());

                    this.EntityMessage(err_message,"ERROR");
                    this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                }
            }

            return isSuccess;
        }
        bool Save_Resident(string process_type)
        {
            string err_message;
            bool isSuccess = true;


            if (process_type == "INSERT")
            {
                #region INSERT
                try
                {
                    int priority = 10000;
                    if (this._primary_YN == "N")
                        priority = 10001;

                    this._sql = new StringBuilder();
                    this._sql.Append("insert into STANDARD.RESIDENT(");
                    this._sql.Append("location_ref,authority_ref,first_name,last_name,date_of_birth,notes_yn,access_ref,priority,primary_yn,health_care_no");

                    if (!string.IsNullOrEmpty(this._phone))
                        this._sql.Append(",phone_1,s_phone_1)");
                    else
                        this._sql.Append(")");


                    this._sql.AppendFormat("values({0},{1},'{2}','{3}'", this._location_def.ToString(),this._authority_ref, this._first_name, this._last_name);

                    if (!string.IsNullOrEmpty(this._dob))
                        this._sql.AppendFormat(",cast('{0}' as date)", this._dob);
                    else
                        this._sql.Append(",NULL");

                    this._sql.AppendFormat(",'{0}',100000006,{1},'{2}','{3}'", this._resident_note_YN, priority.ToString(), this._primary_YN, this._subscriber_id);

                    if (!string.IsNullOrEmpty(this._phone))
                        this._sql.AppendFormat(",'{0}','{1}')", this._phone, this._phone);
                    else
                        this._sql.Append(")");

                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                    this.EntityMessage(string.Format("Successfully inserted resident for subscriber Id {0}", this._subscriber_id), "INFO");
                }
                catch (Exception ex)
                {
                    err_message = string.Format("Failed to insert resident for subscriber Id {0}.SQL: {1}. ERROR: {2}", this._subscriber_id, this._sql.ToString(), ex.Message);
                    this.EntityMessage(err_message, "ERROR");
                    this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                    isSuccess = false;
                }

                if (isSuccess)
                {
                    try
                    {
                        this._sql = new StringBuilder();
                        this._sql.AppendFormat(" select top 1 cast(resident_def as bigint) as [Resident_Def] from STANDARD.RESIDENT where health_care_no = '{0}';", this._subscriber_id);
                        this._resident_def = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0]["Resident_Def"].Parse<int>();

                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                        this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def));
                        this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_def));
                        this._stageProvider.ExecuteNonQuery("SYNC_Save_Subscriber_Resident", this._sqlParms);

                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        err_message = string.Format("Failed to get resident_def for subscriber Id {0} on new insert.SQL: {1}. ERROR: {2}", this._subscriber_id, this._sql.ToString(), ex.Message);
                        this.EntityMessage(err_message, "ERROR");
                        this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                        isSuccess = false;
                    }

                    try
                    {
                        this._sql = new StringBuilder();
                        if (!string.IsNullOrEmpty(this._other_phone))
                        {
                            this._sql.AppendFormat(" update STANDARD.RESIDENT set PHONE_2 = '{0}' where resident_def = {1};", this._other_phone, this._resident_def);

                            this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                        }                                             

                    }
                    catch (Exception ex)
                    {
                        err_message = string.Format("Failed to update resident.phone_2 on insert for subscriber Id {0}.SQL: {1}. ERROR: {2}", this._subscriber_id, this._sql.ToString(), ex.Message);
                        this.EntityMessage(err_message, "INFO");
                    }

                    //11/20/2015: Update the MOBILE_SOL_USER_YN field if sub type = MSD
                    try
                    {
                        this._sql = new StringBuilder();
                        if (this._sub_type == "MSD")
                            this._sql.AppendFormat(" update STANDARD.RESIDENT set MOBILE_SOL_USER_YN = 'Y' where resident_def = {0};",  this._resident_def);
                        else
                            this._sql.AppendFormat(" update STANDARD.RESIDENT set MOBILE_SOL_USER_YN = 'N' where resident_def = {0};", this._resident_def);

                        this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                    }
                    catch (Exception ex)
                    {
                        err_message = string.Format("Failed to update resident.MOBILE_SOL_USER_YN on insert for subscriber Id {0}.SQL: {1}. ERROR: {2}", this._subscriber_id, this._sql.ToString(), ex.Message);
                        this.EntityMessage(err_message, "INFO");
                    }
                }
                #endregion
            }
            else
            {
                #region UPDATE
                //UPDATE
                int priority = 10000;

                if (this._primary_YN == "N")
                {
                    priority = 10001;

                    try
                    {
                        this._sql = new StringBuilder();
                        this._sql.AppendFormat("SELECT MAX(PRIORITY) + 1 as [MAX_PRIORITY] from STANDARD.RESIDENT where Location_ref ={0} and resident_def <> {1}", this._location_def.ToString(), this._resident_def.ToString());
                        priority = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0]["MAX_PRIORITY"].Parse<int>();

                        this.EntityMessage(string.Format("Successfully set priority {0} for resident {1} in location ref {2}."
                            , priority.ToString(), this._resident_def.ToString(), this._location_def.ToString()), "INFO");
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to get max priority for secondary resident {0} in location ref {1}. ERROR: {2}"
                            , this._resident_def.ToString(), this._location_def.ToString(), ex.Message), "INFO");
                    }
                }
                else
                {

                    // before this happens need to check if there is already a primary in the home
                    try
                    {
                        this._sql = new StringBuilder();
                        this._sql.AppendFormat("if exists(select resident_def from STANDARD.RESIDENT where Location_ref ={0} and priority = 10000 and resident_def <> {1}) ",this._location_def.ToString(),this._resident_def.ToString());
                        this._sql.AppendLine("begin ");
                        this._sql.AppendFormat("SELECT Resident_def,PRIORITY from STANDARD.RESIDENT where Location_ref ={0} order by PRIORITY desc", this._location_def.ToString(), this._resident_def.ToString());
                        this._sql.AppendLine(" end");
                        DataTable dtPriorityCheck = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0];
                        if (dtPriorityCheck.Rows.Count > 0)
                        {
                            foreach (DataRow drPriorityCheck in dtPriorityCheck.Rows)
                            {
                                int tmpPriority = drPriorityCheck["PRIORITY"].Parse<int>();
                                int tmpResident = drPriorityCheck["Resident_def"].Parse<int>();
                                tmpPriority = tmpPriority + 1;

                                this._sql = new StringBuilder();
                                this._sql.AppendFormat("UPDATE STANDARD.RESIDENT SET PRIORITY = {0} WHERE RESIDENT_DEF = {1}", tmpPriority.ToString(), tmpResident.ToString());
                                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                                this.EntityMessage(string.Format("Successfully set priority {0} for resident {1} in location ref {2}."
                                    , tmpPriority.ToString(), tmpResident.ToString(), this._location_def.ToString()), "INFO");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to set priority for existing residents in location ref {0}. ERROR: {1}", this._location_def.ToString(), ex.Message), "INFO");
                    }
                }

                try
                {
                    this._sql = new StringBuilder();
                    this._sql.Append("UPDATE STANDARD.RESIDENT");
                    this._sql.AppendFormat(" SET FIRST_NAME = '{0}', LAST_NAME = '{1}'", this._first_name, this._last_name);

                    if (!string.IsNullOrEmpty(this._dob))
                        this._sql.AppendFormat(" ,DATE_OF_BIRTH = cast('{0}' as date)", this._dob);

                    this._sql.AppendFormat(" ,NOTES_YN = '{0}'", this._resident_note_YN);
                    this._sql.AppendFormat(" ,PRIMARY_YN = '{0}', PHONE_1 = '{1}', PRIORITY = {2}", this._primary_YN, this._phone, priority.ToString());
                    this._sql.AppendFormat(" , AUTHORITY_REF = {0}, LOCATION_REF = {1}", this._authority_ref.ToString(), this._location_def.ToString());


                    if (this._sub_type == "MSD")
                    {
                        this._sql.Append(" ,MOBILE_SOL_USER_YN = 'Y'");

                        if(!string.IsNullOrEmpty(_other_phone))
                            this._sql.AppendFormat(" , PHONE_2 = '{0}'", this._other_phone);
                    }
                    else
                        this._sql.Append(" ,MOBILE_SOL_USER_YN = 'N'");

                    this._sql.AppendFormat(" WHERE RESIDENT_DEF = {0}", this._resident_def.ToString());
                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                    this.EntityMessage(string.Format("Successfully update resident {0} for susbcriber {1}. SQL: {2}", this._resident_def.ToString(), this._subscriber_id,this._sql.ToString()), "INFO");
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    err_message = string.Format("Failed to update Resident for subscriber Id {0}. ERROR: {1}. SQL:{2}", process_type.ToLower(), this._subscriber_id, ex.Message);
                    this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                    this.EntityMessage(err_message, "ERROR");
                }
                #endregion
            }

            #region OLD 
    //        try
    //            {
    //                if (process_type == "INSERT")
    //                {
    //                this._sqlParms = new List<SQLParam>();
    //                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
    //                this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def));
    //                this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_def));
    //                this._sqlParms.Add(SQLParam.GetParam("@first_name", this._first_name));
    //                this._sqlParms.Add(SQLParam.GetParam("@last_name", this._last_name));
    //                this._sqlParms.Add(SQLParam.GetParam("@dob", this._dob));
    //                this._sqlParms.Add(SQLParam.GetParam("@phone", this._phone));
    //                this._sqlParms.Add(SQLParam.GetParam("@authority_ref", this._authority_ref));
    //                this._sqlParms.Add(SQLParam.GetParam("@is_primary", this._primary_YN));
    //                this._sqlParms.Add(SQLParam.GetParam("@notes_yn", this._resident_note_YN));

    //                DataRow drResident = _stageProvider.ExecuteDataSet("SYNC_Save_Resident", this._sqlParms).Tables[0].Rows[0];
    //                this._resident_def = drResident["Resident_Def"].Parse<long>();
    //            }
    //            else
    //            {
    //                /*
    //                 * 		
    //UPDATE PNC_KSHEMA..STANDARD.RESIDENT
    //    SET FIRST_NAME = @first_name
    //    , LAST_NAME = @last_name
    //    ,DATE_OF_BIRTH = @dob
    //    , NOTES_YN = @notes_yn
    //    , PRIMARY_YN = @is_primary
    //    , PHONE_1 = @phone
    //    , PRIORITY = @priority
    //    , AUTHORITY_REF = @authority_ref
    //    , Location_ref = @location_def
    //    WHERE RESIDENT_DEF = @resident_def
    //                 * 
    //                 * 
    //                 * */

    //                int priority = 10000;

    //                if (this._primary_YN == "N")
    //                {
    //                    priority = 10001;

    //                    try
    //                    {
    //                        this._sql = new StringBuilder();
    //                        this._sql.AppendFormat("SELECT MAX(PRIORITY) + 1 as [MAX_PRIORITY] from STANDARD.RESIDENT where Location_ref ={0} and resident_def <> {1}", this._location_def.ToString(),this._resident_def.ToString());
    //                        priority = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0]["MAX_PRIORITY"].Parse<int>();

    //                        this.EntityMessage(string.Format("Successfully set priority {0} for resident {1} in location ref {2}."
    //                            , priority.ToString(),this._resident_def.ToString(), this._location_def.ToString()), "INFO");
    //                    }
    //                    catch(Exception ex)
    //                    {
    //                        this.EntityMessage(string.Format("Failed to get max priority for secondary resident {0} in location ref {1}. ERROR: {2}"
    //                            , this._resident_def.ToString(),this._location_def.ToString(), ex.Message), "INFO");
    //                    }
    //                }
    //                else
    //                {

    //                    // before this happens need to check if there is already a primary in the home
    //                    try
    //                    {
    //                        this._sql = new StringBuilder();
    //                        this._sql.AppendFormat("SELECT Resident_def,PRIORITY from STANDARD.RESIDENT where Location_ref ={0} order by PRIORITY desc", this._location_def.ToString(),this._resident_def.ToString());
    //                        DataTable dtPriorityCheck = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0];
    //                        if (dtPriorityCheck.Rows.Count > 0)
    //                        {
    //                            foreach (DataRow drPriorityCheck in dtPriorityCheck.Rows)
    //                            {
    //                                int tmpPriority = drPriorityCheck["PRIORITY"].Parse<int>();
    //                                int tmpResident = drPriorityCheck["Resident_def"].Parse<int>();
    //                                tmpPriority = tmpPriority + 1;

    //                                this._sql = new StringBuilder();
    //                                this._sql.AppendFormat("UPDATE STANDARD.RESIDENT SET PRIORITY = {0} WHERE RESIDENT_DEF = {1}", tmpPriority.ToString(), tmpResident.ToString());
    //                                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
    //                            }
    //                        }
    //                    }
    //                    catch(Exception ex)
    //                    {
    //                        this.EntityMessage(string.Format("Failed to set priority for existing residents in location ref {0}. ERROR: {1}",this._location_def.ToString(),ex.Message),"INFO");
    //                    }
    //                }
                       
    //                this._sql = new StringBuilder();
    //                this._sql.Append("UPDATE STANDARD.RESIDENT");
    //                this._sql.AppendFormat(" SET FIRST_NAME = '{0}', LAST_NAME = '{1}'", this._first_name, this._last_name);

    //                if(!string.IsNullOrEmpty(this._dob))
    //                    this._sql.AppendFormat(" ,DATE_OF_BIRTH = cast('{0}' as date)", this._dob);

    //                this._sql.AppendFormat(" ,NOTES_YN = '{0}'", this._resident_note_YN); 
    //                this._sql.AppendFormat(" ,PRIMARY_YN = '{0}', PHONE_1 = '{1}', PRIORITY = {2}", this._primary_YN, this._phone, priority.ToString());
    //                this._sql.AppendFormat(" , AUTHORITY_REF = {0}, LOCATION_REF = {1}", this._authority_ref.ToString(), this._location_def.ToString());
    //                this._sql.AppendFormat(" WHERE RESIDENT_DEF = {0}", this._resident_def.ToString());
    //                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
    //            }
    //            isSuccess = true;

    //            this.EntityMessage(string.Format("Successfully {0} resident_def {1} in location_def {2} for subscriber Id {3}. SQL: {4}", process_type.ToLower(),this._resident_def, this._location_def.ToString(), this._subscriber_id,this._sql.ToString()), "INFO");
    //        }
    //        catch (Exception ex)
    //        {
    //            isSuccess = false;
    //            err_message = string.Format("Failed to {0} Resident for subscriber Id {1}. ERROR: {2}", process_type.ToLower(),this._subscriber_id, ex.Message);
    //            this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
    //            this.EntityMessage(err_message, "ERROR");
    //        }
            #endregion

            if (process_type == "UPDATE" && isSuccess)
            {
                //*************************
                //  update the keywords
                //*************************
                bool updateKeyword = this.Delete_Keyword_Relation();

                if (updateKeyword)
                {
                    DataTable dtKeywords = null;
                    try
                    {
                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                        dtKeywords = this._stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Keyword_Relation", this._sqlParms).Tables[0];
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to get keywords for Subscriber {0}.ERROR: {1}", this._subscriber_id, ex.Message), "ERROR");
                    }

                    if (dtKeywords != null)
                    {
                        foreach (DataRow drKeyword in dtKeywords.Rows)
                        {
                            StringBuilder keyword_text = new StringBuilder(drKeyword["KEYWORD_TEXT"].ToString());
                            keyword_text.Replace("'", "");
                            try
                            {
                                this._sql = new StringBuilder();
                                this._sql.Append("insert into CONTROLCENTRE.KEYWORD_RELATION(RESIDENT_REF,KEYWORD_NO,KEYWORD_REF,KEYWORD_TEXT)");
                                this._sql.AppendFormat("values({0},{1},{2},'{3}')"
                                    , this._resident_def.ToString()
                                    , drKeyword["KEYWORD_NO"].ToString()
                                    , drKeyword["KEYWORD_DEF"].ToString()
                                    , keyword_text.ToString());
                                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);


                            }
                            catch (Exception ex)
                            {
                                err_message = string.Format("Failed to insert keyword {0}: {1} for resident_def {2}. ERROR: {3}"
                                    , drKeyword["KEYWORD_DEF"].ToString()
                                    , drKeyword["KEYWORD_TEXT"].ToString()
                                    , this._resident_def.ToString()
                                    , ex.Message);

                                this.EntityMessage(err_message, "ERROR");
                                this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                            }
                        }
                    }
                }

            }
            return isSuccess;
        }
        bool Create_Temp_PNC_Equip()
        {
            bool can_process = true;

            try
            {
                this._equip_id = Equipment.Get_Temp_Unit_Id(this._subscriber_id, this._authority_ref).ToString();
                this.EntityMessage(string.Format("Generated unit Id {0} for subscriber {1}",this._equip_id.ToString(), this._subscriber_id), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to generate a temporary unit id for Subscriber {0}. ERROR: {1}", this._subscriber_id, ex.Message), "ERROR");
                can_process = false;
            }
            return can_process;
        }
        bool Generate_Equip_Id(string command)
        {
            /*
             * PNC sub types do not exist in the padding table and will 
             * set the padding = -1
             * 
             * Non PNC sub types now all have a padding so the padding will
             * never come back as 0
             * */
            bool can_process = true;
            long unit_id = 0;
            long padding = 0;
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@sync_type", "KSHEMA"));
                this._sqlParms.Add(SQLParam.GetParam("@type", this._sub_type));
                DataTable dtunit_padding = this._stageProvider.ExecuteDataSet("SYNC_Get_Unit_Padding", this._sqlParms).Tables[0];
                padding = dtunit_padding.Rows[0]["Padding"].Parse<long>();

            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to get the padding for subscriber type {0} for subscriber {1}. ERROR: {2}", this._sub_type, this._subscriber_id, ex.Message), "INFO");
            }

            #region OLD logic 
            //if (padding == 0)
            //{
            //    if (command == "INSERT")
            //    {
            //        can_process = this.Create_Temp_PNC_Equip();
            //    }
            //    else
            //    {
            //        if(this._equip_id == "-1")
            //        {
            //            Equipment equip = new Equipment();
            //            equip.OnEntityMessage += equip_OnEntityMessage;
            //            this._equip_id = equip.GetPNCEquipId(this._subscriber_id,this._location_def);
            //        }
            //    }
            //}
            //else
            //{
            //    if (this._status.ToLower() == "active")
            //    {
            //        if (command == "INSERT")
            //        {
            //            if (this._equip_id == "-1")
            //                can_process = this.Create_Temp_PNC_Equip();
            //        }

            //        if (command == "UPDATE")
            //        {
            //            if (this._hex_unit_id != "-1")
            //            {
            //                try
            //                {
            //                    int convert_unit_id = Convert.ToInt32(this._hex_unit_id, 16);
            //                    unit_id = padding + convert_unit_id;
            //                    this._equip_id = unit_id.ToString();

            //                    this.EntityMessage(string.Format("Converting {0} to {1} for subscriber {2}", this._hex_unit_id, this._equip_id, this._subscriber_id), "INFO");
            //                }
            //                catch (Exception ex)
            //                {
            //                    this.EntityMessage(string.Format("Failed to convert {0} for subscriber {1}. ERROR: {2}", this._equip_id, this._subscriber_id, ex.Message), "INFO");
            //                }
            //            }
            //            else
            //            {
            //                Equipment equip = new Equipment();
            //                equip.OnEntityMessage += equip_OnEntityMessage;
            //                this._equip_id = equip.GetPNCEquipId(this._subscriber_id, this._location_def);
            //            }
            //        }                    
            //    }
            //    else
            //    {
            //        if (command == "INSERT")
            //        {
            //            can_process = this.Create_Temp_PNC_Equip();
            //        }

            //        if (command == "UPDATE")
            //        {
            //            // check if it not alread removed on the PNC side
            //            bool can_change = false;
            //            string t_unit = string.Empty;
            //            this._sql = new StringBuilder();

            //            this._sql.AppendFormat("select colour_ref, Equip_Id from EPEC_LOCATION where location_def = {0}", this._location_def.ToString());
            //            try
            //            {
            //                DataRow dr = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0];

            //                long colour = dr["colour_ref"].Parse<long>();
            //                t_unit = dr["Equip_Id"].ToString();

            //                if (colour != 100000012 & this._status.ToLower() =="removed")
            //                    can_change = true;
            //            }
            //            catch(Exception ex)
            //            {
            //                this.EntityMessage(string.Format("Failed to get the current PNC status for location_def {0}, Subscriber {1}. ERROR:{2}", 
            //                    this._location_def.ToString(), this._subscriber_id,ex.Message), "INFO");
            //            }


            //            var type = new[] { "(PNC)", "MSD", "Cellular" };
            //            if (can_change && !type.Contains(this._sub_type))
            //            {
            //                this._equip_id = string.Format("{0}{1}", DateTime.Now.ToString("yyMMdd"), this._subscriber_id);

            //                this.EntityMessage(string.Format("Changing the current PNC equip_id to {0} for location_def {1}, Subscriber {2}.",
            //                    this._equip_id, this._location_def.ToString(), this._subscriber_id), "INFO");
            //            }
            //            else
            //            {
            //                this.EntityMessage(string.Format("The current home is already removed/cancelled for location_def {0}, Subscriber {1}.",
            //                    this._location_def.ToString(), this._subscriber_id), "INFO");
            //                this._equip_id = t_unit;
            //            }
            //        }
            //    }
            //}
            #endregion
            //see JIRA-39
            if (this._status.ToLower() == "active")
            {
                if (command == "INSERT")
                {
                    if (this._equip_id == "-1")
                        can_process = this.Create_Temp_PNC_Equip();
                    else
                    {
                        if (this._hex_unit_id != "-1")
                        {
                            try
                            {
                                int convert_unit_id = Convert.ToInt32(this._hex_unit_id, 16);
                                unit_id = padding + convert_unit_id;
                                this._equip_id = unit_id.ToString();

                                this.EntityMessage(string.Format("Converting {0} to {1} for subscriber {2}", this._hex_unit_id, this._equip_id, this._subscriber_id), "INFO");
                            }
                            catch (Exception ex)
                            {
                                this.EntityMessage(string.Format("Failed to convert {0} for subscriber {1}. ERROR: {2}", this._equip_id, this._subscriber_id, ex.Message), "INFO");
                            }
                        }
                    }
                }

                if (command == "UPDATE")
                {
                    if (this._hex_unit_id != "-1")
                    {
                        try
                        {
                            int convert_unit_id = Convert.ToInt32(this._hex_unit_id, 16);
                            unit_id = padding + convert_unit_id;
                            this._equip_id = unit_id.ToString();

                            this.EntityMessage(string.Format("Converting {0} to {1} for subscriber {2}", this._hex_unit_id, this._equip_id, this._subscriber_id), "INFO");
                        }
                        catch (Exception ex)
                        {
                            this.EntityMessage(string.Format("Failed to convert {0} for subscriber {1}. ERROR: {2}", this._equip_id, this._subscriber_id, ex.Message), "INFO");
                        }
                    }
                    else
                    {
                        Equipment equip = new Equipment();
                        equip.OnEntityMessage += equip_OnEntityMessage;
                        this._equip_id = equip.GetPNCEquipId(this._subscriber_id, this._location_def);
                    }
                }
            }
            else
            {
                if (command == "INSERT")
                {
                    can_process = this.Create_Temp_PNC_Equip();
                }

                if (command == "UPDATE")
                {
                    // check if it not alread removed on the PNC side
                    bool can_change = false;
                    string t_unit = string.Empty;
                    this._sql = new StringBuilder();

                    this._sql.AppendFormat("select colour_ref, Equip_Id from EPEC_LOCATION where location_def = {0}", this._location_def.ToString());
                    try
                    {
                        DataRow dr = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0];

                        long colour = dr["colour_ref"].Parse<long>();
                        t_unit = dr["Equip_Id"].ToString();

                        if (colour != 100000012 & this._status.ToLower() == "removed")
                            can_change = true;
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to get the current PNC status for location_def {0}, Subscriber {1}. ERROR:{2}",
                            this._location_def.ToString(), this._subscriber_id, ex.Message), "INFO");
                    }


                    var type = new[] { "(PNC)", "MSD", "Cellular" };
                    if (can_change && !type.Contains(this._sub_type))
                    {
                        this._equip_id = string.Format("{0}{1}", DateTime.Now.ToString("yyMMdd"), this._subscriber_id);

                        this.EntityMessage(string.Format("Changing the current PNC equip_id to {0} for location_def {1}, Subscriber {2}.",
                            this._equip_id, this._location_def.ToString(), this._subscriber_id), "INFO");
                    }
                    else
                    {
                        this.EntityMessage(string.Format("The current home is already removed/cancelled for location_def {0}, Subscriber {1}.",
                            this._location_def.ToString(), this._subscriber_id), "INFO");
                        this._equip_id = t_unit;
                    }
                }
            }
            return can_process;
        }
        bool Delete_Keyword_Relation()
        {
            bool updateKeyword = false;
            try
            {
                // first delete the relations
                this._sql = new StringBuilder();
                this._sql.AppendFormat("DELETE FROM CONTROLCENTRE.KEYWORD_RELATION WHERE RESIDENT_REF = {0}", this._resident_def.ToString());
                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                this.EntityMessage(string.Format("Sucessfully deleted keyword_relation for resident_def {0}.", this._resident_def.ToString()), "INFO");
                updateKeyword = true;
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to delete keyword_relation for resident_def {0}.ERROR: {1}", this._resident_def.ToString(), ex.Message), "INFO");
            }

            return updateKeyword;
        }

        void Get_Home_To_Move_Subscriber()
        {
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                DataTable dtLocation = this._stageProvider.ExecuteDataSet("SYNC_Get_Home_To_Move_Subscriber", this._sqlParms).Tables[0];
                if (dtLocation.Rows.Count > 0)
                {
                    this._location_def = dtLocation.Rows[0]["Location_Def"].Parse<int>();

                    if (this._location_def != -1)
                        this._authority_ref = dtLocation.Rows[0]["Authority_Ref"].Parse<int>();
                }
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Fatal error in Subscriber.Get_Home_To_Move_Subscriber, Subscriber Id: {0}. ERROR: {1}", this._subscriber_id, ex.Message), "ERROR");
            }
        }
        void Delete_Subscriber_Contacts()
        {
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_def));
                this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def));
                this._stageProvider.ExecuteNonQuery("SYNC_Delete_Subscriber_Contacts", this._sqlParms);
                this.EntityMessage(string.Format("Delete_Subscriber_Contacts successfull deleted contacts for subscriber {0} during the unlink.", this._subscriber_id), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Delete_Subscriber_Contacts failed to delete contacts for subscriber {0} during the unlink. Location_def: {1}. Resident_def: {2}. ERROR: {3}"
                    ,this._subscriber_id
                    ,this._location_def.ToString()
                    ,this._resident_def.ToString()
                    ,ex.Message), "ERROR");
            }
        }       
        void Save_Resident_Attribute(ATTRIBUTE_TYPE type)
        {
            int entity_type = 5;
            long entity_ref = this._resident_def;
            string entity_name = "Resident";
            long attribute_category = -1;
            long attr_choice_ref = -1;

            if (type == ATTRIBUTE_TYPE.STATUS)
            {
                attribute_category = 100000128;
                attr_choice_ref = 100000294;

                switch (this._status.ToLower())
                {
                    case "pending install":
                        attr_choice_ref = 100000310;
                        break;

                    case "pending removal":
                        attr_choice_ref = 100000292;
                        break;

                    case "removed":
                        attr_choice_ref = 100000293;
                        break;

                    case "temporary hold":
                        attr_choice_ref = 100000297;
                        break;
                }
            }

            if (type == ATTRIBUTE_TYPE.LANGUAGE)
            {
                try
                {
                    this._sql = new StringBuilder();
                    this._sql.AppendFormat("select ATTR_CHOICE_REF, ATTR_CATEGORY_REF from [dbo].[MAP_LANGUAGE_CODE_ATTR] a with(nolock) where Code = '{0}';", this._language);
                    DataRow drLanguage = this._stageProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0];
                    attr_choice_ref = drLanguage["ATTR_CHOICE_REF"].Parse<int>();
                    attribute_category = drLanguage["ATTR_CATEGORY_REF"].Parse<int>();
                }
                catch(Exception ex)
                { 
                    this.EntityMessage(string.Format("No language mapping found for {0} on subscriber {1}. ERROR: {2}", this._language, this._subscriber_id,ex.Message), "INFO"); 
                }
            }

            if (type == ATTRIBUTE_TYPE.GENDER)
            {
                /*
                 * Date: 11/13/2015
                 * Change: Attribute category ref changed from 4 to 306
                 * */
                attribute_category = 306;
                if (this._gender == "M")
                    attr_choice_ref = 100000050;
                else
                    attr_choice_ref = 100000051;
            }


            if (type == ATTRIBUTE_TYPE.SUBSCRIBER_TYPE)
            {
                entity_type = 2;
                attribute_category = 100000003;
                entity_ref = this._location_def;
                entity_name = "Epec_Location";

                try
                {
                    //1. get the sub type mapping 
                    this._sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber_type", this._sub_type));
                    attr_choice_ref = this._stageProvider.ExecuteScalar<long>("SYNC_Get_Subscriber_Type_Lookup", this._sqlParms);
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Failed to set the Subscriber type {0} attribute for Subscriber {1}. ERROR: {2}"
                            ,this._sub_type, this._subscriber_id, ex.Message), "INFO");
                }
            }

            if (type == ATTRIBUTE_TYPE.RF_CODE)
            {
                string frequency = "-1";
                entity_type = 5;
                attribute_category = 100000117;

                try
                {
                    // get the rf code for the sub
                    this._sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                    frequency = this._stageProvider.ExecuteScalar<string>("SYNC_Get_Subscriber_RFCode", this._sqlParms);
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("No Frequency found for subscriber {0}. ERROR: {1}"
                            , this._subscriber_id, ex.Message), "INFO");
                }

                if (frequency != "-1")
                {
                    try
                    {
                        this._sql = new StringBuilder();
                        this._sql.AppendFormat("SELECT ATTR_CHOICE_DEF FROM CONTROLCENTRE.ATTR_CHOICE where attr_category_ref = {0} and TEXT = '{1}'", attribute_category.ToString(), frequency.ToUpper());
                        attr_choice_ref = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0].Rows[0]["ATTR_CHOICE_DEF"].Parse<long>();
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("No Attr_Choice_Ref found for frequency {0}, subscriber {1}. ERROR: {2}"
                            ,frequency, this._subscriber_id, ex.Message), "INFO");
                    }
                }                
            }


            if (attribute_category != -1 && attr_choice_ref != -1)
            {
                bool change_mapping = false;

                try
                {
                    this._sql = new StringBuilder();
                    this._sql.AppendFormat("if exists (select Attr_choice_ref from STANDARD.ATTR_DEF with (nolock) where entity_type = {0} and entity_ref = {1} and attr_category_ref = {2})"
                        , entity_type.ToString(),entity_ref.ToString(), attribute_category.ToString());
                    this._sql.AppendLine("begin ");
                    this._sql.AppendFormat("update STANDARD.ATTR_DEF set attr_choice_ref = {0} where entity_type ={1} and entity_ref = {2} and attr_category_ref = {3} "
                        , attr_choice_ref.ToString(),entity_type.ToString(), entity_ref.ToString(), attribute_category.ToString());
                    this._sql.AppendLine("end");
                    this._sql.AppendLine("else");
                    this._sql.AppendLine("begin");
                    this._sql.AppendLine("insert into STANDARD.ATTR_DEF(entity_type,entity_ref,attr_choice_ref,attr_category_ref,authority_ref)");
                    this._sql.AppendFormat("values({0},{1},{2},{3},{4}) "
                        , entity_type.ToString(),entity_ref.ToString(), attr_choice_ref.ToString(), attribute_category.ToString(), this._authority_ref.ToString());
                    this._sql.AppendLine("end");

                    this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                    this.EntityMessage(string.Format("Inserted attribute {0}. {1} {2} for Subscriber {3}", type.ToString(), entity_name, entity_ref.ToString(), this._subscriber_id), "INFO");
                    change_mapping = true;
                }
                catch (Exception ex)
                {
                    string err_message = string.Format("Failed to insert attribute {0}. {1} {2} for subscriber Id {3}. ERROR: {4}", type.ToString(),entity_name, entity_ref.ToString(), this._subscriber_id, ex.Message);
                    this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                    this.EntityMessage(err_message, "INFO");
                }

                if (change_mapping && entity_name == "Resident")
                {
                    this._sqlParms = new List<SQLParam>();
                    try
                    {
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                        this._sqlParms.Add(SQLParam.GetParam("@resident_def", entity_ref.ToString()));
                        this._sqlParms.Add(SQLParam.GetParam("@attribute_category", attribute_category));
                        this._sqlParms.Add(SQLParam.GetParam("@attr_choice_ref", attr_choice_ref));
                        this._sqlParms.Add(SQLParam.GetParam("@authority_ref", this._authority_ref));
                        _stageProvider.ExecuteNonQuery("SYNC_Save_Resident_Attribute_Mapping", _sqlParms);

                        this.EntityMessage(string.Format("Successfully inserted attribute {0} mapping. Resident_def {1} for Subscriber {2}", type.ToString(), this._resident_def.ToString(), this._subscriber_id), "INFO");
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to insert attribute {0} mapping. Resident_def {1} for Subscriber {2}. ERROR: {3}"
                            , type.ToString(), this._resident_def.ToString(), this._subscriber_id, ex.Message), "INFO");
                    }
                }
            }
        }
        void Remove_Resident_Attribute(ATTRIBUTE_TYPE type)
        {
            int entity_type = 5;
            long entity_ref = this._resident_def;
            string entity_name = "Resident";
            long attribute_category = -1;

            if (type == ATTRIBUTE_TYPE.SUBSCRIBER_TYPE)
            {
                entity_type = 2;
                attribute_category = 100000003;
                entity_ref = this._location_def;
                entity_name = "Epec_Location";
            }

             if (attribute_category != -1)
             {
                 try
                 {
                     this._sql = new StringBuilder();
                     this._sql.AppendFormat("delete from ATTR_DEF where entity_type ={0} and entity_ref = {1} and attr_category_ref = {2}", entity_type, entity_ref, attribute_category);
                     this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                     this.EntityMessage(string.Format("Deleted attribute {0}. {1} {2} for Subscriber {3}", type.ToString(), entity_name, entity_ref.ToString(), this._subscriber_id), "INFO");                     
                 }
                 catch (Exception ex)
                 {
                     string err_message = string.Format("Failed to delete attribute {0}. {1} {2} for subscriber Id {3}. ERROR: {4}", type.ToString(), entity_name, entity_ref.ToString(), this._subscriber_id, ex.Message);
                     this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                     this.EntityMessage(err_message, "INFO");
                 }
             }
        }


        void Process_Nearest_Intersection()
        {
            // get nearest intersection
            string err_message;            
            bool can_process = true;
            int notes_def = -1;
            if (!string.IsNullOrEmpty(this._nearest_intersection))
            {
                try
                {
                    this._sql = new StringBuilder();
                    this._sql.AppendFormat("select coalesce(notes_def,-1) as [Nearest_Intersection] from CONTROLCENTRE.NOTES with (nolock) where entity_type = 2 and entity_ref = {0} and TITLE = 'Directions To Home'"
                        , this._location_def);

                    DataTable dtNearIntersection = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0];
                    if (dtNearIntersection.Rows.Count > 0)
                    {
                        DataRow drNearIntersection = dtNearIntersection.Rows[0];
                        notes_def = drNearIntersection["Nearest_Intersection"].Parse<int>();
                    }
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Error getting nearest intersection note for subscriber Id {0}, location_def {1}. ERROR: {2}", this._subscriber_id, this._location_def.ToString(), ex.Message), "INFO");

                    can_process = false;
                }

                if (can_process)
                {
                    string title = "Directions To Home";
                    int access_ref = 100000061;
                    string process_type = "Insert";

                    if (notes_def == -1)
                    {
                        this._sql = new StringBuilder();
                        this._sql.Append("insert into CONTROLCENTRE.NOTES(ACCESS_REF,ENTITY_TYPE,ENTITY_REF,TITLE,TEXT)");
                        this._sql.AppendFormat("values({0},{1},{2},'{3}','{4}')"
                                , access_ref.ToString()
                                , "2"
                                , this._location_def.ToString()
                                , title
                                , this._nearest_intersection);
                    }
                    else
                    {
                        this._sql = new StringBuilder();
                        this._sql.Append("UPDATE CONTROLCENTRE.NOTES");
                        this._sql.AppendFormat(" SET TEXT = '{0}'", this._nearest_intersection);
                        this._sql.AppendFormat(" WHERE NOTES_DEF = {0}", notes_def.ToString());
                        process_type = "Update";
                    }

                    try
                    {
                        this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                        this.EntityMessage(string.Format("Successfully processed an nearest intersection {0} for location_def {1}, subscriber Id {2}"
                            , process_type.ToLower(), this._location_def.ToString(), this._subscriber_id), "INFO");
                    }
                    catch (Exception ex)
                    {
                        err_message = string.Format("Failed to process a nearest intersection {0} for location_def {1}, subscriber Id {2}.ERROR: {3}"
                            , process_type.ToLower()
                            , this._location_def.ToString()
                            , this._subscriber_id
                            , ex.Message);

                        this.EntityMessage(err_message, "INFO");
                        this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                    }
                } //can_process    
            }
        }
        void Process_Notes()
        {
            string err_message;            
            //get the resident notes - Entity_type = 5
            DataTable dtSubNotes = null;

            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber",this._subscriber_id));
                dtSubNotes = this._stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Notes", this._sqlParms).Tables[0];
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Failed to get notes to process for subscriber Id {0}. ERROR: {1}", this._subscriber_id, ex.Message), "INFO");               
            }

            if (dtSubNotes != null)
            {
                foreach (DataRow drNote in dtSubNotes.Rows)
                {
                    bool can_process = true;
                    string title = drNote["note_title"].ToString();
                    string text = string.Empty;
                    if (!string.IsNullOrEmpty(drNote["note_text"].ToString()))
                        text = drNote["note_text"].ToString().Replace("'", "");
                    int notes_def = -1;
                    int access_ref = 100000061;

                    try
                    {
                        this._sql = new StringBuilder();
                        this._sql.AppendFormat("select coalesce(notes_def, -1) as [Notes_Def] from CONTROLCENTRE.NOTES n with (nolock) where entity_type = 5 and entity_ref = {0} and Title = '{1}'"
                            ,this._resident_def.ToString(),title);

                        DataTable dtPNCNote = this._pncProvider.ExecuteDataSetQuery(this._sql.ToString(), null).Tables[0];
                        if (dtPNCNote.Rows.Count > 0)
                        {
                            DataRow drPNCNote = dtPNCNote.Rows[0];
                            notes_def = drPNCNote["Notes_Def"].Parse<int>();
                        }
                    }
                    catch(Exception ex)
                    {
                        this.EntityMessage(string.Format("Error getting note {0} for subscriber Id {1}, location_def {2}. ERROR: {3}"
                            , title,this._subscriber_id, this._location_def.ToString(), ex.Message), "INFO");
                        can_process = false;
                    }

                    if (can_process)
                    {
                        string process_type = "Insert";
                        this._sql = new StringBuilder();

                        if (notes_def == -1)
                        {
                            this._sql.Append("insert into CONTROLCENTRE.NOTES(ACCESS_REF,ENTITY_TYPE,ENTITY_REF,TITLE,TEXT)");

                            if (!string.IsNullOrEmpty(text))
                            {
                                this._sql.AppendFormat("values({0},{1},{2},'{3}','{4}')"
                                    , access_ref.ToString()
                                    , "5"
                                    , this._resident_def
                                    , title
                                    , text);
                            }
                            else
                            {
                                this._sql.AppendFormat("values({0},{1},{2},'{3}','')"
                                    , access_ref.ToString()
                                    , "5"
                                    , this._resident_def
                                    , title);
                            }
                        }
                        else
                        {
                            this._sql.Append("UPDATE CONTROLCENTRE.NOTES");

                            if (!string.IsNullOrEmpty(text))
                                this._sql.AppendFormat(" SET TEXT = '{0}'", text);
                            else
                                this._sql.Append(" SET TEXT = ''");

                            this._sql.AppendFormat(" WHERE NOTES_DEF = {0}", notes_def.ToString());
                            process_type = "Update";
                        }

                        if (this._sql != null)
                        {
                            try
                            {
                                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);
                                this.EntityMessage(string.Format("Successfully processed a note {0}: {1}: {2}, subscriber Id {3}"
                                    , process_type.ToLower()
                                    , title
                                    , text
                                    , this._subscriber_id), "INFO");
                            }
                            catch (Exception ex)
                            {
                                err_message = string.Format("Failed to process a note {0}: {1}: {2} subscriber Id {3}.ERROR: {4}"
                                    , process_type.ToLower()
                                    , title
                                    , text
                                    , this._subscriber_id
                                    , ex.Message);

                                this.EntityMessage(err_message, "INFO");
                                this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
                            }
                        }
                    }                    
                }
            }
        }
        void Update_Mappings()
        {
            try
            {
                //update  the Subs mappings
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                this._sqlParms.Add(SQLParam.GetParam("@master_ref", this._masterreference));
                this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def));
                this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_def));
                this._stageProvider.ExecuteNonQuery("SYNC_Update_Subscriber_Mappings",this._sqlParms);
                this.EntityMessage(string.Format("Updated the mappings for subscriber {0}.",this._subscriber_id),"INFO");
            }
            catch(Exception ex)
            {                
                this.EntityMessage(string.Format("Failed to update the mappings for subscriber {0}. ERROR: {1}",this._subscriber_id,ex.Message)
                    ,"ERROR");
            }
        }
        void Update_Mapping_Unlink()
        {
            try
            {
                //update  the Subs mappings
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                this._sqlParms.Add(SQLParam.GetParam("@master_ref", this._masterreference));
                this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def));
                this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_def));
                this._sqlParms.Add(SQLParam.GetParam("@can_udpate_original_location", this._new_home_created));
                this._stageProvider.ExecuteNonQuery("SYNC_Update_Subscriber_Mappings_Unlink",this._sqlParms);
                this.EntityMessage(string.Format("Updated the mappings on unlink for subscriber {0}.",this._subscriber_id),"INFO");
            }
            catch(Exception ex)
            {                
                this.EntityMessage(string.Format("Failed to update the mappings on unlink for subscriber {0}. ERROR: {1}",this._subscriber_id,ex.Message)
                    ,"ERROR");
            }
        }
        void Add_Responders_To_Log(string update_type)
        {
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                this._sqlParms.Add(SQLParam.GetParam("@update_type", update_type));
                this._sqlParms.Add(SQLParam.GetParam("@group_record", this._group_record));
                this._stageProvider.ExecuteNonQuery("SYNC_SubscriberResponders_To_Log", this._sqlParms);
                this.EntityMessage(string.Format("Added responders to change log for subscriber {0}.", this._subscriber_id), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to add responders to change log for subscriber {0}. ERROR: {1}", this._subscriber_id, ex.Message)
                    , "ERROR");
            }
        }
        void Add_Keywords_To_Log(string update_type)
        {
            // first delete the current keyword relations
            bool updateKeyword = this.Delete_Keyword_Relation();
            if (updateKeyword)
            {
                try
                {
                    this._sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                    this._sqlParms.Add(SQLParam.GetParam("@update_type", update_type));
                    this._sqlParms.Add(SQLParam.GetParam("@group_record", this._group_record));
                    this._stageProvider.ExecuteNonQuery("SYNC_SubscriberMedicalInfo_To_Log", this._sqlParms);
                    this.EntityMessage(string.Format("Added keywords to change log for subscriber {0}.", this._subscriber_id), "INFO");
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Failed to add keywords to change log for subscriber {0}. ERROR: {1}", this._subscriber_id, ex.Message)
                        , "ERROR");
                }
            }
        }
        void Reset_PL_SecondUser()
        {
            try
            {
                this._sql = new StringBuilder();
                this._sql.AppendFormat("update [ks_Subscriber] set seconduser = 'N', IPAddress = NULL where subscriber_id = '{0}'", this._subscriber_id);

                this._stageProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                EntityMessage(string.Format("Reset SecondUser column to N for subscriber {0}", this._subscriber_id), "INFO");

            }
            catch (Exception ex)
            {
                string err_message = string.Format("Failed to reset SecondUser column for subscriber {0}.ERROR: {1}", this._subscriber_id, ex.Message);
                this.EntityMessage(err_message, "ERROR");
                this._serviceRepository.Save_Transaction_Record(this._group_record, "SUBSCRIBER", this._sql.ToString(), err_message);
            }
        }
        void Transfer_Subscriber_To_Unmapped_Agency()
        {
            bool is_mapped = false;
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", this._subscriber_id));
                DataRow drMappings = this._stageProvider.ExecuteDataSet("SYNC_Is_Subscriber_Mapped", this._sqlParms).Tables[0].Rows[0];

                this._location_def = drMappings["Location_Def"].Parse<int>();
                this._resident_def = drMappings["Resident_Def"].Parse<int>();
                is_mapped = drMappings["Is_Mapped"].Parse<bool>();
            }
            catch { is_mapped = false; }

            if (is_mapped)
            {
                try
                {
                    Save_Epec_Location("UPDATE");
                    Save_Resident("UPDATE");

                    this.EntityMessage(string.Format("Transferred subscriber {0} to an unmapped agency.", this._subscriber_id), "INFO");
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("failed to transfer subscriber {0} to an unmapped agency. ERROR: {1}", this._subscriber_id, ex.Message), "ERROR");
                }
            }
            else
            {
                this.EntityMessage("No Records to process for Get_Subscriber_Update.sql", "INFO");
            }
        }
        void Change_Subscriber_Contact_Relation()
        {
            try
            {
                this._sql = new StringBuilder();
                this._sql.Append("Update CONTROLCENTRE.CONTACT_RELATION");
                this._sql.AppendFormat(" set Location_Ref = {0}", this._location_def.ToString());
                this._sql.AppendFormat(" where resident_ref = {0}", this._resident_def.ToString());
                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                this.EntityMessage(string.Format("Successfully updated the contact_relation.location_ref to {0} for resident {1} for subscriber {2} on an unjoin."
                    ,this._location_def.ToString(), this._resident_def.ToString(),this._subscriber_id), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to update the contact_relation.location_ref to {0} for resident {1} for subscriber {2} on an unjoin. ERROR: {3}"
                    , this._location_def.ToString(), this._resident_def.ToString(), this._subscriber_id,ex.Message), "ERROR");
            }            
        }
        void Update_Map_Contact_Relation()
        {
            //change the mapping
            try
            {
                this._sql = new StringBuilder();
                this._sql.Append("UPDATE MAP_CONTACT_RELATION");
                this._sql.AppendFormat(" SET LOCATION_DEF = {0}", this._location_def.ToString());
                this._sql.AppendFormat(" WHERE Subscriber_Id = '{0}'", this._subscriber_id);
                this._stageProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                this.EntityMessage(string.Format("Successfully updated the map_contact_relation.location_def to {0} for subscriber {1}."
                    , this._location_def.ToString(), this._subscriber_id), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to update the map_contact_relation.location_ref to {0} for subscriber {1}. ERROR: {2}"
                    , this._location_def.ToString(), this._subscriber_id, ex.Message), "ERROR");
            }
        }
        void Add_Record_To_PNC_Log()
        {
            try
            {
                this._sql = new StringBuilder();
                this._sql.Append("INSERT INTO CONTROLCENTRE.ETL_LOG_CUD(ENTITY_TABLE,ENTITY_TYPE,ENTITY_REF,UPDATE_TIME,UPDATE_TYPE)");
                this._sql.AppendFormat("VALUES('RESIDENT',5,{0},GETDATE(),'U')", this._resident_def.ToString());
                this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                this.EntityMessage(string.Format("Successfully added an update for resident {0}, subscriber {1} to the PNC ETL Log."
                    , this._resident_def.ToString(), this._subscriber_id), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to add an update for resident {0}, Subscriber {1} to the PNC ETL Log."
                    , this._resident_def.ToString(), this._subscriber_id, ex.Message), "INFO");
            }
        }
        void Set_Map_Subscriber_Authority_Mapping(bool value)
        {
            //1. since the master reference is the main on the home, we use their authority 
            // when going to PNC
            try
            {
                long auth_ref = this.Get_MasterReference_Authority();

                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                this._sqlParms.Add(SQLParam.GetParam("@authority_ref", auth_ref));
                this._sqlParms.Add(SQLParam.GetParam("@agency_id", this._agency_id));

                if (value)
                {
                    this._sqlParms.Add(SQLParam.GetParam("@method", "INSERT"));
                }
                else
                {
                    this._sqlParms.Add(SQLParam.GetParam("@method", "DELETE"));
                }

                this._stageProvider.ExecuteNonQuery("SYNC_Set_Joined_Subscriber_Authority_Mapping", this._sqlParms);
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Failed to set the joined subscriber authority mapping for subscriber Id {0}. ERROR: {1}", this._subscriber_id, ex.Message)
                    , "INFO");
            }
        }
        void Process_Equipment()
        {
            // get the pendant to process
            DataTable equipment_to_process = null;

            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                this._sqlParms.Add(SQLParam.GetParam("@service", "KSHEMA"));
                this._sqlParms.Add(SQLParam.GetParam("@location_def", -1));
                this._sqlParms.Add(SQLParam.GetParam("@resident_def", -1));
                this._sqlParms.Add(SQLParam.GetParam("@sub_type", this._sub_type));
                equipment_to_process = this._stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Equipment", this._sqlParms).Tables[0];
            }
            catch { }

            if (equipment_to_process != null)
            {
                #region " Logic "
                // process each to the the PNC.Equipment table 
                // if it exists then update the record based on the equip_comment
                // which will hold the id
                // if the unit does not exist then insert a new record
                // column mapping:
                // SubscriberEquipment_Id   -   Equip_Comment
                // Unit_Id                  -   IDENT
                // Status                   -   STATUS
                // InstallDate              -   Installation_DATE
                // Equip_Model_Def          -   Equip_model_ref
                // Frequency                -   TRIGGER_ID
                #endregion
                foreach(DataRow dr_to_process in equipment_to_process.Rows)
                {
                    try
                    {
                        int equipment_id = dr_to_process["equip_id"].Parse<int>();
                        int equip_ref = dr_to_process["equip_ref"].Parse<int>();
                        long model_ref = dr_to_process["Equip_model_def"].Parse<long>();
                        //string unit_id = dr_to_process["unit_id"].ToString();
                        string serial_no = dr_to_process["serial_no"].ToString();
                        string status = dr_to_process["status"].ToString();
                        string trigger_id = dr_to_process["trigger_id"].ToString().ToUpper();  
                        //string warranty_exp = dr_to_process["warranty_exp"].ToString();
                        //string battery_exp = dr_to_process["battery_exp"].ToString();
                        //string maintenance_exp = dr_to_process["maintenance_exp"].ToString();

                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@equip_id", equipment_id));
                        this._sqlParms.Add(SQLParam.GetParam("@equip_ref", equip_ref));
                        this._sqlParms.Add(SQLParam.GetParam("@equip_model_def", model_ref));
                        this._sqlParms.Add(SQLParam.GetParam("@unit_id", this._equip_id));
                        this._sqlParms.Add(SQLParam.GetParam("@serial_no", serial_no));
                        this._sqlParms.Add(SQLParam.GetParam("@trigger_id", trigger_id));
                        this._sqlParms.Add(SQLParam.GetParam("@status", status));

                        if (!string.IsNullOrEmpty(dr_to_process["installation_date"].ToString()))
                            this._sqlParms.Add(SQLParam.GetParam("@install_date", dr_to_process["installation_date"].ToString()));

                        if (!string.IsNullOrEmpty(dr_to_process["battery_exp"].ToString()))
                            this._sqlParms.Add(SQLParam.GetParam("@battery_exp", dr_to_process["battery_exp"].ToString()));

                        if (!string.IsNullOrEmpty(dr_to_process["warranty_exp"].ToString()))
                            this._sqlParms.Add(SQLParam.GetParam("@warranty_exp", dr_to_process["warranty_exp"].ToString()));

                        if (!string.IsNullOrEmpty(dr_to_process["maintenance_exp"].ToString()))
                            this._sqlParms.Add(SQLParam.GetParam("@warranty_exp", dr_to_process["maintenance_exp"].ToString()));

                        this._sqlParms.Add(SQLParam.GetParam("@location_def", this._location_def));
                        this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_def));
                        this._sqlParms.Add(SQLParam.GetParam("@sub_name", string.Format("{0} {1}",this._first_name,this._last_name)));
                        this._sqlParms.Add(SQLParam.GetParam("@sub_type", this._sub_type));

                        this._stageProvider.ExecuteNonQuery("SYNC_Process_Subscriber_Equipment", this._sqlParms);

                        this.EntityMessage(string.Format("Successfully processed equipment for unit {0}. Subscriber Id {1}", this._equip_id,this._subscriber_id), "INFO");
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to process equipment for Subscriber Id {0}. ERROR: {1}", this._subscriber_id, ex.Message), "INFO");
                    }    
                }
            }
        }
        
        void Process_Insert()
        {
            bool can_process = true;
            bool process_resident = true;

            if (this._primary_YN == "Y")
            {
                #region v 1
                //long unit_id = this.UnitId_Transform();
                //if (unit_id == 0)
                //{
                //    can_process = this.Create_Temp_PNC_Equip();
                //}
                //else
                //{
                //    this._equip_id = unit_id.ToString();
                //}
                #endregion
                #region v 1.1

                can_process = this.Generate_Equip_Id("INSERT");

                #endregion
            }

            if (can_process)
                process_resident = this.Save_Epec_Location("INSERT");

            if(process_resident)
            {
                can_process = this.Save_Resident("INSERT");

                if (can_process)
                {
                    Save_Resident_Attribute(ATTRIBUTE_TYPE.STATUS);
                    Save_Resident_Attribute(ATTRIBUTE_TYPE.LANGUAGE);
                    Save_Resident_Attribute(ATTRIBUTE_TYPE.GENDER);


                    //JIRA SYNC-75
                    if(this._status.ToLower() != "pending install")
                        Save_Resident_Attribute(ATTRIBUTE_TYPE.SUBSCRIBER_TYPE);
                }

                this.Process_Nearest_Intersection();
                this.Process_Notes();
                //this.Process_Equipment();
                //1.3.2
                Equipment equip = new Equipment();
                equip.ProcessSubscriberEquipment(this._subscriber_id, this._location_def, this._resident_def, this._sub_type, this._equip_id, string.Format("{0} {1}", this._first_name, this._last_name));

            }
        }
        void Process_Update()
        {
            bool can_process = true;
            bool isMapped = this.Map_Subscriber_Location();
            bool process_resident = true;
            if (isMapped)
            {
                this.EntityMessage(string.Format("Processing subscriber {0} Update", this._subscriber_id), "INFO");

                string link = this.Check_Link_Unlink().ToLower();
                switch (link)
                {
                    case "process":

                        this.EntityMessage(string.Format("Processing subscriber {0} normally", this._subscriber_id), "INFO");

                        can_process = this.Generate_Equip_Id("UPDATE");

                        if(this._subscriber_id == this._masterreference)
                            process_resident = this.Save_Epec_Location("UPDATE");


                        break;

                    case "link":

                        //if the sub is linked, they are added to the home that is associated 
                        // with the master reference sub
                        //1. get the master reference home

                        this.EntityMessage(string.Format("Processing a {0} on subscriber {1}", link, this._subscriber_id), "INFO");

                        Subscriber_Mapping parent = new Subscriber_Mapping();
                        parent.Fetch(this._masterreference);                        

                        if (parent.Location_Def != -1)
                        {

                            this.EntityMessage(string.Format("Found parent location_def {0}, master reference {1} for subscriber {2}, unit Id {3}"
                                , parent.Location_Def.ToString(), parent.Subscriber_Id, this._subscriber_id,parent.Equip_ID.ToString()), "INFO");

                            this._location_def = parent.Location_Def;
                            this._authority_ref = parent.Authority_Ref;
                            this._masterreference = parent.Subscriber_Id;
                            this._equip_id = parent.Equip_ID.ToString();


                            //JIRA: SYNC-67
                            // save the old home unit to the property_ref field in the new home
                            if (this._subscriber_id != this._masterreference)
                            {
                                try
                                {
                                    this._sqlParms = new List<SQLParam>();
                                    this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                                    this._sqlParms.Add(SQLParam.GetParam("@unit_id", this._equip_id));
                                    this._stageProvider.ExecuteNonQuery("SYNC_Set_Joined_Subscriber_PropertyRef_UnitId", this._sqlParms);
                                }
                                catch (Exception ex)
                                {
                                    this.EntityMessage(string.Format("Failed to set property_ref for subscriber {0}. ERROR: {1}"
                                    , this._subscriber_id, ex.Message), "ERROR");
                                }
                            }
                        }
                        break;

                    case "unlink":

                        this.EntityMessage(string.Format("Processing a {0} on subscriber {1}", link, this._subscriber_id), "INFO");
                        if (this._second_user == "Y")
                        {
                            this.Get_Home_To_Move_Subscriber();
                            if (this._location_def == -1)
                            {
                                // since there is no location to put the sub into we create a new shell
                                can_process = this.Create_Temp_PNC_Equip();

                                if (can_process)
                                {
                                    this._new_home_created = true;
                                    this.EntityMessage(string.Format("No home found to move subscriber {0} too. Created new temp unit id {1}.", this._subscriber_id, this._equip_id.ToString()), "INFO");
                                    process_resident = this.Save_Epec_Location("INSERT");
                                }
                            }
                            else
                            {
                                can_process = this.Generate_Equip_Id("UPDATE");
                                if(can_process)
                                    process_resident = this.Save_Epec_Location("UPDATE");

                            }
                        }
                        else
                            can_process = this.Generate_Equip_Id("UPDATE");

                        break;

                }

                if (process_resident)
                {
                    isMapped = this.Map_Subscriber_Resident();
                    if (isMapped)
                    {
                        can_process = this.Save_Resident("UPDATE");

                        if (can_process)
                        {
                            switch (link)
                            {
                                case "link":

                                    if (this._primary_YN == "N")
                                    {
                                        // this person is moving into a home with a primary so delete
                                        //1.contact_relation
                                        //2. map_contact_relation
                                        this.Update_Map_Contact_Relation();
                                    }

                                    Update_Mappings();

                                    // add the user to the MAP_JOINED_SUBSCRIBER_AUTHORITY table
                                    this.Set_Map_Subscriber_Authority_Mapping(true);

                                    break;


                                case "unlink":

                                    if (this._second_user == "Y")
                                    {
                                        // if ipaddress = 1 then we need to change the contact_relation.location_ref to the new home
                                        if (this._ipAddress == "1")
                                        {
                                            this.Change_Subscriber_Contact_Relation();
                                            this.Update_Map_Contact_Relation();

                                        }
                                        else
                                        {
                                            // delete this users contacts and relations so new ones can be created
                                            this.Delete_Subscriber_Contacts();
                                        }
                                    }

                                    // reset the second user field
                                    this.Reset_PL_SecondUser();


                                    this.Update_Mapping_Unlink();

                                    //TODO: need to process inserts to the PNC ETL_LOG_CUD for EPEC so that the equipment changes can pass to PL
                                    this.Add_Record_To_PNC_Log();

                                    this.Set_Map_Subscriber_Authority_Mapping(false);

                                    break;
                            }

                            Save_Resident_Attribute(ATTRIBUTE_TYPE.STATUS);
                            Save_Resident_Attribute(ATTRIBUTE_TYPE.LANGUAGE);
                            Save_Resident_Attribute(ATTRIBUTE_TYPE.GENDER);

                            //JIRA SYNC-77
                            switch(this._sub_type.ToLower())
                            {
                                case "msd":
                                case "cellular":
                                case "(pnc)":
                                    Remove_Resident_Attribute(ATTRIBUTE_TYPE.SUBSCRIBER_TYPE);
                                    break;

                                default:
                                    Save_Resident_Attribute(ATTRIBUTE_TYPE.SUBSCRIBER_TYPE);
                                    break;
                            }                           


                            this.Process_Nearest_Intersection();
                            this.Process_Notes();
                            //this.Process_Equipment();
                            //1.3.2
                            Equipment equip = new Equipment();
                            equip.OnEntityMessage += equip_OnEntityMessage;
                            equip.ProcessSubscriberEquipment(this._subscriber_id, this._location_def, this._resident_def, this._sub_type, this._equip_id, string.Format("{0} {1}", this._first_name, this._last_name));
                            
                        } // can_process   
                    }
                }//process_resident                
            }//mapped
            else
            {
                // if there is no mapping for an update transaction 
                // then it is a transfer and we need to process it as an insert
                // on the next pass
                try
                {
                    this._sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._subscriber_id));
                    this._sqlParms.Add(SQLParam.GetParam("@update_type", "I"));
                    this._stageProvider.ExecuteNonQuery("dbo.SYNC_Subscriber_To_EtlChangeLog", this._sqlParms);

                }
                catch { }
            }
        }
               
        void Process_Delete()
        {

        }

        void equip_OnEntityMessage(string message, string log_type)
        {
            this._serviceRepository.Log_Service_Message(log_type, message);
            Console.WriteLine(string.Format("{0}    {1}: {2}", DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"), log_type, message));
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow(string process)
        {
            bool isMapped = this.Map_Properties();
            if (isMapped)
            {
                // check if this is a synced agency
                isMapped = this.Check_Agency_Mapping(this._agency_id);
                if (isMapped)
                {
                    switch (process.ToLower())
                    {
                        case "insert":
                            this.Process_Insert();
                            break;

                        case "update":
                            this.Process_Update();
                            break;
                    }
                }
                else
                    Transfer_Subscriber_To_Unmapped_Agency();
            }
        }
        
        #endregion
    }
}

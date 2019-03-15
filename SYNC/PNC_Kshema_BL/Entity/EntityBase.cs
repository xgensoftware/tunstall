using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class EntityBase
    {
        #region " Event "
        public event EntityMessage_Handler OnEntityMessage;
        #endregion

        #region " Member Variables "

        protected string _group_record = "-1";

        //Kshema fields
        protected string _Subscriber_ID;
        protected string _MasterReference;
        protected int _SubscriberResponder_ID;
        protected string _AgencyName;
        protected string _AgencyCode;
        protected string _SubscontractorId;
        protected string _Status;
        protected string _Code;
        protected int _SubscriberMedicalInfo;

        //PNC 
        protected int _locationDef;
        protected int _residentDef;
        protected int _authorityRef;
        protected int _equipModelRef = 0;
        protected int _colourRef;
        protected int _contact_def;
        protected int _contact_type_ref;
        protected int _entity_type;
       // protected long _subscriber_type = -1;
        

        protected string _unit_id = string.Empty;
        protected string _addr1 = string.Empty;
        protected string _addr2 = string.Empty;
        protected string _city = string.Empty;
        protected string _state = string.Empty;
        protected string _zip = string.Empty;
        protected string _zipplus4 = string.Empty;
        protected string _phone = string.Empty;
        protected string _mobile = string.Empty;
        protected string _specialInstructions = string.Empty;
        protected string _sub_type = string.Empty;

        protected string _firstname = string.Empty;
        protected string _mi = string.Empty;
        protected string _lastname = string.Empty;
        protected string _fullname = string.Empty;
        protected string _primary_YN = string.Empty;
        protected string _secondUser = string.Empty;
        protected string _gender = string.Empty;
        protected string _nearestIntersection = string.Empty;
        protected string _equipModelText = string.Empty;
        protected string _otherPhone = string.Empty;
        protected string _type = string.Empty;
        protected string _serial_no = "-1";


        protected string _installDate = null;
        protected string _onlineSince = null;
        protected string _removalDate = null;
        protected string _orderDate = null;
        protected string _dob = null;
        protected string _entryDate = null;

        //Common fields 
        protected string _agency_id = string.Empty;

        //protected DataRow _RowToProcess;
        protected DataRow _dataToMapp;
        protected List<SQLParam> _sqlParms = null;
        protected IDataProvider _pncProvider = null;
        protected IDataProvider _kshemaProvider = null;
        protected IDataProvider _stageProvider = null;

        protected string _previous_type;
        

        #endregion

        #region " Methods "

        private void Reset_Subscriber_Tunstall_Equipment()
        {
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._Subscriber_ID));
                this._sqlParms.Add(SQLParam.GetParam("@sub_type", this._sub_type));
                this._stageProvider.ExecuteNonQuery("SYNC_Reset_Subscriber_Tunstall_Equipment", this._sqlParms);
                this.EntityMessage(string.Format("Reseting the AMAC Equipment table for subscriber {0}. Subscriber type: {1}", this._Subscriber_ID,this._sub_type), "INFO");
            }
            catch(Exception ex) { this.EntityMessage(string.Format("Error reseting the AMAC Equipment table for subscriber {0}. ERROR:{1}", this._Subscriber_ID,ex.Message), "INFO"); }
        }

        private string Set_Unit_Padding()
        {
            string val = "PNC";

            if (this._sub_type != "-1")
            {
                long padding = 0;

                try
                {
                    this._sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@sync_type", "PNC"));
                    this._sqlParms.Add(SQLParam.GetParam("@type", this._sub_type.ToString()));
                    DataTable dtunit_padding = this._stageProvider.ExecuteDataSet("SYNC_Get_Unit_Padding", this._sqlParms).Tables[0];
                    padding = dtunit_padding.Rows[0]["Padding"].Parse<long>();
                    this._sub_type = dtunit_padding.Rows[0]["Sub_Type"].ToString();
                    val = "KSHEMA";
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Failed to get the padding for subscriber type {0} for subscriber {1}. ERROR: {2}", this._sub_type.ToString(), this._Subscriber_ID, ex.Message), "INFO");
                }

                if (padding != 0)
                {                    
                    this._unit_id = (this._unit_id.Parse<long>() - padding).ToString("X4");
                }
            }
            
            return val;
        }

        private void Send_Error_Mail(string email_message)
        {
            try
            {
                SendMailHelper mail = new SendMailHelper(AppConfiguration.Email_Server, AppConfiguration.Email_User, AppConfiguration.Email_Password);
                mail.SendEmail("Server_VMTCMSSQL1@tunstallamac.com"
                    , AppConfiguration.Email_ToAddress
                    , "Tunstall PNC to Kshema Sync Error"
                    , email_message);
            }
            catch { }
        }

        private void Send_New_Account_Created_Email(string unit_id, string agency_id)
        {
            try
            {
                string[] toAddr = new string[4] {"providerRelations@tunstallamac.com", "development.admins@tunstallamac.com", "renee.gerrish@tunstall.com", "Charles.derose@tunstallamac.com"};

                string msg = string.Format("New Home created in PNC (Equipment ID: {0}, Agency: {1}). Please check if duplicate.", unit_id, agency_id);

                SendMailHelper mail = new SendMailHelper(AppConfiguration.Email_Server, AppConfiguration.Email_User, AppConfiguration.Email_Password);
                mail.SendEmail("Server_VMTCMSSQL1@tunstallamac.com"
                    , toAddr.ToList()
                    , "PNC New Home"
                    , msg
                    , null
                    , false);
            }
            catch { }
        }

        private void Update_Subscriber_Type()
        {
            //Cellular: (!ISNULL(equip_model_ref) && equip_model_ref == 100000205)
            if (_equipModelRef == 100000205 | _equipModelRef == 61 | _equipModelRef == 100000379)
            {
                this._type = "Cellular";
            }
            else if (_equipModelRef == 92 | _equipModelRef == 100000380)
            {
                //MSD: (!ISNULL(equip_model_ref) && equip_model_ref == 92)
                this._type = "MSD";
            }
            //else if ((_equipModelRef == 100000003 | _equipModelRef == 100000204))
            //{
            //    //DSPNET: (!ISNULL(equip_model_ref) && (equip_model_ref == 100000003) || (equip_model_ref == 100000204))
            //    this._type = "DSPNET";
            //}
            else if (_equipModelRef == 100000001 | 
                _equipModelRef == 100000002 | 
                _equipModelRef == 51 | 
                _equipModelRef == 52 | 
                _equipModelRef == 53 | 
                _equipModelRef == 100000206 | 
                _equipModelRef == 100000207 | 
                _equipModelRef == 100000208)
            {
                //PNC: (!ISNULL(equip_model_ref) && ((equip_model_ref != 100000205) || (equip_model_ref != 92) || equip_model_ref != 100000003) || (equip_model_ref != 100000204))
                this._type = "(PNC)";
            }
        }    

        protected void InitializeProviders()
        {
            _pncProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfiguration.PNC_Connection);
            _kshemaProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.Kshema_Connection);
            _stageProvider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
        }

        protected void Save_Transaction_Log(string group_record, string entity, string transaction, string err_message)
        {
            try
            {
                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@group_record", group_record));
                parms.Add(SQLParam.GetParam("@entity", entity));
                parms.Add(SQLParam.GetParam("@transaction", transaction));
                parms.Add(SQLParam.GetParam("@err_message", err_message));

                _stageProvider.ExecuteNonQuery("SYNC_Insert_Transaction_Log", parms);
            }
            catch { }
        }

        protected void Log_Transaction(string group_record,string entity, string transaction, string err_message)
        {
            try
            {
                List<SQLParam> parms = new List<SQLParam>();
                parms.Add(SQLParam.GetParam("@group_record", group_record));
                parms.Add(SQLParam.GetParam("@entity", entity));
                parms.Add(SQLParam.GetParam("@transaction", transaction));
                parms.Add(SQLParam.GetParam("@err_message", err_message));

                _stageProvider.ExecuteNonQuery("SYNC_Insert_Transaction_Log", parms);

            }
            catch { }
        }

        protected void EntityMessage(string msg,string message_type)
        {
            if (OnEntityMessage != null)
            {
                OnEntityMessage(msg, message_type);
                if (message_type.ToLower() == "error")
                    Send_Error_Mail(msg);
            }
        }

        protected void Check_Joined_Subscriber_Authority()
        {
            if (!string.IsNullOrEmpty(this._Subscriber_ID))
            {
                try
                {
                    _sqlParms = new List<SQLParam>();
                    _sqlParms.Add(SQLParam.GetParam("@subscriber", this._Subscriber_ID));
                    string agency_id = _stageProvider.GetData("SYNC_Check_JOINED_SUBSCRIBER_AUTHORITY", _sqlParms).Rows[0]["Agency_ID"].ToString();
                    if (agency_id != "-1")
                    {
                        this._AgencyCode = agency_id;
                    }

                }
                catch (Exception ex)
                {
                    EntityMessage(string.Format("Failed to get agency mapping on joined Subscriber {0}, agency_Id {1}. ERROR: {2}", this._Subscriber_ID, _agency_id, ex.Message), "INFO");
                }
            }
        }

        protected bool PNC_EPEC_AGENCY_MAP()
        {
            bool isMatch = false;          

            
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@authref", _agency_id));
                DataRow drAgency = _stageProvider.GetData("SYNC_Agency_Lookup", _sqlParms).Rows[0];

                _AgencyName = drAgency["AGENCYNAME"].ToString();
                _AgencyCode = drAgency["AGENCY_ID"].ToString();
                _SubscontractorId = drAgency["SUBCONTRACTOR_ID"].ToString();

                isMatch = true;
                EntityMessage(string.Format("Successfully mapped agency {0} for location def {1}.", _AgencyName, _locationDef.ToString()),"INFO");
            }
            catch (Exception ex)
            {                
                EntityMessage(string.Format("No agency map found for location_def {0} with and authority_ref {1}.", _locationDef.ToString(),_agency_id),"ERROR");
            }
            

            return isMatch;
        }

        protected bool PNC_EQUIP_STATUS_MAP()
        {
            bool isMatch = false;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@authref", _authorityRef));
                _sqlParms.Add(SQLParam.GetParam("@colourref", _colourRef));
                _Status = _stageProvider.ExecuteScalar<string>("SYNC_Map_Equip_Status", _sqlParms);

                isMatch = true;
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Failed to map equipment status for location def {0}. Error: {1}", _locationDef, ex.Message),"ERROR");
            }

            return isMatch;
        }

        protected bool MapProperties()
        {
            bool isSuccess = true;
            
            try
            {
                //non-nullables
                _locationDef = _dataToMapp["location_def"].Parse<int>();
                _residentDef = _dataToMapp["resident_def"].Parse<int>();
                _authorityRef = _dataToMapp["authority_ref"].Parse<int>();
                _equipModelRef = _dataToMapp["equip_model_ref"].Parse<int>();
                _colourRef = _dataToMapp["colour_ref"].Parse<int>();

                _unit_id = _dataToMapp["unit_id"].ToString();
                _agency_id = _dataToMapp["agency_id"].ToString();
                _addr1 = _dataToMapp["address1"].ToString();
                _city = _dataToMapp["city"].ToString();
                _state = _dataToMapp["state"].ToString();
                _phone = _dataToMapp["phone"].ToString();                
                _firstname = _dataToMapp["firstname"].ToString();
                _lastname = _dataToMapp["lastname"].ToString();
                _fullname = _dataToMapp["fullname"].ToString();
                _primary_YN = _dataToMapp["Primary_YN"].ToString();
                _secondUser = _dataToMapp["SecondUser"].ToString();
                _equipModelText = _dataToMapp["equip_model_text"].ToString();
                _type = _dataToMapp["Type"].ToString();
                _gender = _dataToMapp["Gender"].ToString();
                _sub_type = _dataToMapp["Sub_Type"].ToString();

                //if (!string.IsNullOrEmpty(_dataToMapp["Sub_Type"].ToString()))
                //    this._subscriber_type = _dataToMapp["Sub_Type"].Parse<long>();

                if (!string.IsNullOrEmpty(_dataToMapp["Other_Phone"].ToString()))
                {                    
                    _otherPhone = _dataToMapp["Other_Phone"].ToString();
                    if (_otherPhone.Length > 10)
                        _otherPhone = _otherPhone.Substring(1);
                }

                if (!string.IsNullOrEmpty(_dataToMapp["Mobile"].ToString()))
                {
                    _mobile = _dataToMapp["Mobile"].ToString();
                    if(_mobile.Length >10)
                    {
                        _mobile = _mobile.Substring(1);
                    }
                }
                   

                if (!string.IsNullOrEmpty(_dataToMapp["address2"].ToString()))
                    _addr2 = _dataToMapp["address2"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["zip"].ToString()))
                    _zip = _dataToMapp["zip"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["zipplus4"].ToString()))
                    _zipplus4 = _dataToMapp["zipplus4"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["installdate"].ToString()))
                    _installDate = _dataToMapp["installdate"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["OnlineSince"].ToString()))
                    _onlineSince = _dataToMapp["OnlineSince"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["removaldate"].ToString()))
                    _removalDate = _dataToMapp["removaldate"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["entrydate"].ToString()))
                    _entryDate = _dataToMapp["entrydate"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["OrderDate"].ToString()))
                    _orderDate= _dataToMapp["OrderDate"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["dob"].ToString()))
                    _dob = _dataToMapp["dob"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["middleinitial"].ToString()))
                    _mi = _dataToMapp["middleinitial"].ToString();

                if (!string.IsNullOrEmpty(_dataToMapp["specialinst"].ToString()))
                    _specialInstructions = _dataToMapp["specialinst"].ToString();

                if(!string.IsNullOrEmpty(_dataToMapp["NearestIntersection"].ToString()))
                    _nearestIntersection = _dataToMapp["NearestINtersection"].ToString();

            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.EntityMessage(string.Format("Failed to map properties for location_def {0}. Error: {1}", _dataToMapp["location_def"].ToString(), ex.Message),"ERROR");
            }
            return isSuccess;
        }

        protected void Insert_Temp_Subscriber_Q()
        {
            string msg = string.Empty;
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@LocationDef", _locationDef));
                _sqlParms.Add(SQLParam.GetParam("@ResidentDef", _residentDef));
                _sqlParms.Add(SQLParam.GetParam("@FName", _firstname));
                _sqlParms.Add(SQLParam.GetParam("@MI", _mi));
                _sqlParms.Add(SQLParam.GetParam("@LName", _lastname));
                _sqlParms.Add(SQLParam.GetParam("@Agency_ID", _AgencyCode));
                _sqlParms.Add(SQLParam.GetParam("@Address1", _addr1));
                _sqlParms.Add(SQLParam.GetParam("@Address2", _addr2));
                _sqlParms.Add(SQLParam.GetParam("@City", _city));
                _sqlParms.Add(SQLParam.GetParam("@State", _state));
                _sqlParms.Add(SQLParam.GetParam("@Zip", _zip));
                _sqlParms.Add(SQLParam.GetParam("@Phone", _phone));
                _sqlParms.Add(SQLParam.GetParam("@SpInst", _specialInstructions));
                _sqlParms.Add(SQLParam.GetParam("@Equip_Status", _Status));
                _sqlParms.Add(SQLParam.GetParam("@Primary_YN", _primary_YN));
                _sqlParms.Add(SQLParam.GetParam("@SecUser", _secondUser));
                _sqlParms.Add(SQLParam.GetParam("@DOB", _dob));
                _sqlParms.Add(SQLParam.GetParam("@EntryDate", _entryDate));
                _sqlParms.Add(SQLParam.GetParam("@InstallDate", _installDate));
                _sqlParms.Add(SQLParam.GetParam("@RemovalDate", _removalDate));
                _sqlParms.Add(SQLParam.GetParam("@OrderDate", _orderDate));
                _sqlParms.Add(SQLParam.GetParam("@Gender", _gender));
                _sqlParms.Add(SQLParam.GetParam("@NearInters", _nearestIntersection));
                _sqlParms.Add(SQLParam.GetParam("@Zip4", _zipplus4));
                _sqlParms.Add(SQLParam.GetParam("@OnlineSince", _onlineSince));
                _sqlParms.Add(SQLParam.GetParam("@Equip_ID", _unit_id));
                _sqlParms.Add(SQLParam.GetParam("@Equip_Model_Ref", _equipModelRef));
                _sqlParms.Add(SQLParam.GetParam("@Equip_Model_Text", _equipModelText));
                _sqlParms.Add(SQLParam.GetParam("@Other_Phone", _otherPhone));
                _sqlParms.Add(SQLParam.GetParam("@Serial_No", _serial_no));

                _stageProvider.ExecuteNonQuery("SYNC_Insert_Subscriber_Q", _sqlParms);

                EntityMessage("New record inserted into Subscriber queue to be processed.","INFO");

                this.Send_New_Account_Created_Email(_unit_id, _AgencyCode);
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Failed to insert location_def {0}, resident_def {1} into Subscriber queue. ERROR: {2}", _locationDef.ToString(),_residentDef.ToString(), ex.Message),"ERROR");
            }
        }
        
        protected void Update_Subscriber()
        {
            string msg = string.Empty;
            string system = this.Set_Unit_Padding();

            this.Update_Subscriber_Type();

            #region Create string with all parameters
            StringBuilder parameterList = new StringBuilder();
            parameterList.AppendFormat("Subscriber {0} update parameters: ", this._Subscriber_ID);
            parameterList.AppendFormat(",@SubscriberId: {0}", _Subscriber_ID);
            parameterList.AppendFormat(",@Address1: {0}", _addr1);
            parameterList.AppendFormat(",@Address2: {0}", _addr2);
            parameterList.AppendFormat(",@AgencyID: {0}", _AgencyCode);
            parameterList.AppendFormat(",@AgencyName: {0}", _AgencyName);
            parameterList.AppendFormat(",@City: {0}", _city);
            parameterList.AppendFormat(",@DOB: {0}", _dob);
            parameterList.AppendFormat(",@EntryDate: {0}", _entryDate);
            parameterList.AppendFormat(",@FName: {0}", _firstname);
            parameterList.AppendFormat(",@InstallDate: {0}", _installDate);
            parameterList.AppendFormat(",@MI: {0}", _mi);
            parameterList.AppendFormat(",@LName: {0}", _lastname);
            parameterList.AppendFormat(",@NearInters: {0}", _nearestIntersection);
            parameterList.AppendFormat(",@OrderDate: {0}", _orderDate);
            parameterList.AppendFormat(",@Phone: {0}", _phone);
            parameterList.AppendFormat(",@RemovalDate: {0}", _removalDate);
            parameterList.AppendFormat(",@SecUser: {0}", _secondUser);
            parameterList.AppendFormat(",@SpInst: {0}", _specialInstructions);
            parameterList.AppendFormat(",@State: {0}", _state);
            parameterList.AppendFormat(",@Status: {0}", _Status);
            parameterList.AppendFormat(",@Zip: {0}", _zip);
            parameterList.AppendFormat(",@Zip4: {0}", _zipplus4);
            parameterList.AppendFormat(",@OnlineSince: {0}", _onlineSince);
            parameterList.AppendFormat(",@Type: {0}", _sub_type);
            parameterList.AppendFormat(",@Equip_Id: {0}", _unit_id);
            parameterList.AppendFormat(",@Serial_No: {0}", _serial_no);
            parameterList.AppendFormat(",@Model: {0}", _equipModelText);
            parameterList.AppendFormat(",@System: {0}", system);
            parameterList.AppendFormat(",@MasterRef: {0}", _MasterReference);
            parameterList.AppendFormat(",@Gender: {0}", _gender);
            parameterList.AppendFormat(",@Mobile: {0}", _mobile);
            #endregion

            try
            {
                
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@SubscriberId", _Subscriber_ID));
                _sqlParms.Add(SQLParam.GetParam("@Address1", _addr1));
                _sqlParms.Add(SQLParam.GetParam("@Address2", _addr2));
                _sqlParms.Add(SQLParam.GetParam("@AgencyID", _AgencyCode));
                _sqlParms.Add(SQLParam.GetParam("@AgencyName", _AgencyName));
                _sqlParms.Add(SQLParam.GetParam("@City", _city));
                _sqlParms.Add(SQLParam.GetParam("@DOB", _dob));
                _sqlParms.Add(SQLParam.GetParam("@EntryDate", _entryDate));
                _sqlParms.Add(SQLParam.GetParam("@FName", _firstname));
                _sqlParms.Add(SQLParam.GetParam("@InstallDate", _installDate));
                _sqlParms.Add(SQLParam.GetParam("@MI", _mi));
                _sqlParms.Add(SQLParam.GetParam("@LName", _lastname));
                _sqlParms.Add(SQLParam.GetParam("@NearInters", _nearestIntersection));
                _sqlParms.Add(SQLParam.GetParam("@OrderDate", _orderDate));
                _sqlParms.Add(SQLParam.GetParam("@Phone", _phone));
                _sqlParms.Add(SQLParam.GetParam("@RemovalDate", _removalDate));
                _sqlParms.Add(SQLParam.GetParam("@SecUser", _secondUser));
                _sqlParms.Add(SQLParam.GetParam("@SpInst", _specialInstructions));
                _sqlParms.Add(SQLParam.GetParam("@State", _state));
                _sqlParms.Add(SQLParam.GetParam("@Status", _Status));
                _sqlParms.Add(SQLParam.GetParam("@Zip", _zip));
                _sqlParms.Add(SQLParam.GetParam("@Zip4", _zipplus4));
                _sqlParms.Add(SQLParam.GetParam("@OnlineSince", _onlineSince));

                if(system == "PNC")
                    _sqlParms.Add(SQLParam.GetParam("@SubType", _type));
                else
                    _sqlParms.Add(SQLParam.GetParam("@SubType", _sub_type));
                
                _sqlParms.Add(SQLParam.GetParam("@Equip_Id",_unit_id));
                _sqlParms.Add(SQLParam.GetParam("@Serial_No", _serial_no));
                _sqlParms.Add(SQLParam.GetParam("@Model", _equipModelText));
                _sqlParms.Add(SQLParam.GetParam("@System", system));
                _sqlParms.Add(SQLParam.GetParam("@MasterRef", _MasterReference));
                _sqlParms.Add(SQLParam.GetParam("@Gender", _gender));
                _sqlParms.Add(SQLParam.GetParam("@Mobile", _mobile));

                _kshemaProvider.ExecuteNonQuery("SYNC_Update_Subscriber", _sqlParms);//_stageProvider.ExecuteNonQuery("SYNC_Update_Subscriber", _sqlParms);
                

                EntityMessage(string.Format("Subscriber Id {0} updated. {1}", _Subscriber_ID,parameterList),"INFO");
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Failed to update subscriber Id {0}. ERROR: {1}. {2}", _Subscriber_ID,ex.Message,parameterList),"ERROR");
            }
        }
                      
        protected string Map_Subscriber_Resident(int entity_ref)
        {
            string processType = "INSERT";
            string msg;

            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@locdef", -1));
                this._sqlParms.Add(SQLParam.GetParam("@resdef", entity_ref));
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", "-1"));
                DataRow dr = _stageProvider.ExecuteDataSet("SYNC_Map_Subscriber_Resident", _sqlParms).Tables[0].Rows[0];
                _Subscriber_ID = dr["Subscriber_ID"].ToString();//_stageProvider.ExecuteScalar<string>("SYNC_Map_Subscriber_Resident", _sqlParms);
                _MasterReference = dr["MasterReference"].ToString();

                if (_Subscriber_ID != "-1")
                {
                    processType = "UPDATE";
                    msg = string.Format("Subscriber Id {0} found for resident_def {1}", _Subscriber_ID, entity_ref.ToString());
                }
                else
                    EntityMessage(string.Format("No Subscriber Id found for resident_def {0}", entity_ref.ToString()),"INFO");
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Failed to map subsciber to resident for resident_def {0}. Error: {1}", entity_ref.ToString(), ex.Message),"ERROR");
                processType = "ERROR";
            }
            return processType;
        }

        protected void Get_Model_Info()
        {
            /*
             * select *--top 1 e.Serial_No,em.Text as [Model]
from PNC_KSHEMA..CONTROLCENTRE.EQUIPMENT e
 left join PNC_KSHEMA..CONTROLCENTRE.EQUIP_MODELS em on e.equip_model_ref = em.equip_model_def
where e.ident = 88204581
and e.status = 'Active'
and e.TRIGGER_ID is null
order by e.Installation_date desc
             * 
             * */

            StringBuilder sql = new StringBuilder();
            try
            {
                //this should only get teh base units
                sql.Append("select top 1 e.Serial_No,em.Text as [Model]");
                sql.Append(" from CONTROLCENTRE.EQUIPMENT e");
                sql.Append(" left join CONTROLCENTRE.EQUIP_MODELS em on e.equip_model_ref = em.equip_model_def");
                sql.AppendFormat(" where e.ident = {0}", this._unit_id.ToString());
                sql.Append(" and e.status = 'Active'");
                sql.Append(" and (e.TRIGGER_ID <> e.Serial_No)");
                sql.Append(" order by e.Installation_date desc");

                DataTable dtUnit = this._pncProvider.ExecuteDataSet(sql.ToString(), null).Tables[0];
                if (dtUnit.Rows.Count > 0)
                {
                    this._serial_no = dtUnit.Rows[0]["Serial_No"].ToString();
                    this._equipModelText = dtUnit.Rows[0]["Model"].ToString();

                    this.EntityMessage(string.Format("Found serial # {0}, model {1} for location_def {2}", this._serial_no, this._equipModelText, this._locationDef.ToString()), "INFO");
                }
                else
                    this.EntityMessage(string.Format("No serial number or model found for location_def {0}", this._locationDef.ToString()), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Error locating serial no for location_def {0}. ERROR: {1}. Statement: {2}"
                    , this._locationDef, ex.Message, sql.ToString()), "ERROR");

            }
        }
        
        protected void Update_Subscriber_Equipment()
        {
            // first get the equipment
            DataTable equipment_to_process = null;
            int equipCount = 0;
            try
            {
                #region Old
                //this._sqlParms = new List<SQLParam>();
                //this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._Subscriber_ID));
                //this._sqlParms.Add(SQLParam.GetParam("@service", "PNC"));
                //this._sqlParms.Add(SQLParam.GetParam("@location_def", this._locationDef));
                //this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._residentDef));
                //this._sqlParms.Add(SQLParam.GetParam("@sub_type", this._residentDef));
                //equipment_to_process = this._stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Equipment", this._sqlParms).Tables[0];
                //equipCount = equipment_to_process.Rows.Count;
                #endregion

                #region Query
                /*
                 * Select 
			e.equip_def
			,cast(case em.Manufacturer_ref
				when 100000032 then 'AMAC'
				else 'TUNSTALL'
			end as nvarchar(10)) as [Manufacturer]
			,cast(
				(case em.type_ref
						when 100000001 then 'Pendant'
						when 100000002 then 'Services'
						when 100000003 then 'Sensors'
						else 'Base'
				end) 				
			 as nvarchar(10)) as [Model_Type]
			,em.equip_model_def
			,em.text as [Model]
			,e.Ident as [Unit_Id]
			,e.Serial_No
			,e.Trigger_Id
			,coalesce((Select top 1 reason 
				from PNC_KSHEMA..CONTROLCENTRE.EQUIP_HISTORY 
				where equip_ref = e.equip_def and Event = 99
				order by occurred desc),'-1') as [SubscriberEquipment_Id]
			,case em.Manufacturer_ref
				when 100000032 then
					case e.Status
						when 'Active' then 'On Line'
						when 'Removed' then 'Recovered'
						when 'Canceled' then 'Canceled'
						when 'Pending Install' then 'Pending Install'
						else 'UnRecovered'
					end 
				else e.Status
			 end as [Status]
			,e.Installation_Date as [InstallationDate]
			,e.Warranty_Exp as [Requested_Install_Date]
			,e.Maintenance_Exp as [Removal_Date]
			,e.Battery_Exp as [Requested_Removal_Date]
		from PNC_KSHEMA..CONTROLCENTRE.EQUIPMENT e
		join PNC_KSHEMA..CONTROLCENTRE.EQUIP_ALLOC_HIST ea on ea.equip_ref = e.equip_def
		join PNC_KSHEMA..CONTROLCENTRE.EQUIP_MODELS em on em.equip_model_def = e.equip_model_ref
				
		where ea.loc_entity_ref = @location_def
		and ea.owner_entity_ref = @resident_def
		and e.Deleted = 'N'
                 * 
                 * */
                #endregion

                StringBuilder sql = new StringBuilder();
                sql.Append("Select e.equip_def");
                sql.Append(",cast(case em.Manufacturer_ref when 100000032 then 'AMAC' else 'TUNSTALL' end as nvarchar(10)) as [Manufacturer]");
                sql.Append(",cast((case em.type_ref	when 100000001 then 'Pendant' when 100000002 then 'Services' when 100000003 then 'Sensors' else 'Base' end)	as nvarchar(10)) as [Model_Type]");
                sql.Append(",em.equip_model_def,em.text as [Model],e.Ident as [Unit_Id],e.Serial_No, e.Trigger_Id");
                sql.Append(",coalesce((Select top 1 reason from EQUIP_HISTORY where equip_ref = e.equip_def and Event = 99 order by occurred desc),'-1') as [SubscriberEquipment_Id]");
                sql.Append(",case em.Manufacturer_ref when 100000032 then case e.Status when 'Active' then 'On Line' when 'Removed' then 'Recovered' when 'Canceled' then 'Canceled' when 'Pending Install' then 'Pending Install' else 'Unrecovered' end else e.Status end as [Status]");
                sql.Append(",e.Installation_Date as [InstallationDate],e.Warranty_Exp as [Requested_Install_Date],e.Maintenance_Exp as [Removal_Date],e.Battery_Exp as [Requested_Removal_Date]");
                sql.Append(" from EQUIPMENT e");
                sql.Append(" join EQUIP_ALLOC_HIST ea on ea.equip_ref = e.equip_def");
                sql.Append(" join EQUIP_MODELS em on em.equip_model_def = e.equip_model_ref");
                sql.AppendFormat(" where ea.loc_entity_ref = {0}", this._locationDef);
                sql.AppendFormat(" and ea.owner_entity_ref = {0}", this._residentDef);
                sql.Append(" and e.Deleted = 'N'");

                equipment_to_process = this._pncProvider.ExecuteDataSetQuery(sql.ToString(), null).Tables[0];
                equipCount = equipment_to_process.Rows.Count;
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("No equipment to process in PNC for Subscriber {0}. Reason: {1}", this._Subscriber_ID, ex.Message),"INFO");
            }

            if (equipCount > 0)
            {

                this.EntityMessage(string.Format("Processing {0} rows for equipment for Subscriber {1}.", equipCount,this._Subscriber_ID), "INFO");

                // if its AMAC, delete all equipment and just do inserts
                // this needs to be done usig the padding table
                this.Reset_Subscriber_Tunstall_Equipment();


                foreach (DataRow dr in equipment_to_process.Rows)
                {

                    string model = dr["Model"].ToString().Trim();
                    string manufacturer = dr["Manufacturer"].ToString();
                    string trigger_id = string.Empty;
                    if(!string.IsNullOrEmpty(dr["Trigger_Id"].ToString()))
                        trigger_id = dr["Trigger_Id"].ToString();

                    string model_type = dr["Model_Type"].ToString();
                    string unit_id = dr["Unit_Id"].ToString();
                    string serial_no = dr["Serial_No"].ToString();
                    string status = dr["Status"].ToString();
                    int equip_def  = dr["equip_def"].Parse<int>();

                    try
                    {
                        this._sqlParms = new List<SQLParam>();
                        this._sqlParms.Add(SQLParam.GetParam("@manufacturer", manufacturer));       
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._Subscriber_ID));
                        this._sqlParms.Add(SQLParam.GetParam("@subscriber_equipment_id", dr["SubscriberEquipment_Id"].ToString()));
                        this._sqlParms.Add(SQLParam.GetParam("@model", model));
                        this._sqlParms.Add(SQLParam.GetParam("@model_type", model_type));
                        this._sqlParms.Add(SQLParam.GetParam("@unit_id", unit_id));
                        this._sqlParms.Add(SQLParam.GetParam("@trigger_id", trigger_id));
                        this._sqlParms.Add(SQLParam.GetParam("@serial_no", serial_no));

                        if (!string.IsNullOrEmpty(dr["InstallationDate"].ToString()))
                        {
                            this._sqlParms.Add(SQLParam.GetParam("@installation_date", dr["InstallationDate"].ToString()));
                        }

                        if (!string.IsNullOrEmpty(dr["Requested_Install_Date"].ToString()))
                            this._sqlParms.Add(SQLParam.GetParam("@warranty_exp", dr["Requested_Install_Date"].ToString()));

                        if (!string.IsNullOrEmpty(dr["Requested_Removal_Date"].ToString()))
                            this._sqlParms.Add(SQLParam.GetParam("@battery_exp", dr["Requested_Removal_Date"].ToString()));

                        if (!string.IsNullOrEmpty(dr["Removal_Date"].ToString()))
                            this._sqlParms.Add(SQLParam.GetParam("@maintenance_exp", dr["Removal_Date"].ToString()));

                        this._sqlParms.Add(SQLParam.GetParam("@status",status ));
                        this._sqlParms.Add(SQLParam.GetParam("@equip_def",equip_def));
                        this._kshemaProvider.ExecuteNonQuery("SYNC_Save_SubscriberEquipment", this._sqlParms);

                        this.EntityMessage(string.Format("Successfully saved equipment model {0} for Subscriber {1}.", model, this._Subscriber_ID), "INFO");


                        
                        ///if model name = iVi then insert optional service 33
                        if (model.ToLower() == "i v i -trigger" && (status.ToLower() == "on line" || status.ToLower() == "active"))
                        {
                            this.Set_Subscriber_Optional_Service("INSERT", 33);
                        }
                        else if (model.ToLower() == "i v i -trigger" && (status.ToLower() == "recovered" || status.ToLower() == "unrecovered" || status.ToLower() == "removed"))
                        {
                            this.Set_Subscriber_Optional_Service("DELETE", 33);
                        }

                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to save equipment model {0} for Subscriber {1}. ERROR: {2}", model,this._Subscriber_ID,ex.Message), "INFO");
                    }                    
                }
            }
        }

        protected void Add_Record_To_PNC_Log()
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO CONTROLCENTRE.ETL_LOG_CUD(ENTITY_TABLE,ENTITY_TYPE,ENTITY_REF,UPDATE_TIME,UPDATE_TYPE)");
                sql.AppendFormat("VALUES('RESIDENT',5,{0},GETDATE(),'U')", this._residentDef.ToString());
                this._pncProvider.ExecuteNonSPQuery(sql.ToString(), null);

                //this.EntityMessage(string.Format("Successfully added an update for resident {0}, subscriber {1} to the PNC ETL Log."
                //    , this._resident_def.ToString(), this._subscriber_id), "INFO");
            }
            catch (Exception ex)
            {
                //this.EntityMessage(string.Format("Failed to add an update for resident {0}, Subscriber {1} to the PNC ETL Log."
                //    , this._resident_def.ToString(), this._subscriber_id, ex.Message), "INFO");
            }
        }

        #region Deprecated
        protected bool Get_Subscriber_Type()
        {
            bool isMatch = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@subscriber_id", _Subscriber_ID));
                _previous_type = _stageProvider.ExecuteScalar<string>("SYNC_Get_Subscriber_Type", _sqlParms);
                EntityMessage(string.Format("Previous Type {0} : Current Type {1} found for subscriber {2}", _previous_type, _type, _Subscriber_ID), "INFO");
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Failed to get subscriber type for Subscriber Id {0}.ERROR: {1}", _Subscriber_ID, ex.Message), "INFO");
                isMatch = false;
            }

            return isMatch;
        }
               
        protected void Process_Subscriber_Type()
        {
            #region v 1.3.1
            ////Transferred from any to Cell 450: Type != Prev_Type && Type == "Cellular" && Prev_Type != "MSD"
            //if (_type != _previous_type && _type == "Cellular" && _previous_type != "MSD")
            //{
            //    this.Set_Subscriber_Optional_Service("INSERT", 32);
            //    return;
            //}

            ////Transferred from any to MSD:Type != Prev_Type && Type == "MSD" && Prev_Type != "Cellular"
            //if (_type != _previous_type && _type == "MSD" && _previous_type != "Cellular")
            //{
            //    if(this._equipModelRef == 92)
            //        this.Set_Subscriber_Optional_Service("INSERT", 34);
            //    else
            //        this.Set_Subscriber_Optional_Service("INSERT", 41);

            //    return;
            //}

            ////Transferred from Cell 450 to other: Type != Prev_Type && Prev_Type == "Cellular" && Type != "MSD"
            //if (_type != _previous_type && _previous_type == "Cellular" && _type != "MSD")
            //{
            //    this.Set_Subscriber_Optional_Service("DELETE", 32);
            //    return;
            //}

            ////Transferred from MSD to other: Type != Prev_Type && Prev_Type == "MSD" && Type != "Cellular"
            //if (_type != _previous_type && _previous_type == "MSD" && _type != "Cellular")
            //{
            //    if(this._equipModelRef == 92)
            //        this.Set_Subscriber_Optional_Service("DELETE", 34);
            //    else
            //        this.Set_Subscriber_Optional_Service("DELETE", 41);

            //    return;
            //}

            ////Transferred from Cell 450 to MSD: Prev_Type == "Cellular" && Type == "MSD"
            //if (_previous_type == "Cellular" && _type == "MSD")
            //{
            //    if(this._equipModelRef == 92)
            //        this.Set_Subscriber_Optional_Service("INSERT", 34);
            //    else
            //        this.Set_Subscriber_Optional_Service("INSERT", 41);


            //    this.Set_Subscriber_Optional_Service("DELETE", 32);
            //    return;
            //}

            ////Transferred from  MSD to Cell 450: Prev_Type == "MSD" && Type == "Cellular"
            //if (_previous_type == "MSD" && _type == "Cellular")
            //{
            //    this.Set_Subscriber_Optional_Service("INSERT", 32);

            //    if(this._equipModelRef == 92)
            //        this.Set_Subscriber_Optional_Service("DELETE", 34);
            //    else
            //        this.Set_Subscriber_Optional_Service("DELETE", 41);

            //    return;
            //}

            //if (_type != _previous_type && _type == "MSD" && _previous_type == "PNC")
            //{
            //    if(this._equipModelRef == 92)
            //        this.Set_Subscriber_Optional_Service("INSERT", 34);
            //    else
            //        this.Set_Subscriber_Optional_Service("INSERT", 41);

            //    return;
            //}

            //if (_type != _previous_type && _type == "Cellular" && _previous_type == "PNC")
            //{
            //    this.Set_Subscriber_Optional_Service("INSERT", 32);
            //    return;
            //}
            #endregion

            this.Set_SubscriberPL2Info(_type);
        }
        #endregion

        protected void Set_SubscriberPL2Info(string modelType)
        {
            StringBuilder sql = new StringBuilder();

            try
            {
                #region "Old"
                //_sqlParms = new List<SQLParam>();
                //_sqlParms.Add(SQLParam.GetParam("@subscriber_id", _Subscriber_ID));
                //_sqlParms.Add(SQLParam.GetParam("@type", modelType));
                //_sqlParms.Add(SQLParam.GetParam("@other_phone", _otherPhone));
                //_stageProvider.ExecuteNonQuery("SYNC_Update_Subscriber_Type_SubscriberPL2Info", _sqlParms);


                ////update the sub type
                //sql.Append("UPDATE [Subscriber] SET [Type] = '@type' WHERE [Subscriber_ID] = '@subscriber_id'");
                //sql.Replace("@type", modelType);
                //sql.Replace("@subscriber_id", _Subscriber_ID);
                //_kshemaProvider.ExecuteNonSPQuery(sql.ToString(),null);
                #endregion

                #region " V1.1


                #endregion


                sql = new StringBuilder();
                sql.Append("UPDATE [SubscriberPL2info] ");

                switch (modelType)
                {
                    case "Cellular":
                        sql.AppendFormat("SET [NoLandlineAvailable] = '{0}', [RequestMSD] = '{1}'", "Yes", "No");

                        if (!string.IsNullOrEmpty(_otherPhone))
                            sql.AppendFormat(",  [MSDPhone] = '{0}'",_otherPhone);

                        break;

                    case "MSD":
                        sql.AppendFormat("SET [NoLandlineAvailable] = '{0}', [RequestMSD] = '{1}'", "No", "Yes");
                        sql.AppendFormat(",  [MSDPhone] = '{0}'", _mobile);

                        break;

                    case "(PNC)":
                    case "DSPNET":
                        sql.AppendFormat("SET [NoLandlineAvailable] = '{0}', [RequestMSD] = '{1}', [MSDPhone] = {2}", "No", "No", "NULL");
                        break;
                }

                sql.AppendFormat(" WHERE [Subscriber_ID] = {0}", _Subscriber_ID);

                #region " Old "
                //sql.Append("UPDATE [SubscriberPL2info] SET [NoLandlineAvailable] = '@no_land_line',[RequestMSD] = '@request_msd',[MSDPhone] =  '@other_phone' WHERE [Subscriber_ID] = '@subscriber_id'");

                //switch (modelType)
                //{
                //    case "Cellular":
                //        sql.Replace("@no_land_line", "Yes");
                //        sql.Replace("@request_msd", "No");
                //        sql.Replace("@other_phone", "NULL");
                //        break;

                //    case "MSD":
                //        sql.Replace("@no_land_line", "No");
                //        sql.Replace("@request_msd", "Yes");
                //        sql.Replace("@other_phone", _otherPhone);
                //        break;

                //    case "DSPNET":
                //        sql.Replace("@no_land_line", "No");
                //        sql.Replace("@request_msd", "No");
                //        sql.Replace("@other_phone", "NULL");
                //        break;

                //    case "(PNC)":
                //        sql.Replace("@no_land_line", "No");
                //        sql.Replace("@request_msd", "No");
                //        sql.Replace("@other_phone", "NULL");
                //        break;
                //}

                //sql.Replace("@subscriber_id", _Subscriber_ID);
                #endregion

                _kshemaProvider.ExecuteNonSPQuery(sql.ToString(), null);


                this.EntityMessage(string.Format("Setting Subscriber_ID {0} type to {1}", _Subscriber_ID, modelType), "INFO");
            }
            catch (Exception ex)
            {
                string err_message = string.Format("Error setting Subscriber_ID {0} type to {1}, ERROR: {2}. QUERY: {3}", _Subscriber_ID, modelType, ex.Message,sql.ToString());
                this.EntityMessage(err_message, "ERROR");
                this.Save_Transaction_Log(this._group_record, "EPEC", sql.ToString(), err_message);
            }
        }

        protected void Set_Subscriber_Optional_Service(string command, int optionalService)
        {
            StringBuilder sql = new StringBuilder();
            try
            {
                if (command == "INSERT")
                {
                    sql.Append("INSERT INTO  [SubscriberOptionalService] (Subscriber_Id, OptionalService_ID)	VALUES ( '@subscriber_id' , @optionalService)");
                }
                else if (command == "DELETE")
                {
                    sql.Append("delete from [SubscriberOptionalService] where subscriber_id = '@subscriber_id' and [OptionalService_ID] =@optionalService");
                }
                sql.Replace("@subscriber_id", _Subscriber_ID);
                sql.Replace("@optionalService", optionalService.ToString());

                _kshemaProvider.ExecuteNonSPQuery(sql.ToString(), null);

                this.EntityMessage(string.Format("Subscriber_ID {0} performing an {1} on optional service {2}", _Subscriber_ID, command, optionalService.ToString()), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to set an {0} on optional service {1} for subscriber_id {2}. ERROR: {3}", command, optionalService.ToString(), _Subscriber_ID, ex.Message), "INFO");
            }
        }

        public virtual void ProcessDataDelete() { }
        #endregion
    }
}

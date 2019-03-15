using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace Kshema_PNC.BL.Entity
{
    class Equipment : EntityBase
    {
        #region " Member Variables "
        string _subscriberId;
        long _locationDef;
        long _residentDef;
        long _equipModelRef;
        int _Id;
        long _equipRef;
        string _subType;
        string _unitId;
        string _subName;
        string _serialNo;
        string _triggerId;
        string _status;
        string _installDate;
        string _batteyDate;
        string _warrantyDate;
        string _maintenanceDate;

        #endregion
        
        #region " Constructor "
        public Equipment()
        {
            this.InitializeProviders();
        }
        #endregion

        #region " Private Methods "
        private bool InsertEquipment()
        {
            StringBuilder sql = null;
            bool isSuccess = true;
            sql = new StringBuilder();
            sql.Append("insert into  CONTROLCENTRE.EQUIPMENT(SERVICE_INTERVAL_NO, EQUIP_MODEL_REF, IDENT, STATUS, EQUIP_COMMENT, SERIAL_NO,TRIGGER_ID");

            if (!string.IsNullOrEmpty(this._installDate))
                sql.Append(",INSTALLATION_DATE)");
            else
                sql.Append(");");


            sql.AppendFormat(" values(0,{0},{1},'{2}','{3}','{4}','{5}'"
                , this._equipModelRef, this._unitId, this._status, this._Id.ToString(), this._serialNo, this._triggerId);

            if (!string.IsNullOrEmpty(this._installDate))
                sql.AppendFormat(",cast('{0}' as date));", this._installDate);
            else
                sql.Append(");");

            sql.AppendFormat("select EQUIP_DEF from CONTROLCENTRE.EQUIPMENT where equip_comment = '{0}';", this._Id.ToString());

            try
            {
                this._equipRef = this._pncProvider.ExecuteDataSetQuery(sql.ToString(), null).Tables[0].Rows[0]["EQUIP_DEF"].Parse<long>();
                this.EntityMessage(string.Format("Inserted equipment model ref {0} for subscriber {1}.",this._equipModelRef,this._subscriberId), "INFO");
            }
            catch(Exception ex)
            {
                isSuccess = false;
                this.EntityMessage(string.Format("Failed to get equip_def for subscriber Id {0}, SQL: {1}. ERROR: {2}"
                    , this._subscriberId, sql.ToString(), ex.Message), "INFO");
            }

            //if we dont have the equipmentDef then we cannot continue
            if (this._equipRef != -1)
            {
                sql.Clear();
                sql.Append("insert into CONTROLCENTRE.EQUIP_ALLOC_HIST(EQUIP_REF,VALID_FROM, VALID_TO, LOC_ENTITY_TYPE, LOC_ENTITY_REF, LOC_DESC, OWNER_ENTITY_TYPE, OWNER_ENTITY_REF, OWNER_DESC)");
                sql.AppendFormat(" values({0}, cast('{1}' as date), cast('2038-12-31' as date), 2, {2}, '{3}', 5, {4}, '{5}'); "
                    , this._equipRef, this._installDate, this._locationDef, string.Format("Unit: {0}", this._unitId), this._residentDef.ToString(), this._subName);

                sql.Append("Insert into CONTROLCENTRE.EQUIP_HISTORY(EQUIP_REF,OCCURRED,EVENT,ACCESS_REF,REASON)");
                sql.AppendFormat(" values({0}, GETDATE(), 99, 100000061, '{1}'); ",this._equipRef, this._Id.ToString());

                sql.AppendFormat("update CONTROLCENTRE.EQUIPMENT set EQUIP_COMMENT = NULL where EQUIP_DEF = {0};", this._equipRef);

                try
                {
                    this._pncProvider.ExecuteNonSPQuery(sql.ToString(), null);
                    this.EntityMessage(string.Format("Inserted equipment history for subscriber {0}, model ref {1}.",  this._subscriberId,this._equipModelRef), "INFO");
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    this.EntityMessage(string.Format("Failed to insert to equipment history tables for subscriber Id {0}. SQL: {1}. ERROR: {2}"
                    , this._subscriberId, sql.ToString(), ex.Message), "INFO");
                }
            }

            return isSuccess;
        }

        private void UpdateEquipment()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("update CONTROLCENTRE.EQUIPMENT");
            sql.AppendFormat(" set STATUS = '{0}' ", this._status);
            sql.AppendFormat(",IDENT = {0}", this._unitId);
            sql.AppendFormat(",EQUIP_MODEL_REF	= {0}", this._equipModelRef);
            sql.AppendFormat(",SERIAL_NO  = '{0}'", this._serialNo);
            sql.AppendFormat(",TRIGGER_ID = '{0}'", this._triggerId);
            sql.AppendFormat(",INSTALLATION_DATE = cast('{0}' as date)", this._installDate);

            if (!string.IsNullOrEmpty(this._batteyDate))
                sql.AppendFormat(",BATTERY_EXP = cast('{0}' as date)", this._batteyDate);

            if (!string.IsNullOrEmpty(this._warrantyDate))
                sql.AppendFormat(",WARRANTY_EXP = cast('{0}' as date)", this._warrantyDate);

            if (!string.IsNullOrEmpty(this._maintenanceDate))
                sql.AppendFormat(",MAINTENANCE_EXP	= cast('{0}' as date)", this._maintenanceDate);


            sql.AppendFormat(" where equip_def = {0};", this._equipRef);


            sql.Append(" update CONTROLCENTRE.EQUIP_ALLOC_HIST");
            sql.AppendFormat(" set LOC_ENTITY_REF = {0}", this._locationDef);
            sql.AppendFormat(",OWNER_ENTITY_REF = {0}", this._residentDef);
            sql.AppendFormat(",LOC_DESC = 'Unit: {0}'", this._unitId);
            sql.AppendFormat(",OWNER_DESC = '{0}'", this._subName);
            sql.AppendFormat(" where equip_ref = {0};", this._equipRef);

            try
            {
                this._pncProvider.ExecuteNonSPQuery(sql.ToString(), null);
                this.EntityMessage(string.Format("Updated equipment for Subscriber {0}, Model {1}",this._subscriberId,this._equipModelRef), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to update to equipment history tables for subscriber Id {0}. SQL: {1}. ERROR: {2}"
               , this._subscriberId, sql.ToString(), ex.Message), "INFO");
            }
        }

        private void UpdatePNCEquipmentTable()
        {
            try
            {
                string sql = string.Format("update [ks_PNC_EQUIPMENT] set PNC_EQUIP_REF = {0} where Id = {1}", this._equipRef, this._Id);
                _stageProvider.ExecuteNonSPQuery(sql, null);
                this.EntityMessage(string.Format("Updated the PNC_EQUIPMENT record {0} with Equip_Ref {1} for subscriber {0}",this._Id,this._equipRef,this._subscriberId), "INFO");
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Failed to update PNC_EQUIPMENT table for subsriber Id {0}. ERROR: {1}",this._subscriberId,ex.Message),"ERROR");
            }
        }
        #endregion 

        #region " Static Methods "
        public static string Get_Temp_Unit_Id(string subscriber_id, long authority_ref)
        {
            //TODO: use YYMMDD + SubscriberId for temp unit id's
            //long unit_id = -1;

            return string.Format("{0}{1}", DateTime.Now.ToString("yyMMdd"), subscriber_id);
            //IDataProvider provider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
            //List<SQLParam> parms = new List<SQLParam>();

            //parms.Add(SQLParam.GetParam("@subscriber_id", subscriber_id));
            //parms.Add(SQLParam.GetParam("@authority_ref", authority_ref));
            //unit_id = provider.ExecuteScalar<long>("SYNC_Get_Temp_Unit_Id", parms);

            //return unit_id;
        }
        #endregion

        public void ProcessSubscriberEquipment(string subscriberID,long locationDef, long residentDef, string subType, string unitId, string subName)
        {
            this.EntityMessage(string.Format("Processing subscriber equipment for {0}",subscriberID),"INFO");
            this._subscriberId = subscriberID;
            this._locationDef = locationDef;
            this._residentDef = residentDef;
            this._subType = subType;
            this._unitId = unitId;
            this._subName = subName;

            string message = string.Empty;
            bool canProcess = true;
            StringBuilder sql = null;
            DataTable equipment_to_process = null;
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", subscriberID));
                this._sqlParms.Add(SQLParam.GetParam("@service", "KSHEMA"));
                this._sqlParms.Add(SQLParam.GetParam("@location_def", -1));
                this._sqlParms.Add(SQLParam.GetParam("@resident_def", -1));
                this._sqlParms.Add(SQLParam.GetParam("@sub_type", subType));
                equipment_to_process = this._stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Equipment", this._sqlParms).Tables[0];
                this.EntityMessage(string.Format("Retrieved {0} pieces of equipment for subscriber Id {1}, service {2}.", equipment_to_process.Rows.Count,subscriberID, "KSHEMA"), "INFO");
            }
            catch(Exception ex)
            { 
                this.EntityMessage(string.Format("Failed to get equipment for subscriber Id {0}, service {1}. ERROR: {2}", subscriberID,"KSHEMA",ex.Message), "ERROR");
                canProcess = false;
            }

            if (canProcess)
            {
                foreach(DataRow dr_to_process in equipment_to_process.Rows)
                {
                    try
                    {
                        this._Id = dr_to_process["equip_id"].Parse<int>();
                        this._equipRef = dr_to_process["equip_ref"].Parse<int>();
                        this._equipModelRef = dr_to_process["Equip_model_def"].Parse<long>();
                        this._serialNo = dr_to_process["serial_no"].ToString();
                        this._status = dr_to_process["status"].ToString();
                        this._triggerId = dr_to_process["trigger_id"].ToString().ToUpper();
                        this._installDate = string.Empty;
                        this._batteyDate = string.Empty;
                        this._warrantyDate = string.Empty;
                        this._maintenanceDate = string.Empty;

                        if (!string.IsNullOrEmpty(dr_to_process["installation_date"].ToString()))
                            this._installDate = dr_to_process["installation_date"].Parse<DateTime>().ToString("yyyy-MM-dd");
                        else
                            this._installDate = DateTime.Now.ToString("yyyy-MM-dd");

                        if (!string.IsNullOrEmpty(dr_to_process["battery_exp"].ToString()))
                            this._batteyDate = dr_to_process["battery_exp"].Parse<DateTime>().ToString("yyyy-MM-dd");

                        if (!string.IsNullOrEmpty(dr_to_process["warranty_exp"].ToString()))
                            this._warrantyDate = dr_to_process["warranty_exp"].Parse<DateTime>().ToString("yyyy-MM-dd");

                        if (!string.IsNullOrEmpty(dr_to_process["maintenance_exp"].ToString()))
                            this._maintenanceDate = dr_to_process["maintenance_exp"].Parse<DateTime>().ToString("yyyy-MM-dd");

                        //first check is on the sub type
                        if (subType == "MSD" | subType == "Cellular" | subType == "(PNC)")
                        {
                           if(this._equipRef == -1)
                           {
                               bool isSucces = this.InsertEquipment();
                               if (isSucces)
                                   this.UpdatePNCEquipmentTable();
                           }
                           else
                           {
                               this.UpdateEquipment();
                           }
                        }
                        else
                        {
                            DataTable dtEquipment = null;
                            int rowCount = 0;
                            sql = new StringBuilder();
                            try
                            {
                                sql.Append("select * from CONTROLCENTRE.EQUIPMENT e");
                                sql.Append(" join CONTROLCENTRE.EQUIP_HISTORY eh on eh.equip_ref = e.equip_def and eh.Event = 99");
                                sql.AppendFormat(" where eh.Reason = CAST({0} as nvarchar(25))", this._Id.ToString());

                                dtEquipment = this._pncProvider.ExecuteDataSetQuery(sql.ToString(), null).Tables[0];
                                rowCount = dtEquipment.Rows.Count;
                                this._equipRef = dtEquipment.Rows[0]["EQUIP_DEF"].Parse<long>();
                            }
                            catch(Exception ex)
                            {
                                this.EntityMessage(string.Format("Failed to check KSHEMA equipment for Subscriber Id {0}. ERROR: {1}", this._subscriberId, ex.Message), "INFO");
                            }

                            if (rowCount == 0)
                                this.InsertEquipment();
                            else
                                this.UpdateEquipment();

                        }
                    }
                    catch (Exception ex)
                    {
                        this.EntityMessage(string.Format("Failed to process equipment for Subscriber Id {0}. ERROR: {1}", subscriberID, ex.Message), "ERROR");
                    }    
                }
            }
        }

        public string GetPNCEquipId(string subscriber_id, long location_def)
        {
            StringBuilder sql = new StringBuilder();
            string equip_id = "-1";
            sql.AppendFormat("select equip_Id from EPEC_LOCATION where location_def = {0}", location_def.ToString());

            try
            {
                equip_id = _pncProvider.ExecuteDataSetQuery(sql.ToString(), null).Tables[0].Rows[0]["equip_id"].ToString();
                this.EntityMessage(string.Format("Found equip id {0} for subscriber {1} in epec_location {2}",equip_id,subscriber_id,location_def.ToString()),"INFO");
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Failed to get the equipment Id from epec_location for subscriber {0}, location_def {1}. ERROR: {2}",subscriber_id,location_def.ToString(),ex.Message), "ERROR");
            }

            return equip_id;
        }
    }
}

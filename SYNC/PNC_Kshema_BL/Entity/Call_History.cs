using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class Call_History : EntityBase, IEntity
    {
        #region  "Member Variables "
        long _call_def;
        long _reason_def;

        string _call_type;
        string _arrival_time;
        string _call_code;
        string _meaning;
        string _protocol_tag;
        string _reason;
        string _alarm_code;
        string _disposition;
        #endregion

        #region " Constructor "
        public Call_History(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();

        }
        #endregion

        #region " Private Methods "
        bool Map_Properties()
        {
            bool isMapped = true;

            try
            {
                this._call_def = this._dataToMapp["call_def"].Parse<long>();
                this._reason_def = this._dataToMapp["reason_def"].Parse<long>();

                this._locationDef = this._dataToMapp["location_def"].Parse<int>();
                this._residentDef = this._dataToMapp["resident_def"].Parse<int>();

                this._call_type = this._dataToMapp["Type"].ToString();
                this._call_code = this._dataToMapp["call_code"].ToString();
                this._unit_id = this._dataToMapp["equip_id"].ToString();
                this._phone = this._dataToMapp["equip_phone"].ToString();
                this._meaning = this._dataToMapp["meaning"].ToString();
                this._protocol_tag = this._dataToMapp["protocol_tag"].ToString();
                this._reason = this._dataToMapp["reason"].ToString();
                this._arrival_time = this._dataToMapp["arrival_time"].ToString();
            }
            catch (Exception ex)
            {
                isMapped = false;
                this.EntityMessage(string.Format("Failed to map properties for call_def {0}. Error: {1}", _dataToMapp["call_def"].ToString(), ex.Message), "ERROR");
            }

            return isMapped;
        }
        bool Map_Subscriber()
        {
            bool isMapped = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@locdef", this._locationDef));
                this._sqlParms.Add(SQLParam.GetParam("@resdef", this._residentDef));
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", "-1"));
                _Subscriber_ID = _stageProvider.ExecuteScalar<string>("SYNC_Map_Subscriber_Resident", _sqlParms);

                if (_Subscriber_ID != "-1")
                {
                    this.EntityMessage(string.Format("Subscriber Id {0} found for call_def {1}", this._Subscriber_ID, this._call_def),"INFO");
                }
                else
                {
                    this.EntityMessage(string.Format("No Subscriber Id found for call_def {0}", this._call_def), "INFO");
                    isMapped = false;
                }

            }
            catch (Exception ex)
            {
                isMapped = false;
                this.EntityMessage(string.Format("Failed to map subscriber for call_def {0}. Error: {1}", _dataToMapp["call_def"].ToString(), ex.Message), "ERROR");
            }

            return isMapped;
        }
        bool Can_Sync_Call_History()
        {
            bool can_sync = false;

            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._Subscriber_ID));
                can_sync = _stageProvider.ExecuteScalar<bool>("SYNC_Can_Sync_Call_History", _sqlParms);              
                
            }
            catch { }


            return can_sync;
        }
        bool Is_Kshema_SubType()
        {
            bool isKshema = false;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@subscriber_id", this._Subscriber_ID));
                string type = _stageProvider.ExecuteScalar<string>("SYNC_Get_Subscriber_Type", _sqlParms);

                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@sync_type", "KSHEMA"));
                _sqlParms.Add(SQLParam.GetParam("@type", type));
                int padding = _stageProvider.ExecuteDataSet("SYNC_Get_Unit_Padding", _sqlParms).Tables[0].Rows[0]["Padding"].Parse<int>();
                if (padding != -1)
                {
                    this._unit_id = (this._unit_id.Parse<long>() - padding).ToString("X");
                    isKshema = true;
                }
            }
            catch { }

            return isKshema;
        }
        bool Map_Alarm_Code()
        {
            bool isMapped = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@call_code", this._call_code));
                this._alarm_code = _stageProvider.ExecuteDataSet("SYNC_Get_Call_Code_Mapping", _sqlParms).Tables[0].Rows[0]["Alarm_Code"].ToString();

                this.EntityMessage(string.Format("Successfully mapped call_code {0} to alarm_code {1} for call_def {2}, Subscriber Id {3}.",
                    this._call_code, this._alarm_code, this._call_def.ToString(), this._Subscriber_ID), "INFO");
            }
            catch (Exception ex)
            {
                isMapped = false;
                this.EntityMessage(string.Format("Failed to map call_code {0} for call_def {1}, Subscriber Id {2}.",
                    this._call_code, this._call_def.ToString(), this._Subscriber_ID), "INFO");
            }

            return isMapped;
        }
        bool Map_Reason_Disposition()
        {
            bool ismapped = true;

            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@type", "PNC"));
                this._sqlParms.Add(SQLParam.GetParam("@reason", this._reason_def.ToString()));
                this._disposition = _stageProvider.ExecuteDataSet("SYNC_Get_PNC_Reason_PL_Disposition", _sqlParms).Tables[0].Rows[0]["Reason"].ToString();

                this.EntityMessage(string.Format("Successfully mapped reason_def {0} to disposition {1} for call_def {2}. Subscriber Id {3}",
                    this._reason_def, this._disposition, this._call_def.ToString(), this._Subscriber_ID), "INFO");
            }
            catch (Exception ex)
            {
                ismapped = false;
                this.EntityMessage(string.Format("Failed to map reason_def {0} to a disposition for call_def {1}. Subscriber Id {2}",
                    this._reason_def, this._call_def.ToString(), this._Subscriber_ID), "INFO");
            }

            return ismapped;
        }

        void Insert_Call()
        {
            try
            {
                _sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@intUser_ID", "1216"));
                this._sqlParms.Add(SQLParam.GetParam("@vchrUnit_ID", this._unit_id));
                this._sqlParms.Add(SQLParam.GetParam("@chrSubscriber_ID", this._Subscriber_ID));
                this._sqlParms.Add(SQLParam.GetParam("@chrType", "OTHER"));
                this._sqlParms.Add(SQLParam.GetParam("@vchrDisposition", this._disposition));
                this._sqlParms.Add(SQLParam.GetParam("@vchrComments", this._phone));
                this._sqlParms.Add(SQLParam.GetParam("@dteCallDateTime", this._arrival_time));
                this._sqlParms.Add(SQLParam.GetParam("@chrAlarmCode", this._alarm_code));
                this._sqlParms.Add(SQLParam.GetParam("@bintCallID", 0));
                this._kshemaProvider.ExecuteNonQuery("ap_Call_i", this._sqlParms);

                this.EntityMessage(string.Format("Successfully inserted call_def {0} for subscriber {1}.", this._call_def, this._Subscriber_ID), "INFO");
            }
            catch (Exception ex)
            {
                this.EntityMessage(string.Format("Failed to insert call_def {0} for subscriber {1}. Error: {2}", this._call_def,this._Subscriber_ID,ex.Message), "ERROR");
            }
        }
        #endregion

        #region " Interface Implementation "
        public void ProcessDataFlow()
        {
            bool process_next_step = Map_Properties();
            if (process_next_step)
            {
                if (this._call_type == "Home")
                {
                    //Check if its a Kshema Sub Type - only those call will go over
                    if (Map_Subscriber())
                    {
                        if (Is_Kshema_SubType())
                        {
                            if (Map_Alarm_Code())
                                if (Map_Reason_Disposition())
                                    this.Insert_Call();
                        }
                        else
                        {
                            this.EntityMessage(string.Format("The Subscriber {0} is not a mapped type to sync the call history", this._Subscriber_ID), "INFO");
                        }
                    }
                }
                else
                {
                    this.EntityMessage(string.Format("Unknown call history entity type for call_def {0}. Entity_Type: {1}", _dataToMapp["call_def"].ToString(),this._type), "INFO");
                }
            }
        }

        #endregion
    }
}

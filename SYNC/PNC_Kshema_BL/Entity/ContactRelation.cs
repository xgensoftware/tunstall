using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class ContactRelation : EntityBase, IEntity
    {
        #region " Member Variables "
        bool _has_Key;
        bool _NVI;

        int _nvi_Order;
        int _priority;
        int _relation_def;

        string _home_phone;
        string _work_phone;
        string _cell_phone;
        #endregion

        #region " Constructor "
        public ContactRelation(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();
        }
        #endregion

        #region " Private Methods "
        private bool Map_Contact_Relation_Fields()
        {
            bool isMapped = true;

            try
            {
                _contact_def = this._dataToMapp["contact_def"].Parse<int>();
                _contact_type_ref = this._dataToMapp["contact_type_ref"].Parse<int>();
                _relation_def = this._dataToMapp["relation_def"].Parse<int>();
                _locationDef = this._dataToMapp["location_ref"].Parse<int>();
                _residentDef = this._dataToMapp["resident_ref"].Parse<int>();
                _fullname = this._dataToMapp["ResponderName"].ToString();
                _has_Key = this._dataToMapp["HasKey"].Parse<bool>();
                _NVI = this._dataToMapp["NVI"].Parse<bool>();
                _nvi_Order = this._dataToMapp["NVIOrder"].Parse<int>();
                _priority = this._dataToMapp["Priority"].Parse<int>();

                if(!string.IsNullOrEmpty(this._dataToMapp["HomePhone"].ToString()))
                    _home_phone = this._dataToMapp["HomePhone"].ToString();

                if(!string.IsNullOrEmpty(this._dataToMapp["WorkPhone"].ToString()))
                    _work_phone = this._dataToMapp["WorkPhone"].ToString();

                if (!string.IsNullOrEmpty(this._dataToMapp["CellPhone"].ToString()))
                    _cell_phone = this._dataToMapp["CellPhone"].ToString();

            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Failed to map Contact Relation fields. ERROR: {0}", ex.Message),"ERROR");
                isMapped = false;
            }

            return isMapped;
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow()
        {
            bool processrelation = this.Map_Contact_Relation_Fields();
            if (processrelation)
            {
                try
                {
                    _sqlParms = new List<SQLParam>();
                    this._sqlParms.Add(SQLParam.GetParam("@process_type", "PNC"));
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber", "-1"));
                    this._sqlParms.Add(SQLParam.GetParam("@subscriber_responder", "-1"));
                    this._sqlParms.Add(SQLParam.GetParam("@location_ref", this._locationDef.ToString()));
                    this._sqlParms.Add(SQLParam.GetParam("@resident_ref", this._residentDef.ToString()));
                    this._sqlParms.Add(SQLParam.GetParam("@contact_ref", this._contact_def.ToString()));
                    this._sqlParms.Add(SQLParam.GetParam("@type_ref", this._contact_type_ref.ToString()));
                    this._sqlParms.Add(SQLParam.GetParam("@priority", this._priority));

                    _stageProvider.ExecuteNonQuery("SYNC_Process_ContactRelation", _sqlParms);
                    this.EntityMessage(string.Format("Processed contact relation for contact_def {0}, relation_def {1}", _contact_def.ToString(),_relation_def),"INFO");
                }
                catch (Exception ex)
                {
                    this.EntityMessage(string.Format("Unable to process contact relation for contact_def {0}, relation_def {1}", _contact_def,_relation_def),"ERROR");

                }
            }
        }
        #endregion
    }
}

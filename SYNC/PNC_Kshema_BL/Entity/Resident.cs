using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class Resident : EntityBase, IEntity
    {
        #region " Member Variables "
        int _attr_choice_def;
        #endregion

        
        #region " Constructor "
        public Resident(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();
           
        }
        #endregion

        #region " Private Methods "
        private void ProcessResidentTransaction()
        {
            string processType = "INSERT";
            bool isSuccess = true;

            processType = this.Map_Subscriber_Resident(_residentDef);
            if (processType != "ERROR")
            {
                //this.Check_Joined_Subscriber_Authority();
                if (this.PNC_EPEC_AGENCY_MAP())
                {
                    if (isSuccess)
                    {
                        switch (processType)
                        {
                            case "UPDATE":
                                //if (this.Get_Subscriber_Type())
                                //    this.Process_Subscriber_Type();
                                this.Set_SubscriberPL2Info(_type);
                                Update_Subscriber();
                                this.Update_Subscriber_Equipment();
                                break;

                            case "INSERT":
                                Insert_Temp_Subscriber_Q();
                                break;
                        }
                    }
                }
            }
            else
                isSuccess = false;
        }

        private bool Map_Resident_Characteristic()
        {
            bool isMatch = false;

            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@attr_choice_def", _attr_choice_def));
                _Status = _stageProvider.ExecuteScalar<string>("SYNC_Map_Resident_Status", _sqlParms);

                isMatch = true;
            }
            catch (Exception ex)
            {
                EntityMessage(string.Format("Failed to map resident status for resident_def {0}. Error: {1}", _residentDef.ToString(), ex.Message),"ERROR");
            }

            return isMatch;
        }

        private void Update_KShema_Resident()
        {
              
        }

        private bool MapResidentFields()
        {
            bool isMapped = true;

            try
            {
                _attr_choice_def = _dataToMapp["attr_choice_def"].Parse<int>();
            }
            catch { }

            return isMapped;
        }
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow()
        {
            bool processResident = false;
            bool processNextStep = this.MapProperties();

            this.Get_Model_Info();

            // map unique resident fields
            processNextStep = this.MapResidentFields();

            //Conditional Split on Primary_YN
            switch (_primary_YN.ToUpper())
            {
                case "Y":
                    if (PNC_EQUIP_STATUS_MAP())
                        processResident = true;
                    break;

                case "N":
                    if (Map_Resident_Characteristic())
                        processResident = true;

                    break;
            }

            if (processResident)
                ProcessResidentTransaction();
            
        }
        #endregion
    }
}

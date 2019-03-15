using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.BL.Entity
{
    class Epec : EntityBase, IEntity
    {
        #region " Member Variables "       
        
        #endregion

        #region " Constructor "
        public Epec(DataRow dr, string group_record)
        {
            _dataToMapp = dr;
            this._group_record = group_record;

            //intialize the providers
            base.InitializeProviders();
           
        }
        #endregion

        #region " Private Methods "
             

        

        private string PNC_EPEC_MAP()
        {
            string processType = "INSERT";
            
            try
            {
                _sqlParms = new List<SQLParam>();
                _sqlParms.Add(SQLParam.GetParam("@locdef", _locationDef));
                _sqlParms.Add(SQLParam.GetParam("@resdef", _residentDef));
                _sqlParms.Add(SQLParam.GetParam("@subscriber", "-1"));
                //_Subscriber_ID = _stageProvider.ExecuteScalar<string>("SYNC_Map_Subscriber_Location", _sqlParms);
                DataRow dr = _stageProvider.ExecuteDataSet("SYNC_Map_Subscriber_Location", _sqlParms).Tables[0].Rows[0];
                _Subscriber_ID = dr["Subscriber_ID"].ToString();
                _MasterReference = dr["MasterReference"].ToString();

                if (_Subscriber_ID != "-1")
                {
                    processType = "UPDATE";
                    EntityMessage(string.Format("Subscriber Id {0} found for location_def {1}", _Subscriber_ID, _locationDef.ToString()),"INFO");
                }
                else
                {
                    processType = "INSERT";
                    EntityMessage(string.Format("No Subscriber Id found for location_def {0}", _locationDef.ToString()), "INFO");
                        
                }
            }
            catch (Exception ex)
            {
                processType = "NONE";
                EntityMessage(string.Format("Failed to map subsciber to location for location_def {0}. Error: {1}", _locationDef.ToString(), ex.Message), "ERROR");
            }


            return processType;
        }
           

        //private void Update_Subscriber_Type()
        //{
        //    //Cellular: (!ISNULL(equip_model_ref) && equip_model_ref == 100000205)
        //    if (_equipModelRef == 100000205 | _equipModelRef == 61)
        //    {
        //        Set_SubscriberPL2Info("Cellular");
        //    }
        //    else if (_equipModelRef == 92)
        //    {
        //        //MSD: (!ISNULL(equip_model_ref) && equip_model_ref == 92)
        //        Set_SubscriberPL2Info("MSD");
        //    }
        //    else if ((_equipModelRef == 100000003 | _equipModelRef == 100000204))
        //    {
        //        //DSPNET: (!ISNULL(equip_model_ref) && (equip_model_ref == 100000003) || (equip_model_ref == 100000204))
        //        Set_SubscriberPL2Info("DSPNET");
        //    }
        //    else
        //    {
        //        //PNC: (!ISNULL(equip_model_ref) && ((equip_model_ref != 100000205) || (equip_model_ref != 92) || equip_model_ref != 100000003) || (equip_model_ref != 100000204))
        //        Set_SubscriberPL2Info("(PNC)");
        //    }
        //}    
              
        #endregion 

        #region " Public Methods "
        public void ProcessDataFlow()
        {
            bool processNextStep = this.MapProperties();             

        
            // get equipment model info
            this.Get_Model_Info();

            if (processNextStep)
            {
                string next_step = this.PNC_EPEC_MAP();
                //this.Check_Joined_Subscriber_Authority();
                switch (next_step)
                {
                    case "INSERT":                        

                        if (this.PNC_EPEC_AGENCY_MAP())
                            if (PNC_EQUIP_STATUS_MAP())
                            {
                                Insert_Temp_Subscriber_Q();

                                //this will handle the processing of equipment on
                                // new homes by inserting an update trigger for the resident process
                                this.Add_Record_To_PNC_Log();
                            }

                        break;


                    case "UPDATE":
                        if(this.PNC_EPEC_AGENCY_MAP())
                            if (PNC_EQUIP_STATUS_MAP())
                            {        
                                //if (this.Get_Subscriber_Type())
                                //    this.Process_Subscriber_Type();
                                this.Set_SubscriberPL2Info(_type);

                                this.Update_Subscriber();

                                this.Update_Subscriber_Equipment();
                            }
                        break;
                }                
            }

        }

        public static void Process_Epec_Stage_Queue()
        {
            IDataProvider pncStage = null;

            try
            {
                pncStage = DALFactory.CreateSqlProvider(DatabaseProvider_Type.MSSQLProvider, AppConfiguration.PNCStaging_Connection);
                pncStage.ExecuteNonQuery("TEMP_EPEC_INSERT_SUBSCRIBER", null);
            }
            catch { }

            if (pncStage != null)
                pncStage = null;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace Kshema_PNC.BL.Entity
{
    class SubscriberMedicalInfo : EntityBase, IEntity
    {
        #region " Member Variables "
        
        int _resident_ref = -1;
        int _keyword_num;
        int _keyword_def = -1;

        string _subscriber_Id;
        string _agency_id;
        StringBuilder _keyword_text = new StringBuilder();
        #endregion

        #region " Constructor "
        public SubscriberMedicalInfo(DataRow dr, string group_record)
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
                this._subscriber_Id = this._dataToProcess["Subscriber_Id"].ToString();
                this._resident_ref = this._dataToProcess["RESIDENT_REF"].Parse<int>();
                this._keyword_num = this._dataToProcess["KEYWORD_NO"].Parse<int>();
                this._keyword_def = this._dataToProcess["KEYWORD_DEF"].Parse<int>();
                this._keyword_text.Append(this._dataToProcess["KEYWORD_TEXT"].ToString());
                this._agency_id = this._dataToProcess["Agency_Id"].ToString();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                this.EntityMessage(string.Format("Failed to map properties for Subscriber {0}. Error: {1}", this._subscriber_Id, ex.Message),"ERROR");
            }

            return isSuccess;
        }

        bool Subscriber_Has_Keyword()
        {
            bool has_keyword = false;

            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@resident_def", this._resident_ref));
                this._sqlParms.Add(SQLParam.GetParam("@keyword_def",this._keyword_def));
                this._sqlParms.Add(SQLParam.GetParam("@keyword_no",this._keyword_num));
                has_keyword = this._stageProvider.ExecuteScalar<bool>("SYNC_Subscriber_Has_Keyword",this._sqlParms);
            }
            catch{}

            return has_keyword;
        }

        
        #endregion

        #region " Public Methods "
        public void ProcessDataFlow(string process)
        {
            bool isMapped = this.Map_Properties();
            if (isMapped)
            {
                isMapped = this.Check_Agency_Mapping(this._agency_id);
                if (isMapped)
                {
                    bool has_keyword = this.Subscriber_Has_Keyword();
                    if (!has_keyword)
                    {
                        // clean up before putting in the db
                        this._keyword_text.Replace("'", "''");
                        try
                        {

                            this._sql = new StringBuilder();
                            this._sql.Append("insert into CONTROLCENTRE.KEYWORD_RELATION(RESIDENT_REF,KEYWORD_NO,KEYWORD_REF,KEYWORD_TEXT)");
                            this._sql.AppendFormat("values({0},{1},{2},'{3}')", this._resident_ref.ToString(), this._keyword_num.ToString(), this._keyword_def.ToString(), this._keyword_text.ToString());
                            this._pncProvider.ExecuteNonSPQuery(this._sql.ToString(), null);

                            this.EntityMessage(string.Format("Successfully added keyword {0}: {1} for subscriber {2}", this._keyword_def.ToString(), this._keyword_text, this._subscriber_Id), "INFO");

                        }
                        catch (Exception ex)
                        {
                            string err_message = string.Format("Failed to insert keyword {0}: {1} for Subscriber {2}.ERROR: {3}", this._keyword_def.ToString(), this._keyword_text, this._subscriber_Id, ex.Message);
                            this.EntityMessage(err_message, "ERROR");
                            this._serviceRepository.Save_Transaction_Record(this._group_record, "SubscriberMedicalInfo", this._sql.ToString(), err_message);
                        }
                    }
                }
            }
        }
        #endregion
    }
}

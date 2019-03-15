using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace Kshema_PNC.BL.Entity
{
    class Subscriber_Mapping : EntityBase
    {
        #region " Member Variables "
        int _location_def = -1;
        int _resident_def = -1;
        int _authority_ref = -1;
        long _equip_id = -1;
        string _subscriber;
        #endregion

        #region " Properties "
        public int Location_Def
        {
            get { return _location_def; }
        }

        public int Resident_Def
        {
            get { return _resident_def; }
        }

        public int Authority_Ref
        {
            get { return _authority_ref; }
        }

        public string Subscriber_Id
        {
            get { return _subscriber; }
        }

        public long Equip_ID
        {
            get { return _equip_id; }
        }
        #endregion

        #region " Constructor "
        public Subscriber_Mapping()
        {
            this.InitializeProviders();
        }
        #endregion

        #region " Methods "
        public void Fetch(string subscriber)
        {
            try
            {
                this._sqlParms = new List<SQLParam>();
                this._sqlParms.Add(SQLParam.GetParam("@subscriber", subscriber));
                DataRow drMapping = _stageProvider.ExecuteDataSet("SYNC_Get_Subscriber_Mapping",this._sqlParms).Tables[0].Rows[0];
                _location_def = drMapping["Location_Ref"].Parse<int>();
                _resident_def = drMapping["Resident_Def"].Parse<int>();
                _authority_ref = drMapping["Authority_Ref"].Parse<int>();
                _equip_id = drMapping["Equip_Id"].Parse<long>();
                _subscriber = subscriber;
            }
            catch(Exception ex)
            {
                this.EntityMessage(string.Format("Failed to fetch the subscriber mapping for {0}",subscriber), "ERROR");
            }
        }
        #endregion
    }
}

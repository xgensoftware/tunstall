using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallBL.Models;
using TunstallBL.Helpers;
using TunstallDAL;

namespace TunstallBL.Services
{
    public class HomeService : BaseService<HomeService>, IDisposable
    {
        #region Private Methods

        EpecLocationModel SearchHomeByUnit(long unitId)
        {
            EpecLocationModel el = null;
            DataTable data = null;
            StringBuilder sql = new StringBuilder();

            sql.AppendFormat("SELECT * FROM EPEC_LOCATION WHERE EQUIP_ID = {0}", unitId);
            try
            {
                data = _provider.GetData(sql.ToString(), null);
                LogInfo(string.Format("Running: {0}", sql.ToString()));
            }
            catch (Exception e)
            {
                LogError(e);
            }

            if (data != null && data.Rows.Count > 0)
            {
                try
                {
                    var row = data.Rows[0];
                    el = new EpecLocationModel(row);
                    LogInfo(string.Format("Creating entity for unit {0}", unitId));
                }
                catch { }
            }

            return el;
        }

        EpecLocationModel SearchHomeByPhone(string phone)
        {
            EpecLocationModel el = null;
            DataTable data = null;
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT * FROM EPEC_LOCATION");
            sql.AppendFormat(" WHERE (equip_phone IS NOT NULL and equip_phone = '{0}')", phone);
            sql.AppendFormat(" or(other_phone is not null and other_phone = '{0}')", phone);

            try
            {
                data = _provider.GetData(sql.ToString(), null);
                LogInfo(string.Format("Running: {0}", sql.ToString()));
            }
            catch (Exception e)
            {
                LogError(e);
            }

            if (data != null && data.Rows.Count > 0)
            {
                var row = data.Rows[0];
                el = new EpecLocationModel(row);
                LogInfo(string.Format("Searching Home for phone {0}", phone));
            }

            return el;
        }

        EpecLocationModel SearchResidentByPhone(string phone)
        {
            EpecLocationModel el = null;
            DataTable data = null;
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT * FROM RESIDENT r");
            sql.Append(" JOIN EPEC_LOCATION el ON el.Location_Def = r.Location_Ref");
            sql.AppendFormat(" WHERE (r.s_phone_1 IS NOT NULL AND r.s_phone_1 = '{0}')", phone);
            sql.AppendFormat(" OR (r.s_phone_2 IS NOT NULL AND r.s_phone_2 = '{0}')", phone);

            try
            {
                data = _provider.GetData(sql.ToString(), null);
                LogInfo(string.Format("Running: {0}", sql.ToString()));
            }
            catch (Exception e)
            {
                LogError(e);
            }

            if (data != null && data.Rows.Count > 0)
            {
                var row = data.Rows[0];
                el = new EpecLocationModel(row);
                LogInfo(string.Format("Searching Resident for phone {0}", phone));
            }

            return el;
        }
        #endregion

        public static HomeService Instance
        {
            get
            {
                return new HomeService();
            }
        }

        HomeService()
        {
            try
            {
                _log = new LogHelper(AppConfigurationHelper.LogFile);
            }
            catch { }

            _provider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfigurationHelper.PNCConnection);
        }

        public EpecLocationModel GetHomeByWorkPhone(string phone )
        {
            EpecLocationModel el = null;
            DataTable data = null;
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT * FROM EPEC_LOCATION el ");
            sql.Append("JOIN RESIDENT r ON r.LOCATION_REF = el.LOCATION_DEF ");
            sql.AppendFormat("WHERE el.OTHER_PHONE = '{0}' ", phone);
            sql.Append("ORDER BY r.PRIORITY ASC");
            
            try
            {
                data = _provider.GetData(sql.ToString(), null);
                LogInfo(string.Format("Running: {0}", sql.ToString()));
            }
            catch(Exception e)
            {
                LogError(e);
            }

            if (data != null)
            {
                var row = data.Rows[0];
                el = new EpecLocationModel(row);
                LogInfo(string.Format("Creating entity for phone {0}", phone));
            }

            return el;
        }

        public CallCodeModel GetCallCodeForEvent(string internalEventCode)
        {
            CallCodeModel model = null;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM CALL_CODE_LOOKUP WHERE CALL_CODE_TO_USE = '{0}'",internalEventCode);

            try
            {
                var data = _provider.GetData(sql.ToString(), null);
                LogInfo(string.Format("Fetching call code for event code {0}. QUERY: {1}", internalEventCode, sql.ToString()));

                if(data != null)
                {
                    var row = data.Rows[0];

                    model = new CallCodeModel();
                    model.CALL_CODE_DEF = row["CALL_CODE_DEF"].Parse<long>();
                }
            }
            catch(Exception e)
            {
                LogError(e);
            }

            return model;
        }
        
        public bool SearchExistingUnit(CellDeviceModel model)
        {
            // 20190604 - Search Epec/Resident for the phone and equipId
            // Fields: e.equip_phone,e.other_phone,r.phone_1,r.phone_2, r.phone_3
            // Fields: e.equip_id
            EpecLocationModel el = null;

            //1. check the unit first
            el = SearchHomeByUnit(model.UNIT_ID);
            if(el != null)
            {
                return true;
            }

            //2. search epec for phone
            el = SearchHomeByPhone(model.MDN);
            if (el != null)
                return true;

            //3 Search resident phone fields
            el = SearchResidentByPhone(model.MDN);
            if (el != null)
                return true;

            return false;
        }
    }
}

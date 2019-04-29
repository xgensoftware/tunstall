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

        public List<CellDeviceModel> GetCellDevices()
        {
            List<CellDeviceModel> model = new List<CellDeviceModel>();
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM CELL_DEVICE_ACTIVE WHERE TEST = 'True'");

            try
            {
                var data = _provider.GetData(sql.ToString(), null);
                LogInfo(string.Format("Fetching all cell devices. QUERY: {0}", sql.ToString()));

                if (data != null)
                {
                    foreach(DataRow dr in data.Rows)
                    {
                        var item = new CellDeviceModel(dr);
                        model.Add(item);
                    }
                    
                    LogInfo(string.Format("Fetching all cell device models"));
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }

            return model;
        }

        public CellDeviceModel GetCellDeviceByUnitId(string unitId)
        {
            CellDeviceModel model = null;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM CELL_DEVICE_ACTIVE WHERE UNIT_ID = {0}", unitId);

            try
            {
                var data = _provider.GetData(sql.ToString(), null);
                LogInfo(string.Format("Fetching cell device for unit Id {0}. QUERY: {1}", unitId, sql.ToString()));

                if(data != null)
                {
                    var row = data.Rows[0];

                    model = new CellDeviceModel(row);
                    LogInfo(string.Format("Creating cell device model for unit {0}", unitId));
                }
            }
            catch(Exception e)
            {
                LogError(e);
            }

            return model;
        }

        public void UpdateCellDeviceStatus(long id, bool status)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE CELL_DEVICE_ACTIVE SET TEST = '{0}' WHERE ID = {1}", status.ToString(),id);

            try
            {
                _provider.ExecuteNonSPQuery(sql.ToString(), null);
                LogInfo(string.Format("Updating cell device record {0} to {1}", id, status.ToString()));
            }
            catch(Exception e)
            {
                LogError(e);
            }
        }
    }
}

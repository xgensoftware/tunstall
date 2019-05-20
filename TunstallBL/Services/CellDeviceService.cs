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
    public class CellDeviceService : BaseService<CellDeviceService>, IDisposable
    {
        public static CellDeviceService Instance
        {
            get
            {
                return new CellDeviceService();
            }
        }

        CellDeviceService()
        {
            try
            {
                _log = new LogHelper(AppConfigurationHelper.LogFile);
            }
            catch { }

            _provider = DALFactory.CreateSqlProvider(DatabaseProvider_Type.SqlAnywhere, AppConfigurationHelper.PNCConnection);
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
                    foreach (DataRow dr in data.Rows)
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

                if (data != null)
                {
                    var row = data.Rows[0];

                    model = new CellDeviceModel(row);
                    LogInfo(string.Format("Creating cell device model for unit {0}", unitId));
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }

            return model;
        }

        public void UpdateCellDeviceStatus(long id, bool status)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE CELL_DEVICE_ACTIVE SET TEST = '{0}' WHERE ID = {1}", status.ToString(), id);

            try
            {
                _provider.ExecuteNonSPQuery(sql.ToString(), null);
                LogInfo(string.Format("Updating cell device record {0} to {1}", id, status.ToString()));
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        public List<CellDeviceModel> SearchCellDevice(CellDeviceSearchModel model)
        {
            StringBuilder sql = new StringBuilder("SELECT * FROM CELL_DEVICE_ACTIVE WHERE 1=1");

            if (!string.IsNullOrEmpty(model.UnitId))
            {
                sql.AppendFormat(" AND UNIT_ID = {0}", model.UnitId);
            }

            if (!string.IsNullOrEmpty(model.IMEI))
            {
                sql.AppendFormat(" AND IMEI LIKE '{0}%'", model.IMEI);
            }

            if (!string.IsNullOrEmpty(model.UnitType))
            {
                sql.AppendFormat(" AND LOWER(OTHER) LIKE '%{0}%'", model.UnitType.ToLower());
            }

            if (model.TestMode.ToLower() != "all")
            {
                sql.AppendFormat(" AND TEST = '{0}'", model.TestMode);
            }

            if (!string.IsNullOrEmpty(model.SerialNumber))
            {
                sql.AppendFormat(" AND SERIAL LIKE '%{0}%'", model.SerialNumber);
            }


            List<CellDeviceModel> collection = new List<CellDeviceModel>();
            try
            {
                var data = _provider.GetData(sql.ToString(), null);

                if (data != null)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        collection.Add(new CellDeviceModel(row));
                    }
                }
            }
            catch (Exception e)
            {
                LogError(e);
            }
            return collection;
        }

        public bool UpdateDevicePhone(CellDeviceModel model)
        {
            bool isUdpated = true;
            string sql = string.Format("UPDATE CELL_DEVICE_ACTIVE SET MDN = '{0}' WHERE ID = {1}", model.MDN, model.ID);

            try
            {
                _provider.ExecuteNonSPQuery(sql.ToString(), null);
                LogInfo(string.Format("Updating cell device {0} phone number to {1}", model.UNIT_ID, model.MDN));
            }
            catch(Exception e)
            {
                LogError(e);
                isUdpated = false;
            }

            return isUdpated;
        }
    }
}

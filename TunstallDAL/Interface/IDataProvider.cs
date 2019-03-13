using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallDAL
{
    public interface IDataProvider
    {


        #region " Properties "        string ConnectionName { get; set; }




        #endregion
        #region " Data Table "        DataTable GetData(CommandType type, string sql, List<SQLParam> parms);        DataTable GetData(string sql, List<SQLParam> parm);





        #endregion
        #region " DataSet "        DataSet ExecuteDataSet(string _procName, List<SQLParam> _parameters);        DataSet ExecuteDataSetQuery(string _sqlString, List<SQLParam> _parameters);




        #endregion
        #region " Execute "        int ExecuteNonQuery(string _procName, List<SQLParam> _parameter);        int ExecuteNonSPQuery(string _sql, List<SQLParam> _parameters);        T ExecuteScalar<T>(CommandType cmdType, string sql, List<SQLParam> parameters);        T ExecuteScalar<T>(string _procName, List<SQLParam> _parameters);        #endregion
    }
}

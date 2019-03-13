using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallDAL
{
    public enum DatabaseProvider_Type    {        MSSQLProvider = 1        , SqlAnywhere = 2    }    public static class DALFactory    {        public static IDataProvider CreateSqlProvider(DatabaseProvider_Type DbType, string connectionName)        {            IDataProvider _provider = null;            switch (DbType)            {                //case DatabaseProvider_Type.MSSQLProvider:                //    _provider = DALBase.GetInstance<Providers.MSSQLProvider>();                //    break;                case DatabaseProvider_Type.SqlAnywhere:                    _provider = DALBase.GetInstance<Providers.SQLAnywhereProvider>();
                    break;

            }            _provider.ConnectionName = connectionName;            return _provider;        }    }
}

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iAnywhere.Data.SQLAnywhere;

namespace TunstallDAL.Providers
{
    class SQLAnywhereProvider : DALBase, IDataProvider
        {

        }



        #region " Private Methods "




        #endregion
        #region " Interface Implemenatation "
        public DataTable GetData(CommandType type, string sql, List<SQLParam> parms)


        #endregion
}
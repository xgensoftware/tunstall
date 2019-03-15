using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;
using com.Xgensoftware.DAL;

namespace PNC_Kshema.Repository
{
    class RepositoryBase : IDisposable
    {
        protected IDataProvider _stageProvider = null;

        public virtual void Dispose()
        {
            if (_stageProvider != null)
                _stageProvider = null;
        }
    }
}

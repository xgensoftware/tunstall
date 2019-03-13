using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallDAL;
using TunstallBL.Helpers;
using TunstallBL.Models;
namespace TunstallBL.Services
{
    public abstract class BaseService<T>
    {
        protected TunstallDatabaseContext _db = null;
        protected IDataProvider _provider = null;
        protected LogHelper _log = null;

        public void Dispose()
        {
            if(_db != null)
                {
                _db.Dispose();
            }

            if (_provider != null)
                _provider = null;

            if(_log != null)
            {
                _log = null;
            }
        }

        protected void LogInfo(string message)
        {
            if(_log != null)
            {
                _log.LogMessage(LogMessageType.INFO, message);
            }
        }

        protected void LogError(Exception e)
        {
            if(_log != null)
            {
                _log.LogException(e);
            }
        }

       protected string StripPhoneNumber(string phoneNumber)
        {
            if (AppConfiguration.StripPhoneNumberField)
            {
                return phoneNumber.Remove(0, 1);
            }
            else
                return phoneNumber;
        }
    }
}

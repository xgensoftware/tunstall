using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallDAL;
using TunstallBL.Models;
namespace TunstallBL.Services
{
    public abstract class BaseService<T>
    {
        protected TunstallDatabaseContext _db = null;


        public void Dispose()
        {
            if(_db != null)
                {
                _db.Dispose();
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

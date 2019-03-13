using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallDAL
{
    abstract class DALBase    {        public string ConnectionName { get; set; }        public static T GetInstance<T>()        {            return Activator.CreateInstance<T>();        }

    }
}

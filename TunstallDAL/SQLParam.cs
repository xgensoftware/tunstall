using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallDAL
{
    public struct SQLParam    {        public string Name;        public object Value;        public static SQLParam GetParam(string name, object value)        {            return new SQLParam() { Name = name, Value = value };        }    }
}

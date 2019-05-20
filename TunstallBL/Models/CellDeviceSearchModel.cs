using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.Models
{
    public class CellDeviceSearchModel
    {
        
        public string UnitId { get; set; }

       public string IMEI { get; set; }

        public string SerialNumber { get; set; }

        public string TestMode { get; set; }

        public string UnitType { get; set; }
    }
}

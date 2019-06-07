using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.Models
{
    public class CellDeviceModel
    {
        public CellDeviceModel()
        {

        }
        public CellDeviceModel(System.Data.DataRow dr)
        {
            if(dr != null)
            {
                ID = dr["ID"].Parse<long>();
                UNIT_ID = dr["UNIT_ID"].Parse<long>();
                ICCID = dr["ICCID"].ToString();
                MDN = dr["MDN"].ToString();
                IMEI = dr["IMEI"].ToString();
                SERIALNO = dr["SERIAL"].ToString();
                TEST = string.IsNullOrEmpty(dr["TEST"].ToString()) ? false : dr["TEST"].Parse<bool>();
                OTHER = dr["OTHER"].ToString();
            }
        }
        public long ID { get; set; }

        public long UNIT_ID { get; set; }

        public string ICCID { get; set; }

        public string  MDN { get; set; }

        public string IMEI { get; set; }

        public bool TEST { get; set; }

        public string SERIALNO { get; set; }

        public string OTHER { get; set; }
    }
}

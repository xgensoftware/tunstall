using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallBL;
namespace TunstallBL.Models
{
    public class ResidentModel
    {
        public ResidentModel(System.Data.DataRow dr = null)
        {
            if(dr != null)
            {
                try
                {
                    RESIDENT_DEF = dr["RESIDENT_DEF"].Parse<long>();
                    LOCATION_REF = dr["LOCATION_REF"].Parse<long>();
                    FIRST_NAME = dr["FIRST_NAME"].ToString();
                    LAST_NAME = dr["LAST_NAME"].ToString();
                    PRIORITY = dr["PRIORITY"].Parse<int>();
                }
                catch { }
             }
        }

        public long RESIDENT_DEF { get; set; }

        public long LOCATION_REF { get; set; }

        public string FIRST_NAME { get; set; }

        public string LAST_NAME { get; set; }

        public int PRIORITY { get; set; }
    }

    public class EpecLocationModel
    {

        public EpecLocationModel(System.Data.DataRow dr = null)
        {
            Residents = new List<ResidentModel>();

            if (dr != null)
            {
                try
                {
                    LOCATION_DEF = dr["LOCATION_DEF"].Parse<long>();
                    EQUIP_ID = dr["EQUIP_ID"].Parse<long>();
                    AUTHORITY_REF = dr["AUTHORITY_REF"].Parse<long>();
                    ADDRESS_STREET = dr["ADDRESS_STREET"].ToString();
                    ADDRESS_AREA = dr["ADDRESS_AREA"].ToString();
                    ADDRESS_TOWN = dr["ADDRESS_TOWN"].ToString();
                    ADDRESS_COUNTY = dr["ADDRESS_COUNTY"].ToString();
                    ADDRESS_POSTCODE = dr["ADDRESS_POSTCODE"].ToString();
                    OTHER_PHONE = dr["OTHER_PHONE"].ToString();
                    EQUIP_PHONE = dr["EQUIP_PHONE"].ToString();
                    EQUIP_MODEL_REF = dr["EQUIP_MODEL_REF"].Parse<long>();
                }
                catch { }
                
                var r = new ResidentModel(dr);
                Residents.Add(r);
            }
        }


        public long LOCATION_DEF { get; set; }

        public long EQUIP_ID { get; set; }

        public long AUTHORITY_REF { get; set; }

        public string ADDRESS_STREET { get; set; }

        public string ADDRESS_AREA { get; set; }

        public string ADDRESS_TOWN { get; set; }

        public string ADDRESS_COUNTY { get; set; }

        public string ADDRESS_POSTCODE { get; set; }

        public string OTHER_PHONE { get; set;}

        public string EQUIP_PHONE { get; set; }

        public long EQUIP_MODEL_REF { get; set; }


        public List<ResidentModel> Residents { get; set; }
    }
}

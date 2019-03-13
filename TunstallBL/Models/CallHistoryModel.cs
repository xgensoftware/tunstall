using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallBL;

namespace TunstallBL.Models
{
    public class CallHistoryModel
    {
        public CallHistoryModel(System.Data.DataRow dr = null)
        {

        }

        public long CALL_DEF { get; set; }

        public DateTime OFF_HOOK_TIME { get; set; }

        public DateTime ARRIVAL_TIME { get; set; }

        public int CALL_ENTITY_TYPE { get; set; }

        public long CALL_ENTITY_REF { get; set; }

        public int SCHEME_ID { get; set; }

        public long EQUIP_ID { get; set; }

        public string CALL_CODE { get; set; }

        public string CALL_INFO { get; set; }

        public string MEANING { get; set; }

        public long REASON_REF { get; set; }

        public int ORIGIN { get; set; }

        public string REFERRALS_YN { get; set; }

        public string USER_CREATED_YN { get; set; }

        public string DELETED_YN { get; set; }

        public string PHONE_NUMBER { get; set; }

        public string NOTES_YN { get; set; }

        public string PROTOCOL_TAG { get; set; }

        public long AUTHORITY_REF { get; set; }

        public int VR_RECORDER_ID { get; set; }

        public string IP_CALL_YN { get; set; }

        public string INCOMING_YN { get; set; }
    }
}

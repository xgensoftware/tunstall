using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace MytrexAPI.Models
{
    public class EventModel
    {
        public EventModel()
        {
            TestMode = false;
            MCCallOrigination = false;

        }

        [JsonProperty("accountCode")]
        public string AccountCode { get; set; }

        [JsonProperty("callerID")]
        public string CallerId { get; set; }

        [JsonProperty("eventCode")]
        public string EventCode { get; set; }

        [JsonProperty("eventTime")]
        public long EventTime { get; set; }

        public DateTime EventTimeStamp { get; set; }

        [JsonProperty("eventQualifier")]
        public string Qualifier { get; set; }

        [JsonProperty("eventZone")]
        public string Zone { get; set; }

        [JsonProperty("lineID")]
        public string LineId { get; set; }

        [JsonProperty("unitModel")]
        public string UnitModel { get; set; }

        [JsonProperty("testMode")]
        public bool TestMode { get; set; }

        [JsonProperty("mcCallOrigination")]
        public bool MCCallOrigination { get; set; }
    }
}
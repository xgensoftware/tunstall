﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TunstallBL.Models
{
    public enum External_Service
    {
        MYTREX = 1,
        ANELTO = 2
    }

    public class EventModel
    {
        public EventModel()
        {
            TestMode = false;
            MCCallOrigination = false;

        }

        public long Id { get; set; }

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

       

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string VerificationURL { get; set; }

        public int ServiceId { get; set; }

        public string RawData { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", AccountCode, CallerId);
        }
    }
}

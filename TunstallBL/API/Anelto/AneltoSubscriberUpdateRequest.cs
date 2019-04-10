using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TunstallBL.API.Model
{
    public class AneltoSubscriberUpdateRequest
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ani { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string iccid { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string firstname { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lastname { get; set; }
        
        [JsonProperty(NullValueHandling =NullValueHandling.Ignore)]
        public string account { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string address1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string city { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string state { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zip { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string dob { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cgemail { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cgmobile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cghome { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.API.Model
{
    public class AneltoSubscriberUpdateRequest
    {
        public string ani { get; set; }
        public string iccid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string account { get; set; }
        public string address { get; set; }
        public string address1 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string dob { get; set; }
        public string cgemail { get; set; }
        public string cgmobile { get; set; }
        public string cghome { get; set; }
    }
}

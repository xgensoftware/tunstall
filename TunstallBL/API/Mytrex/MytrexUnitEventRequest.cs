using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.API.Model
{
    public class EventData
    {
        public string EventName { get; set; }
        public string EventCode { get; set; }
        public string InitialVoice { get; set; }
        public int Enabled { get; set; }
        public int Beeping { get; set; }
        public int VoiceAnnouncements { get; set; }
    }

    public class MytrexUnitEvent
    {
        public string UnitSerNum { get; set; }
        public string EventCategoryName { get; set; }
        public string EventCategoryPhone1 { get; set; }
        public string EventCategoryPhone2 { get; set; }
        public List<EventData> EventData { get; set; }
    }
}

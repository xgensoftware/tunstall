using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
namespace TunstallBL.Models
{

    [XmlRoot(ElementName = "location")]
    public class Location
    {
        [XmlElement(ElementName = "latitude")]
        public string Latitude { get; set; }
        [XmlElement(ElementName = "longitude")]
        public string Longitude { get; set; }
        [XmlElement(ElementName = "evtype")]
        public string Evtype { get; set; }
    }

    [XmlRoot(ElementName = "alarm")]
    public class Alarm
    {
        [XmlElement(ElementName = "csAccountNumber")]
        public string AccountNumber { get; set; }
        [XmlElement(ElementName = "zone")]
        public string Zone { get; set; }
        [XmlElement(ElementName = "verificationUrl")]
        public string VerificationUrl { get; set; }
        [XmlElement(ElementName = "ani")]
        public string Ani { get; set; }
        [XmlElement(ElementName = "location")]
        public Location Location { get; set; }
        [XmlElement(ElementName = "timestamp")]
        public string Timestamp { get; set; }
    }
}

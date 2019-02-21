using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TunstallDAL.Entities
{
    public class Event
    {
        public Event()
        {
            TestMode = false;
            MCCallOrigination = false;
            IsProcessed = false;
            ServiceId = 1;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(75)]
        public string AccountCode { get; set; }

        [StringLength(25)]
        public string CallerId { get; set; }

        [StringLength(255)]
        public string EventCode { get; set; }

        public long EventTime { get; set; }

        public DateTime EventTimeStamp { get; set; }

        [StringLength(25)]
        public string Qualifier { get; set; }

        [StringLength(50)]
        public string Zone { get; set; }

        [StringLength(25)]
        public string LineId { get; set; }

        [StringLength(50)]
        public string UnitModel { get; set; }

        public bool TestMode { get; set; }

        public bool MCCallOrigination { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string VerificationURL { get; set; }

        public bool IsProcessed { get; set; }

        public int ServiceId { get; set; }

        [StringLength(10)]
        public string EventZone { get; set; }
    }
}

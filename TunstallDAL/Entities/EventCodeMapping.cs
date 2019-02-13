using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TunstallDAL.Entities
{
    public class EventCodeMapping
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        [StringLength(10)]
        public string ExternalEventCode { get; set; }

        [StringLength(10)]
        public string InternalEventCode { get; set; }

        [StringLength(25)]
        public string Integration { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.Models
{
    public class CallCodeModel
    {
        public long CALL_CODE_DEF { get; set; }

        public string TEXT { get; set; }

        public string CALL_CODE_TO_USE { get; set; }

        public long REASON_REF { get; set; }

        public string PROTOCOL { get; set; }
    }
}

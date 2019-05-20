using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TunstallBL.Models
{
    public class ListItem
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Key;
        }
    }
}

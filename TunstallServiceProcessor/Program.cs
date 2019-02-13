using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallBL;
using TunstallBL.Services;

namespace TunstallServiceProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceId = ConfigurationManager.AppSettings["ServiceId"].Parse<int>();
            EventService.Instance.ProcessEventQueue(serviceId);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using com.Xgensoftware.Core;
using PNC_Kshema.BL.SyncServices;

namespace PNC_Kshema_CallsHistory
{
    class Program
    {
        const string Id = "55803117-FAA3-47C9-85FF-35F26C5F70E5";
        static void Main(string[] args)
        {
            bool thisInstance;
            using (var semaphore = new Semaphore(0, 1, Id, out thisInstance))
            {
                if (thisInstance)
                {
                    Guid group_record = Guid.NewGuid();

                    DateTime from_date = DateTime.Now.AddDays(-1);
                    DateTime to_date = DateTime.Now;

                    if (args.Length != 0)
                    {
                        from_date = args[0].Parse<DateTime>();
                        to_date = args[1].Parse<DateTime>();
                    }

                    PNC_CallHistory_Sync sync = new PNC_CallHistory_Sync(group_record.ToString());
                    sync.RunSync();
                    
                    semaphore.Release();
                }
            }
        }
    }
}

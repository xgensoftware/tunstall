using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using com.Xgensoftware.Core;
using Kshema_PNC.BL.Service;
using PNC_Kshema.BL.SyncServices;
using Agency_Import;

namespace TestApp
{
    class Program
    {
        const string Id = "af49d266-e4f4-4a63-b73c-f62c1144b585";
        static void Main(string[] args)
        {
            string unitId = "3000237";
            string temp = (unitId.Parse<long>() - "3000000".Parse<long>()).ToString("X4");
            
            Console.WriteLine(temp);
            Console.ReadLine();
            //bool thisInstance;
            //using (var semaphore = new Semaphore(0, 1, Id, out thisInstance))
            //{
            //    if (thisInstance)
            //    {
            //        Guid group_record = Guid.NewGuid();

            //        // first run the Agency Importer
            //        //Agency_Sync agency_sync = new Agency_Sync(group_record.ToString());
            //        //agency_sync.Run_Sync();


            //        //process KShema to PNC
            //        //Kshema_PNC_Sync kshema_pnc = new Kshema_PNC_Sync(group_record.ToString());
            //        //kshema_pnc.Run_Sync();


            //        PNC_Kshema_Sync sync = new PNC_Kshema_Sync(group_record.ToString());
            //        sync.RunSync();

            //        semaphore.Release();
            //    }
            //}
        }
    }
}

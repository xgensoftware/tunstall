using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNC_Kshema.BL.SyncServices
{
    public interface ISync
    {       

        void Start();
        void Stop();
        void RunSync();
    }
}

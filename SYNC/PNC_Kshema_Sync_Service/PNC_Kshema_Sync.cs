using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using PNC_Kshema.BL.SyncServices;

namespace PNC_Kshema_Sync_Service
{
    public partial class PNC_Kshema_Sync : ServiceBase
    {
        PNC_Kshema_Sync _service = null;
        public PNC_Kshema_Sync()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _service = new PNC_Kshema_Sync();
            _service.OnStart(args);
        }

        protected override void OnStop()
        {
            if (_service != null)
                _service.Stop();
        }
    }
}

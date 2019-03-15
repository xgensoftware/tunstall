using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kshema_PNC.BL.Entity
{
    public delegate void EntityMessage_Handler(string message, string log_type);

    interface IEntity
    {
        event EntityMessage_Handler OnEntityMessage;

        void ProcessDataFlow(string process_type);
        //void ProcessDataDelete();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNC_Kshema.BL.Entity
{
    public delegate void EntityMessage_Handler(string message,string message_type);

    interface IEntity
    {
        event EntityMessage_Handler OnEntityMessage;

        void ProcessDataFlow();
        void ProcessDataDelete();

    }
}

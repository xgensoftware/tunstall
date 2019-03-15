using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using com.Xgensoftware.Core;

namespace PNC_Kshema.BL.Entity
{
    class EntityResponse
    {
        public bool IsSuccess { get; set; }
        public string EntityMessage { get; set; }

        public EntityResponse()
        {
            IsSuccess = false;
        }

        public EntityResponse(bool isSuccess, string msg)
        {
            IsSuccess = isSuccess;
            EntityMessage = msg;
        }
    }
}

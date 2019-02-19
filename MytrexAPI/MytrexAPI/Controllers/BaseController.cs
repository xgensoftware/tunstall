using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using TunstallBL.Helpers;
using TunstallBL.Models;
namespace MytrexAPI.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected bool IsUserAuthorized(HttpActionContext actionContext)
        {

            var authorization = FetchFromHeader(actionContext);
            var token = JWTHelper.DecodeToken(authorization);
            var username = token.Payload.First(p => p.Key == "username").Value.ToString();


            return true;
        }
        private string FetchFromHeader(HttpActionContext ctx)
        {
            string requestToken = string.Empty;
            var authRequest = ctx.Request.Headers.Authorization;
            if(authRequest != null)
            {
                requestToken = authRequest.Parameter;
            }

            return requestToken;
        }
    }
}

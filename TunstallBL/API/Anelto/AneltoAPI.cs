using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using TunstallBL.API.Model;
namespace TunstallBL.API
{
    public class AneltoAPI
    {
        #region Member Variables
        string _username = string.Empty;
        string _password = string.Empty;
        RestClient _client = null;
        #endregion

        #region Private Methods
        static RestRequest FormRequest(Method method, string url)        {            RestRequest request = new RestRequest(url, method);            request.AddHeader("Content-Type", "application/json");            return request;        }
        #endregion

        #region Constructor
        public AneltoAPI(string username, string password)
        {
            _username = username;
            _password = password;
        }
        #endregion

        public string SubscriberCreateUpdate(AneltoSubscriberUpdateRequest model)
        {
            var client = new RestClient("https://ss.anelto.com:12332");
            client.Authenticator = new HttpBasicAuthenticator(_username, _password);

            var request = FormRequest(Method.POST, "/subscriber/create");
            request.AddJsonBody(model);
            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Failed Anelto SubscriberCreateUpdate. Status: {0} Response: {1}", response.StatusCode.ToString(), response.Content));
            }
            else
            {
                return string.Format("Successfully sent signal to Anelto. Status: {0} Response: {0}", response.StatusCode, response.ResponseStatus.ToString());
            }
        }
    }
}

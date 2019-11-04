using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using TunstallBL.API.Model;
using TunstallBL.Helpers;
namespace TunstallBL.API
{
    public class AneltoAPI
    {
        #region Member Variables
        RestClient _client = null;

        string _apiUsername = string.Empty;
        string _apiPassword = string.Empty;
        #endregion

        #region Private Methods
        static RestRequest FormRequest(Method method, string url)        {            RestRequest request = new RestRequest(url, method);            request.AddHeader("Content-Type", "application/json");            return request;        }
        #endregion

        #region Constructor
        public AneltoAPI(string userName, string password)
        {
            _apiUsername = userName;
            _apiPassword = password;
        }
        #endregion

        public bool SubscriberExists(AneltoSubscriberGetRequest model)
        {
            bool subscriberExists = false;

            var client = new RestClient(AppConfigurationHelper.AneltoURL);
            client.Authenticator = new HttpBasicAuthenticator(_apiUsername, _apiPassword);

            var request = FormRequest(Method.POST, "/subscriber/get");
            request.AddJsonBody(model);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                subscriberExists = true;

            return subscriberExists;
        }

        public string SubscriberCreateUpdate(AneltoSubscriberUpdateRequest model)
        {
            var client = new RestClient(AppConfigurationHelper.AneltoURL);
            client.Authenticator = new HttpBasicAuthenticator(_apiUsername, _apiPassword);

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

        public string SubscriberCCOverride(AneltoSubscriberOverrideRequest model)
        {
            var client = new RestClient(AppConfigurationHelper.AneltoURL);
            client.Authenticator = new HttpBasicAuthenticator(_apiUsername, _apiPassword);

            var request = FormRequest(Method.POST, "/subscriber/ccoverride");

            StringBuilder body = new StringBuilder();
            body.Append("{");
            body.AppendFormat("\"accounts\":[\"{0}\"],", model.accounts);
            body.AppendFormat("\"number\":\"{0}\"", model.number);
            body.Append("}");
            
            request.AddJsonBody(body.ToString());

            var response = client.Execute(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(string.Format("Failed Anelto SubscriberCCOverride. Status: {0} Response: {1}", response.StatusCode.ToString(), response.Content));
            }
            else
            {
                return string.Format("Successfully sent signal to Anelto. Status: {0} Response: {0}", response.StatusCode, response.ResponseStatus.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using Newtonsoft.Json;
using TunstallBL.Helpers;
using TunstallBL.API.Model;

namespace TunstallBL.API
{
    public class MytrexAPI
    {
        #region Member Variables
        RestClient _client = null;
        #endregion

        #region Private Methods
        RestRequest FormRequest(Method method, string url)        {            RestRequest request = new RestRequest(url, method);            request.AddHeader("Content-Type", "application/json");            return request;        }
        #endregion

        #region Public Methods
        public List<MytrexUnitEvent> GetEvents(string unitSerialNum)
        {
            var token = JWTHelper.GetToken(AppConfigurationHelper.MytrexSecret, AppConfigurationHelper.MytrexUsername);
            var client = new RestClient(AppConfigurationHelper.MytrexUrl);

            var request = FormRequest(Method.GET, string.Format("/unit/event?dealerkey={0}&UnitSerNum={1}", AppConfigurationHelper.MytrexDealerKey, unitSerialNum));
            request.AddHeader("Authorization", string.Format("Bearer {0}",token));

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<List<MytrexUnitEvent>>(response.Content);
            }
            else
            {
                throw new Exception(string.Format("Failed to get Mytex event list. RESPONSE: {0}", response.Content));
            }
        }

        public string UpdateEvents(List<MytrexUnitEvent> events)
        {
            var token = JWTHelper.GetToken(AppConfigurationHelper.MytrexSecret, AppConfigurationHelper.MytrexUsername);
            var client = new RestClient(AppConfigurationHelper.MytrexUrl);

            var request = FormRequest(Method.PUT, string.Format("/unit/event?dealerkey={0}", AppConfigurationHelper.MytrexDealerKey));
            request.AddHeader("Authorization", string.Format("Bearer {0}", token));
            request.AddJsonBody(events);

            IRestResponse response = client.Execute(request); 
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return string.Format("Successfully sent Mytrex Update Event. Status: {0} Response: {1}", response.StatusCode.ToString(), response.Content);
            }
            else
            {
                return string.Format("Failed Mytrex Update Event. Status: {0} Response: {1}", response.StatusCode.ToString(), response.Content);
            }
        }
        #endregion
    }
}

using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        public void SendMoney(int toId, decimal money)
        {
            RestRequest request = new RestRequest($"transfer/{toId}/{money}");
            IRestResponse restResponse = client.Post(request);
            CheckForError(restResponse);
        }

        public decimal GetBalance()
        {
            RestRequest request = new RestRequest("/money");
            IRestResponse<decimal> restResponse = client.Get<decimal>(request);
            CheckForError(restResponse);
            return restResponse.Data;
        }

        public List<OtherUser> GetOtherUsers()
        {
            RestRequest request = new RestRequest("/transfer");
            IRestResponse<List<OtherUser>> restResponse = client.Get<List<OtherUser>>(request);
            CheckForError(restResponse);
            return restResponse.Data;
        }






        public List<Transfer> ViewTransfers()
        {
            RestRequest request = new RestRequest("/transfer/history");
            IRestResponse<List<Transfer>> restResponse = client.Get<List<Transfer>>(request);
            CheckForError(restResponse);
            return restResponse.Data;
        }

        


    }
}

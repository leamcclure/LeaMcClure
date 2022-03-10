using RestSharp;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoApiService : AuthenticatedApiService
    {
        public readonly string ApiUrl;

        public TenmoApiService(string apiUrl) : base(apiUrl) { }

        // Add methods to call api here...

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

        //public List<TransferDetails> ViewTransferDetails()
        //{
        //    RestRequest request = new RestRequest("/transfer/");
        //    IRestResponse<List<TransferDetails>> restResponse = client.Get<List<TransferDetails>>(request);
        //    CheckForError(restResponse);
        //    return restResponse.Data;
        //}


    }
}

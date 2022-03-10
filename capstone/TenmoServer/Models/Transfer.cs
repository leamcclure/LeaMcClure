using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int Id { get; set; }

        public int TransferTo { get; set; }

        public int TransferFrom { get; set; }

        public decimal Amount { get; set; }
        public string Username { get; set; }

        public Transfer()
        {

        }
        public Transfer(int id, int transferTo, int transferFrom, decimal amount, string username)
        {
            Id = id;
            TransferTo = transferTo;
            TransferFrom = transferFrom;
            Amount = amount;
            Username = username;
        }


    }
}


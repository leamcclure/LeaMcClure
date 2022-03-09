using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int Id { get; set; }

        public string TransferTo { get; set; }

        public string TransferFrom { get; set; }

        public decimal Amount { get; set; }

        public Transfer()
        {

        }
        public Transfer(int id, string transferTo, string transferFrom, decimal amount)
        {
            Id = id;
            TransferTo = transferTo;
            TransferFrom = transferFrom;
            Amount = amount;
        }


    }
}


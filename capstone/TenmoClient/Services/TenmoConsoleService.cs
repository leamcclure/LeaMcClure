using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }
        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        // Add application-specific UI methods here...
        public void PrintOtherUsers(List<OtherUser> otherUsers)
        {
            Console.WriteLine("| --------------Users---------------|");
            Console.WriteLine("| Id | Username                     |");
            Console.WriteLine("| -------+--------------------------|");

            foreach(OtherUser user in otherUsers) 
            {
                Console.WriteLine($"|  {user.UserId} | {user.Username}");
            } 
            Console.WriteLine("| ----------------------------------|");
        }




        public void ViewTransfers(List<Transfer> transfers, int currentUser)
        {
            Console.WriteLine("| -------------------------------------------|");
            Console.WriteLine("| Id             From/To            Amount   |");
            foreach (Transfer transfer in transfers)
            {   
                if(transfer.TransferFrom == currentUser) 
                {
                    Console.WriteLine($"{transfer.Id}          To:   {transfer.TransferTo}          {transfer.Amount.ToString("C")}");
                }
                else
                {
                    Console.WriteLine($"{transfer.Id}          From: {transfer.TransferFrom}          {transfer.Amount.ToString("C")}");
                }
            }
            Console.WriteLine("| ----------------------------------|");
        }

        //public int ViewTransferDetails(List<TransferDetails> transferDetails)
        //{
        //    Console.WriteLine("-------------------------------------------");
        //    Console.WriteLine("Transfers");
        //    Console.WriteLine("ID From/ To                 Amount");
        //    Console.WriteLine($"{userId},           From:{user},     ${amount}");
        //    Console.WriteLine($"{userId},           To:{user},     ${amount}");
        //    Console.WriteLine("--------------------------------------------");
        //    Console.WriteLine("Please enter transfer ID to view details(0 to cancel):");

        //}


    }
}

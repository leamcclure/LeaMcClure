using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService currentConsole = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            currentConsole.PrintLoginMenu();
            int menuSelection = currentConsole.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                currentConsole.PrintError("Invalid selection. Please choose an option.");
                currentConsole.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            currentConsole.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = currentConsole.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                Console.Clear();
                decimal balance = tenmoApiService.GetBalance();

                Console.WriteLine($"Your current account balance is: {balance.ToString("C")}");
                // View your current balance
            }

            if (menuSelection == 2)
            {
                try
                {
                    List<Transfer> transfers = tenmoApiService.ViewTransfers();
                    currentConsole.ViewTransfers(transfers, tenmoApiService.UserId, tenmoApiService.Username);  
                }
                catch
                {
                    currentConsole.PrintError("Unable to proccess your request!");
                }

                // View your past transfers
            }

            if (menuSelection == 3)
            {


                // View your pending requests
            }

            if (menuSelection == 4)
            {
                // Send TE bucks
                try
                {
                    List<OtherUser> otherUser = tenmoApiService.GetOtherUsers();
                    currentConsole.PrintOtherUsers(otherUser);
                    int userId = currentConsole.PromptForInteger("Id of the user you are sending to", 0);
                    if (CheckOtherUser(userId, otherUser))
                    {
                        decimal moneyToTransfer = currentConsole.PromptForDecimal("Enter amount to send (press enter to cancel)", 0);
                        if (moneyToTransfer != 0)
                        {
                            tenmoApiService.SendMoney(userId, moneyToTransfer);
                            currentConsole.PrintSuccess($"You've successfully transfered {moneyToTransfer.ToString("C")} to {otherUser.Find(x => x.UserId == userId).Username}");
                        }
                        else
                        {
                            currentConsole.PrintError("Transaction has been cancelled.");
                        }
                    }
                }
                catch (Exception)
                {
                    currentConsole.PrintError("Transaction failed.");
                }


            }

            if (menuSelection == 5)
            {
                try
                {
                    List<OtherUser> otherUser = tenmoApiService.GetOtherUsers();
                    currentConsole.PrintOtherUsers(otherUser);
                    int userId = currentConsole.PromptForInteger("Id of the user you are requesting money from", 0);
                    if (CheckOtherUser(userId, otherUser))
                    {
                        decimal moneyToTransfer = currentConsole.PromptForDecimal("Enter amount to request (press enter to cancel)", 0);
                        if (moneyToTransfer != 0)
                        {
                            tenmoApiService.RequestMoney(userId, moneyToTransfer);
                            currentConsole.PrintSuccess($"You've successfully requested {moneyToTransfer.ToString("C")} from {otherUser.Find(x => x.UserId == userId).Username}");
                        }
                        else
                        {
                            currentConsole.PrintError("Transaction has been cancelled.");
                        }
                    }

                }
                catch
                {
                    currentConsole.PrintError("Request failed.");
                }


                // Request TE bucks
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                currentConsole.PrintSuccess("You are now logged out");
            }
            currentConsole.Pause();
            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = currentConsole.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    currentConsole.PrintError("Login failed.");
                }
                else
                {
                    currentConsole.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                currentConsole.PrintError("Login failed.");
            }
            currentConsole.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = currentConsole.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    currentConsole.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    currentConsole.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                currentConsole.PrintError("Registration was unsuccessful.");
            }
            currentConsole.Pause();
        }
        public bool CheckOtherUser(int userId, List<OtherUser> otherUser)
        {
            if (null == otherUser.Find(x => x.UserId == userId))
            {
                Console.WriteLine($"{userId} is not a valid user id.");
                return false;
            }
            return true;
        }

    }
}

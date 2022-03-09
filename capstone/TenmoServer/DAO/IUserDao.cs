using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDao
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();
        decimal GetCash(int user_id);
        List<User> GetOtherUsers(int id = 0);

        List<Transfer> ViewTransfers();
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class UserSqlDao : IUserDao
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public UserSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM tenmo_user WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }

        public List<User> GetUsers()
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username, password_hash, salt FROM tenmo_user", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = GetUserFromReader(reader);
                        returnUsers.Add(u);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }
        public List<User> GetOtherUsers(int id = 0)
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, username FROM tenmo_user WHERE user_id != @user_id", conn);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = new User();
                        u.UserId = Convert.ToInt32(reader["user_id"]);
                        u.Username = Convert.ToString(reader["username"]);
                        returnUsers.Add(u);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO tenmo_user (username, password_hash, salt) VALUES (@username, @password_hash, @salt)", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("INSERT INTO account (user_id, balance) VALUES (@userid, @startBalance)", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return GetUser(username);
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
            };

            return u;
        }

        public decimal GetCash(int user_id)
        {
            decimal returnDecimal = 0M;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT balance FROM account WHERE user_id = @user_id;", conn);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    returnDecimal = Convert.ToDecimal(cmd.ExecuteScalar());

                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnDecimal;
        }

        public List<Transfer> ViewTransfers(int user_id)
        {
            List<Transfer> transferList = new List<Transfer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT transfer_id, account.user_id AS account_from, (SELECT account.user_id FROM account
                                                    JOIN transfer ON transfer.account_to = account.account_id
                                                    WHERE account.user_id = @user_id )AS account_to, amount, transfer_status_id, transfer_type_id
                                                    FROM transfer
                                                    JOIN account ON account.account_id = transfer.account_to
                                                    JOIN tenmo_user ON account.user_id = tenmo_user.user_id
                                                    WHERE transfer.account_from = (SELECT account_id FROM account WHERE user_id = @user_id)
                                                    UNION
                                                    SELECT transfer_id, (SELECT account.user_id FROM account
                                                    JOIN transfer ON transfer.account_to = account.account_id
                                                    WHERE account.user_id = @user_id ) AS account_from, account.user_id AS account_to, amount, transfer_status_id, transfer_type_id
                                                    FROM transfer
                                                    JOIN account ON account.account_id = transfer.account_from
                                                    JOIN tenmo_user ON account.user_id = tenmo_user.user_id
                                                    WHERE transfer.account_to = (SELECT account_id FROM account WHERE user_id = @user_id)
                                                    ", conn);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Transfer transfer = GetTransfersFromReader(reader);
                        transferList.Add(transfer);

                    }

                }

            }
            catch (SqlException)
            {

                throw;
            }
            return transferList;

        }
        public Transfer SendMoney(int fromId, int toId, decimal money)
        {
            int transferId;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"BEGIN TRANSACTION;
                                                    INSERT INTO
                                                    transfer(account_from, account_to, amount, transfer_status_id, transfer_type_id)
                                                    OUTPUT INSERTED.transfer_id
                                                    VALUES ((SELECT account_id FROM account WHERE user_id = @fromId), (SELECT account_id FROM account WHERE user_id = @toId), @money, 2, 2);
                                                    UPDATE account SET balance -= @money WHERE user_id = @fromId;
                                                    UPDATE account SET balance += @money WHERE user_id = @toId;
                                                    COMMIT;
                                                    ", conn);
                    cmd.Parameters.AddWithValue("@fromId", fromId);
                    cmd.Parameters.AddWithValue("@money", money);
                    cmd.Parameters.AddWithValue("@toId", toId);
                    transferId = Convert.ToInt32(cmd.ExecuteScalar());

                    
                }
                return GetTransferById(transferId);
            }
            catch (Exception)
            {

                throw;
            }
            
        }



        public Transfer GetTransferById(int transferId)
        {

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(@"SELECT transfer_id, account_from, account_to, amount FROM transfer WHERE transfer_id = @transfer_id;", conn);
                    cmd.Parameters.AddWithValue("@transfer_id", transferId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if(reader.Read())
                    {

                        return GetTransfersFromReader(reader);
                    }
                    else
                    {
                        throw new Exception();
                    }
                    

                }

            }
            catch (Exception)
            {

                throw;
            }



        }

        private Transfer GetTransfersFromReader(SqlDataReader reader)
        {

            Transfer transfer = new Transfer();
            transfer.Id = Convert.ToInt32(reader["transfer_id"]);
            transfer.TransferFrom = Convert.ToInt32(reader["account_from"]);
            transfer.TransferTo = Convert.ToInt32(reader["account_to"]);
            transfer.Amount = Convert.ToDecimal(reader["amount"]);

            return transfer;

        }


    }
}

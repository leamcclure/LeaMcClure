namespace TenmoClient.Models
{
    /// <summary>
    /// Model to provide login parameters
    /// </summary>
    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class OtherUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}

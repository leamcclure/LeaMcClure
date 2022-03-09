using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TenmoServer.Models;
using TenmoServer.DAO;


namespace TenmoServer.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        
        private IUserDao userDao;
        public TransferController(IUserDao _userDao)
        {
            this.userDao = _userDao;
        }

        [HttpGet]
        public List<User> GetUsers()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            List<User> returnUsers = userDao.GetOtherUsers(userId);
            return returnUsers;
        }
        [HttpPost("{id}")]
        public int SendCash(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);


            throw new NotImplementedException();
        }    


    }
}

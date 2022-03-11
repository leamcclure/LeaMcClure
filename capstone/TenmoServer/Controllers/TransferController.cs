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
        [HttpPost("{toId}/{money}")]
        public ActionResult<Transfer> SendCash(int toId, decimal money)
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            try
            {
                if(userId == toId)
                {
                    return StatusCode(400);
                }
                else if(userDao.GetCash(userId) < money)
                {
                    return StatusCode(400);
                }
                return userDao.SendMoney(userId, toId, money);
            }
            catch (Exception)
            {
                return StatusCode(500);   
            }
        }
        [HttpPost("request/{toId}/{money}")]
        public ActionResult<Transfer> RequestCash(int toId, decimal money)
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            try
            {
                if (userId == toId)
                {
                    return StatusCode(400);
                }
                else if (money <= 0)
                {
                    return StatusCode(400);
                }
                return userDao.RequestMoney(userId, toId, money);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("history")]
        public List<Transfer> ViewTransfers()
        {
            int user_Id = Convert.ToInt32(User.FindFirst("sub")?.Value);
            return userDao.ViewTransfers(user_Id);
        }
        [HttpGet("{id}")]
        public Transfer GetTransferById(int id)
        {    
            return userDao.GetTransferById(id);
        }

    }
}

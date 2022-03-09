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
    public class AccountController : ControllerBase
    {
        private IUserDao userDao;
        public AccountController(IUserDao _Dao)
        {
            this.userDao = _Dao;

        }

        [HttpGet]
        public decimal GetMoney()
        {
            int user_id = Convert.ToInt32(User.FindFirst("sub")?.Value);
            return userDao.GetCash(user_id);
        }

        [HttpGet]
        public List<Transfer> ViewTransfers()
        {




        }

    }
}

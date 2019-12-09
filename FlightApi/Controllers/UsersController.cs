using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FlightApi.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FlightApi.Controllers
{
    [Route("user/")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly UserContext _context;
        public UsersController(UserContext context)
        {
            _context = context;
        }

        [HttpPost("registration")]
        public async Task<ActionResult<IEnumerable<User>>> PostUser(User registeringUser)
        {
            if (!_context.Users.Any(user => user.UserName.Equals(registeringUser.UserName)))
            {
                Registration.CreateUser(registeringUser);
                await _context.SaveChangesAsync();
            }

            return await _context.Users.ToListAsync();
        
        }
    }
}

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
using System.Net;
using Microsoft.AspNetCore.WebUtilities;

namespace FlightApi.Controllers
{
    [Route("user/")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly UserContext _context;
        public string Secret { get; set; } = "AAAAAAAAAAAAAAAAAAAAAAAAAA==";
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public UsersController(UserContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }



        [HttpPost("registration")]
        public async Task<ActionResult<string>> PostUser(User registeringUser)
        {
            var tokenString = string.Empty;

            if (!_context.Users.Any(user => user.UserName.Equals(registeringUser.UserName)))
            {
                string hashedPassword = HashUserPassword(registeringUser.Password);
                registeringUser.Password = hashedPassword;
                _context.Add(registeringUser);

                tokenString = CreateJWTToken(registeringUser, Secret);

                registeringUser.TokenString = tokenString;
 
                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            return tokenString;
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> PostLogin(User loggingInUser)
        {
            string hashedIncomingPassword = HashUserPassword(loggingInUser.Password);

            User foundUser = _context.Users.Where(user => user.UserName.Equals(loggingInUser.UserName)).FirstOrDefault();

            string tokenString = string.Empty;

            if (foundUser != null && hashedIncomingPassword.Equals(foundUser.Password))
            {
                tokenString = CreateJWTToken(foundUser, Secret);

                loggingInUser.TokenString = tokenString;

                await _context.SaveChangesAsync();
            }
            else
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            return tokenString;
        }

        private string HashUserPassword(string password)
        {
            string hashedPassword = PasswordHasher.CreateSaltedPasswordHash(password);
            return hashedPassword;
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        private static string CreateJWTToken(User user, string secretKey)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            JwtHeader header = new JwtHeader(credentials);

            JwtPayload payload = new JwtPayload();
            payload.Add("firstName", user.FirstName);
            payload.Add("lastName", user.LastName);
            payload.Add("userName", user.UserName);

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(secToken);

            return tokenString;
        }
    }
}

using Nancy.Authentication.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FlightApi
{
    public class PasswordHasher
    {
        private static string Salt = "$2a$10$rBV2JDeWW3.vKyeQcM8fFO";

        public static string CreateSaltedPasswordHash(string password)
        {
            string savedPasswordHash = BCrypt.Net.BCrypt.HashPassword(password, Salt);

            return savedPasswordHash;
        }
    }
}

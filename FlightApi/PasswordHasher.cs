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
            //byte[] salt = new byte[16];
            //new RNGCryptoServiceProvider().GetBytes(salt);

            //var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            //byte[] hash = pbkdf2.GetBytes(20);

            //byte[] hashBytes = new byte[36];
            //Array.Copy(salt, 0, hashBytes, 0, 16);
            //Array.Copy(hash, 0, hashBytes, 16, 20);
            //string savedPasswordHash = Convert.ToBase64String(hashBytes);
            //return savedPasswordHash;
        }
    }
}

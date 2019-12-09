using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightApi
{
    public class Registration
    {
        public void CreateUser(string userName, string password, string firstName, string lastName)
        {
            using (var db = new UserContext())
            {                
                db.Add(new User
                {
                    UserName = userName,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName
                });
                db.SaveChanges();  
            }
        }
    }
}

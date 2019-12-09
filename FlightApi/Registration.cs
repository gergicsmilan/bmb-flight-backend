using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightApi
{
    public class Registration
    {
        public static void CreateUser(User registeringUser)
        {
            using (var db = new UserContext())
            {                
                db.Add(registeringUser); 
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo.TestData
{
    public static class UserServer
    {
        public static User GetUser(string userName)
        {
            return TestUsers.users.FirstOrDefault(t => t.UserName == userName);
        }
    }
}

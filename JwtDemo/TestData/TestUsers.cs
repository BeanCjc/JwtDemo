using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo.TestData
{
    public static class TestUsers
    {
        public static List<User> users = new List<User> {
            new User { Id=Guid.NewGuid().ToString(),UserName="admin",Password="admin123",UserRoles=new List<string> { "Administrator","Leader","Manager","Common"}},
            new User{Id=Guid.NewGuid().ToString(),UserName="Bean",Password="Bean123",UserRoles=new List<string>{"Leader","Manager"}},
            new User{Id=Guid.NewGuid().ToString(),UserName="Lisa",Password="Lisa123",UserRoles=new List<string>{ "Common "}}};
    }
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> UserRoles { get; set; }

    }
}

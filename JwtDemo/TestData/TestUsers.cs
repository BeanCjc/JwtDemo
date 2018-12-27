using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtDemo.TestData
{
    public static class TestUsers
    {
        public static List<User> users = new List<User> {
            new User { Id=Guid.NewGuid().ToString(),UserName="admin",Password="admin123",UserRoles=new List<string> { "Administrator","Leader","Manager","Common"},Age=12},
            new User{Id=Guid.NewGuid().ToString(),UserName="Bean",Password="Bean123",UserRoles=new List<string>{"Leader","Manager"},Age=15},
            new User{Id=Guid.NewGuid().ToString(),UserName="Lisa",Password="Lisa123",UserRoles=new List<string>{ "Common "},Age=18},
            new User{Id=Guid.NewGuid().ToString(),UserName="testuser",Password="testuser123",UserRoles=new List<string>{"username","usernamerole","testuser"},Age=22}};
    }
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<string> UserRoles { get; set; }
        public int Age { get; set; }
    }
}

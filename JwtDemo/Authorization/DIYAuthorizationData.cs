using Microsoft.AspNetCore.Authorization;

namespace JwtDemo.Authorization
{
    public class DIYAuthorizationData:IAuthorizationRequirement
    {
        public DIYAuthorizationData(int age)
        {
            Age = age;
        }
        public int Age { get; set; }

    }
}

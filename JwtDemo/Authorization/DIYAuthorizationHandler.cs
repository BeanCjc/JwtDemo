using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;


namespace JwtDemo.Authorization
{
    public class DIYAuthorizationHandler : AuthorizationHandler<DIYAuthorizationData>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DIYAuthorizationData requirement)
        {
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                return Task.CompletedTask;
            }

            var userAge = TestData.UserServer.GetUser(context.User.Claims.FirstOrDefault(t => t.Type == ClaimTypes.Name).Value).Age;
            //int.TryParse(context.User.Claims.FirstOrDefault(t => t.Type == "age").Value, out int age);
            if (userAge >= requirement.Age)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

            #region 原生写法

            //var httpContext = (context.Resource as AuthorizationFilterContext).HttpContext;
            //var jwtOption = (httpContext.RequestServices.GetService(typeof(IOptions<JwtSetting>)) as IOptions<JwtSetting>).Value;

            #region 认证
            //获取请求头中的Authorization的值
            //var result = httpContext.Request.Headers.TryGetValue("Authorization", out StringValues authStr);
            //if (!result || string.IsNullOrEmpty(authStr))
            //{
            //    return Task.CompletedTask;
            //}
            //var success = true;
            //var jwtarr = authStr.ToString().Substring("Beare ".Length).Trim().Split('.');
            //var jwtHeader = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Decode(jwtarr[0]));
            //var jwtPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Decode(jwtarr[1]));
            //var hs256 = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(jwtOption.SecretKey));

            ////验证签名,一般是必须的
            //success = success && string.Equals(jwtarr[2], Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Encode(hs256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(jwtarr[0] + "." + jwtarr[1]))));
            //if (!success)
            //{
            //    return Task.CompletedTask;
            //}
            ////验证有效期,一般是必须的
            //var now = ToUnixEpochDate(DateTime.UtcNow);
            //success = success && (long.Parse(jwtPayload["nbf"].ToString()) <= now && long.Parse(jwtPayload["exp"].ToString()) >= now);
            //if (!success)
            //{
            //    return Task.CompletedTask;
            //}
            ////其他必须的验证项

            #endregion

            #region 授权


            #endregion
            #endregion

            //return Task.CompletedTask;
        }
        public static long ToUnixEpochDate(DateTime date) =>
            (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
    }
}

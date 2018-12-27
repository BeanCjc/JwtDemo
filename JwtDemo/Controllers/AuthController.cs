using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using JwtDemo.TestData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JwtDemo.Controllers
{
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly JwtSetting jwtSetting;

        /// <summary>
        /// 123
        /// </summary>
        /// <param name="options"></param>
        public AuthController(Microsoft.Extensions.Options.IOptions<JwtSetting> options)
        {
            jwtSetting = options.Value;
        }

        /// <summary>
        /// 验证签发
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <returns></returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /RequestToken
        ///     {
        ///        "UserName": "admin",
        ///        "Password": "admin123"
        ///     }
        /// </remarks>
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult RequestToken([FromBody]TokenRequest request)
        {
            if (ModelState.IsValid)
            {
                var user = UserServer.GetUser(request.UserName);
                if (user == null)
                {
                    return Ok(new { success = true, massage = "账号不存在，请前往注册" });
                }
                if (request.Password == user.Password)
                {
                    //var claims = new[] { new Claim(ClaimTypes.Name, request.UserName) };
                    var claims = new List<Claim>{ new Claim(ClaimTypes.Name, request.UserName)/*,new Claim(ClaimTypes.Role,"admin")*/ };
                    var roleLists = UserServer.GetTestUser(request.UserName);
                    var claimsIdentity = new ClaimsIdentity();
                    if (roleLists!=null&&roleLists.Count>0)
                    {
                        foreach (var role in roleLists)
                        {
                            claimsIdentity.AddClaim(new Claim("name", role));
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                        }
                    }
                    claims.AddRange(claimsIdentity.FindAll("name"));
                    claims.AddRange(claimsIdentity.FindAll(ClaimTypes.Role));
                    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSetting.SecretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    

                    var token = new JwtSecurityToken(issuer: jwtSetting.Issuer, audience: jwtSetting.Audience, claims: claims, expires: DateTime.Now.AddMinutes(20), signingCredentials: creds);
                    return Ok(new { success = true, message = "验证成功，请查看token", token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
                else
                {
                    return Ok("密码错误");
                }
            }
            else
            {
                return BadRequest("数据格式不正确");
            }
        }
    }
    /// <summary>
    /// requestInfo
    /// </summary>
    public class TokenRequest
    {
        /// <summary>
        /// username
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// password
        /// </summary>
        [Required]
        public string Password { get; set; }

    }
}
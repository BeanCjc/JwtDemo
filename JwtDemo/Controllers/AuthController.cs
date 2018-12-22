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
                    var claims = new[] { new Claim(ClaimTypes.Name, request.UserName) };
                    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSetting.SecretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(issuer: jwtSetting.Issuer, audience: jwtSetting.Audience, claims: claims, expires: DateTime.Now.AddMinutes(20), signingCredentials: creds);
                    return Ok(new { success = true, message = "验证成功，请查看token", token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
                else
                {
                    return BadRequest("密码错误");
                }
            }
            else
            {
                return Ok("数据格式不正确");
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
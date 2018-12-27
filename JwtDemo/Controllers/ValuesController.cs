using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Xml;

namespace JwtDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        // GET api/values/5
        /// <summary>
        /// 无需认证的action
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<string>> GetWithOutAuthentication()
        {
            return new string[] { "info", "it is none authentiction." };
        }

        // GET api/values
        /// <summary>
        /// 角色是administrator或Leader的才能访问该action
        /// </summary>
        /// <returns></returns>
        /// <response code="401">认证失败</response>
        /// <response code="403">授权不通过</response>
        [HttpGet]
        [Authorize]
        [Authorize(Policy = "admin")]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        /// <summary>
        /// 自定义授权，年龄大于15的才能访问该action
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Policy = "DIYPolicy")]
        public ActionResult<IEnumerable<string>> GetDIYPolicy()
        {
            return new string[] { "info", "it is a DIY policy." };
        }

        /// <summary>
        /// Function GetTestUserPolicy
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Policy = "testuser")]
        public ActionResult<IEnumerable<string>> GetTestUserPolicy()
        {
            return new string[] { "Info", "it is a testuser policy." };
        }
    }
}

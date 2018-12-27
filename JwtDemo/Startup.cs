using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.IO;
using JwtDemo.MiddleWare;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using JwtDemo.Authorization;
using System.Security.Claims;

namespace JwtDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //注册中间件1
            services.AddTransient<FirstMiddleWare>();
            services.AddTransient<SecondMiddleWare>();



            services.Configure<JwtSetting>(Configuration.GetSection("JwtSetting"));
            //var jwtSetting = new JwtSetting();
            //Configuration.Bind("JwtSetting", jwtSetting);
            var jwtSetting = Configuration.GetSection("JwtSetting").Get<JwtSetting>();
            //var varible = Configuration["JwtSetting"];
            //services.Configure<JwtSetting>(Configuration.GetSection("JwtSetting"));
            #region  Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = jwtSetting.Audience,//Audience
                        ValidIssuer = jwtSetting.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSetting.SecretKey))//拿到SecurityKey
                    };
                    options.SaveToken = true;
                });
            #endregion

            #region Authorization
            services.AddSingleton<IAuthorizationHandler, DIYAuthorizationHandler>();
            services.AddAuthorization(configure =>
            {
                //基于角色的授权，只需在认证的时候将需要的声明添加到角色即可
                configure.AddPolicy("admin", policy => policy.RequireClaim(ClaimTypes.Role, "Administrator", "Leader"));

                //基于声明的授权，只需在认证的时候将需要的声明添加自定义的声明即可，
                //可能会存在数据无法保证实时准确，比如我先登录验证，然后修改用户数据，这时在进行请求便会存在数据差异的情况，故可以采用基于管道或者基于中间件的方式
                configure.AddPolicy("testuser", policy => policy.RequireClaim("name", "testuser"));

                //基于管道的自定义授权策略，写法会复杂一点，但相比中间件会有更好的性能，怎么说呢，基于管道的只有在方法或控制器上有Authorise才会进入管道，而中间件则是都会进入，造成不必要的资源浪费。
                configure.AddPolicy("DIYPolicy", policy => policy.AddRequirements(new Authorization.DIYAuthorizationData(15)));

                //基于中间件的自定义授权策略，更直观。类似中间件1，2，3的写法，只是其中的内容换成管道的授权代码，这里不写


                configure.AddPolicy("user1", policy =>
                    policy.RequireAssertion(context =>
                    {
                        return context.User.HasClaim(c => c.Value == Configuration["qwe"]);
                    }));


            });
            #endregion

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                //注册swaggerAPI文档服务
                options.SwaggerDoc("SwaggerDocName", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Version = "v1.0",
                    Title = "Swagger Title",
                    Description = "Swagger description\r\n" +
                    "Test User Data:\r\n" +
                    "new User { Id = Guid.NewGuid().ToString(), UserName = \"admin\", Password = \"admin123\", UserRoles = new List<string> { \"Administrator\",\"Leader\",\"Manager\",\"Common\"}, Age = 12},\r\n" +
                    "new User { Id = Guid.NewGuid().ToString(), UserName = \"Bean\", Password = \"Bean123\", UserRoles = new List<string> { \"Leader\", \"Manager\" }, Age = 15 },\r\n" +
                    "new User { Id = Guid.NewGuid().ToString(), UserName = \"Lisa\", Password = \"Lisa123\", UserRoles = new List<string> { \"Common \" }, Age = 18 },\r\n" +
                    "new User { Id = Guid.NewGuid().ToString(), UserName = \"testuser\", Password = \"testuser123\", UserRoles = new List<string> { \"username\", \"usernamerole\", \"testuser\" }, Age = 22 }"
                });
                //添加注释
                //var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
                //var basePath = Directory.GetCurrentDirectory();
                //options.IncludeXmlComments(Path.Combine(basePath, "JwtDemo.xml"));
                options.IncludeXmlComments(Path.Combine(Directory.GetCurrentDirectory(), "JwtDemo.xml"));
                options.AddSecurityDefinition("Bearer", new Swashbuckle.AspNetCore.Swagger.ApiKeyScheme { Name = "Authorization", In = "header", Description = "Format: Bearer {auth_token}" });

                //jwt认证方式，此方式为全局添加
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, });
            });
            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseFirstMiddleWare();
            app.UseSecondMiddleWare();
            app.Use(async (content, next) =>
            {
                Console.WriteLine("Third In");
                await next.Invoke();
                Console.WriteLine("Third Out");

            });

            //app.UseStaticFiles();
            app.UseAuthentication();
            app.UseSwagger().UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/SwaggerDocName/swagger.json", "Swagger Endpoint Name");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

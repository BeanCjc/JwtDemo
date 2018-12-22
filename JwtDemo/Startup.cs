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
            #region  添加jwt验证：
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

            #region Swagger
            services.AddSwaggerGen(options =>
            {
                //注册swaggerAPI文档服务
                options.SwaggerDoc("SwaggerDocName", new Swashbuckle.AspNetCore.Swagger.Info { Version = "v1.0", Title = "Swagger Title", Description = "Swagger description" });
                //添加注释
                var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
                options.IncludeXmlComments(Path.Combine(basePath, "JwtDemo.xml"));
                //options.IncludeXmlComments(Path.Combine(Directory.GetCurrentDirectory(), "JwtDemo.xml"));
                options.AddSecurityDefinition("Bearer", new Swashbuckle.AspNetCore.Swagger.ApiKeyScheme { Name = "Authorization", In = "header", Description = "Format: Bearer {auth_token}" });

                //jwt认证方式，此方式为全局添加
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, });
            });
            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);




            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            //    {
            //        option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidateAudience = true,
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            ValidIssuer = "https://localhost:44389",
            //            ValidAudience = "https://localhost:44389",
            //            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
            //        };
            //    });
            //    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace JwtDemo.MiddleWare
{
    public static class SecondMiddleWareExtension
    {
        public static IApplicationBuilder UseSecondMiddleWare(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecondMiddleWare>();
        }
    }
}

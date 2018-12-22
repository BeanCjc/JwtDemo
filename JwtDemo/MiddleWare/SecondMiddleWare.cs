using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JwtDemo.MiddleWare
{
    public class SecondMiddleWare
    {
        private RequestDelegate _next;
        public SecondMiddleWare(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Second In");
            await _next(context);
            Console.WriteLine("Second Out");
        } 
    }
}

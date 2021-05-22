using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Web
{
    public static class Context
    {
        [ContextStatic]
        public static IApplicationBuilder builder;

        /// <summary>
        /// 加载HttpContext对象
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
        {
            return builder = app;
        }

        public static HttpContext Current
        {
            get
            {
                // 未执行赋值方法（在非Web环境中）
                if (builder == null) return null;
                IHttpContextAccessor factory = builder.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
                return factory.HttpContext;
            }
        }
    }
}

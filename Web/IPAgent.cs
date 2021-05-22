using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common.Ioc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Common.Web
{
    /// <summary>
    /// 获取IP
    /// </summary>
    public static class IPAgent
    {

        private static IPHeader header = IocCollection.GetService<IPHeader>();

        /// <summary>
        /// 没有IP地址
        /// </summary>
        private const string NO_IP = "0.0.0.0";

        /// <summary>
        /// 不支持的IP（可能是IPv6）
        /// </summary>
        private const string ERROR_IP = "255.255.255.255";

        /// <summary>
        /// 获取当前访问的IP
        /// </summary>
        public static string IP
        {
            get
            {
                return Context.Current.GetIP();
            }
        }

        /// <summary>
        /// IPv4的正则验证
        /// </summary>
        public static readonly Regex regex = new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}");
        public static string GetIP(this HttpContext context)
        {
            if (context == null) return NO_IP;
            string ip = string.Empty;

            string[] keys = header == null ?
                new[] { "Ali-CDN-Real-IP", "X-Real-IP", "X-Forwarded-IP", "X-Forwarded-For" }
                :
                header.Headers;

            foreach (string key in keys)
            {
                if (key == null || !context.Request.Headers.ContainsKey(key)) continue;
                string value = context.Request.Headers[key];
                if (regex.IsMatch(value))
                {
                    ip = regex.Match(value).Value;
                    break;
                }
            }
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.MapToIPv4().ToString();
            }
            if (!regex.IsMatch(ip))
            {
                ip = ERROR_IP;
            }
            return ip;
        }
    }
}

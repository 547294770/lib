using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Web
{
    /// <summary>
    /// 自定义获取IP的头部信息
    /// </summary>
    public sealed class IPHeader
    {
        internal readonly string[] Headers;

        public IPHeader(string[] headers)
        {
            this.Headers = headers;
        }
    }
}

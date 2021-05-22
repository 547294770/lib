using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    /// <summary>
    /// 错误类型
    /// </summary>
    public enum ErrorType
    {
        [Description("请先登录")]
        Login,
        [Description("没有权限")]
        Permission,
        [Description("请求无效")]
        BadRequest,
        [Description("无效授权")]
        Authorization,
        [Description("系统异常")]
        Exception,
    }
}

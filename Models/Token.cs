using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    /// <summary>
    /// 当前登录的账号信息
    /// </summary>
    public struct UserToken
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        public Guid ID;

        public static implicit operator bool(UserToken user)
        {
            return user.ID != Guid.Empty;
        }

        public static implicit operator Guid(UserToken user)
        {
            return user.ID;
        }
    }
}

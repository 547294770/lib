using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    /// <summary>
    /// 消息传递对象
    /// </summary>
    public class MessageResult
    {

        private List<string> message = new List<string>();

        public void Add(string msg)
        {
            if (!string.IsNullOrEmpty(msg)) this.message.Add(msg);
        }

        public override string ToString()
        {
            return string.Join("\t", message);
        }

        /// <summary>
        /// 默认转化成为字符串
        /// </summary>
        /// <param name="result">当前对象</param>
        /// <returns>JSON</returns>
        public static implicit operator string(MessageResult result)
        {
            return result.ToString();
        }
    }
}

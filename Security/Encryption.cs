using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Security
{
    public static class Encryption
    {
        /// <summary>
        /// MD5编码(32位大写）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding">默认UTF-8</param>
        /// <returns>默认大写</returns>
        public static string toMD5(string input, Encoding encoding = null, int length = 32)
        {
            if (encoding == null) encoding = Encoding.UTF8;
            string md5 = toMD5(encoding.GetBytes(input ?? string.Empty));
            if (length == 32) return md5;
            return md5.Substring(0, length);
        }

        /// <summary>
        /// 获取一个二进制流的MD5值（大寫）
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string toMD5(byte[] buffer)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                byte[] data = md5.ComputeHash(buffer);
                return string.Join(string.Empty, data.Select(t => t.ToString("x2"))).ToUpper();
            }
        }
    }
}

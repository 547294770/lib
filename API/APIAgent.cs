using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Common.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Common.API
{
    /// <summary>
    /// 内部API接口的对接
    /// 务必配置 appsetting.json 文件的 studio:{ uploadUrl:"图片上传接口",imgServer:"图片访问地址" }
    /// </summary>
    public static class APIAgent
    {
        /// <summary>
        /// 图片上传路径
        /// </summary>
        private static string uploadUrl { get; set; }

        /// <summary>
        /// 图片服务器
        /// </summary>
        public static string imgServer { get; private set; }

        static APIAgent()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                  //.SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json")
                  .Build();

            uploadUrl = config["UploadConfig:uploadUrl"];
            imgServer = config["UploadConfig:imgServer"];
        }

        /// <summary>
        /// 从外部设定
        /// </summary>
        public static void Set(string _imgServer, string _uploadUrl)
        {
            if (!string.IsNullOrEmpty(_imgServer)) imgServer = _imgServer;
            if (!string.IsNullOrEmpty(_uploadUrl)) uploadUrl = _uploadUrl;

        }

        /// <summary>
        /// 上传文件流
        /// </summary>
        /// <param name="type">自定义的文件扩展名</param>
        /// <returns>返回上传结果</returns>
        public static Result Upload(this IFormFile file, string type = null)
        {
            if (string.IsNullOrEmpty(uploadUrl)) return new Result(false, "未配置上传路径");
            using (MemoryStream ms = new MemoryStream())
            {
                file.CopyTo(ms);
                byte[] data = ms.ToArray();
                Dictionary<string, string> header = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(type)) header.Add("x-type", type);
                string result = NetAgent.UploadData(uploadUrl, data, Encoding.UTF8, null, header);
                return new Result(ContentType.Result, result);
            }
        }

        /// <summary>
        /// 上传文件流
        /// </summary>
        /// <param name="url">远程文件URL</param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Result Upload(string url, string type = null)
        {
            if (string.IsNullOrEmpty(uploadUrl)) return new Result(false, "未配置上传路径");
            if (!System.Uri.IsWellFormedUriString(url, UriKind.Absolute)) return new Result(false, "资源路径非法");
            byte[] data = NetAgent.DownloadFile(url);
            if (data == null) return new Result(false, "下载文件失败");

            Dictionary<string, string> header = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(type)) header.Add("x-type", type);
            string result = NetAgent.UploadData(uploadUrl, data, Encoding.UTF8, null, header);
            return new Result(ContentType.Result, result);
        }

        public static Result Upload2(string localPath, string type = null)
        {
            byte[] data = File.ReadAllBytes(localPath);
            if (data == null) return new Result(false, "文件不存在");

            Dictionary<string, string> header = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(type)) header.Add("x-type", type);
            string result = NetAgent.UploadData(uploadUrl, data, Encoding.UTF8, null, header);
            return new Result(ContentType.Result, result);
        }

        public static string GetImage(this string path)
        {
            return path.GetImage("/images/space.png");
        }

        /// <summary>
        /// 获取图片路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="defaultPath">如果不存在，默认的图片</param>
        /// <returns></returns>
        public static string GetImage(this string path, string defaultPath)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (string.IsNullOrEmpty(defaultPath)) return defaultPath;
                return $"{imgServer}{defaultPath}";
            }
            if (path.StartsWith("http")) return path;
            return $"{imgServer}{path}";
        }
    }
}

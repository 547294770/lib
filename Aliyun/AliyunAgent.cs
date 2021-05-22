using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.OSS;
using Common.Models;
using Common.Models.Aliyun;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Common.Aliyun
{
    public class AliyunAgent
    {
        internal static OSSConfig _oSSConfig = new OSSConfig();
        internal static string _imgServer;

        public AliyunAgent()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .Build();

            //阿里云oss
            _oSSConfig.AccessKeyId = configuration["AliyunOss:accessKeyId"];
            _oSSConfig.AccessKeySecret = configuration["AliyunOss:accessKeySecret"];
            _oSSConfig.BucketName = configuration["AliyunOss:bucketName"];
            _oSSConfig.EndPoint = configuration["AliyunOss:endpoint"];

            //图片服务
            _imgServer = configuration["UploadConfig:imgServer"];
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="localFileName"></param>
        /// <returns></returns>
        public async Task<Result> UploadImage(IFormFile file)
        {
            string[] extends = new[] { ".jpg", "jpeg", ".png", ".gif" };
            string ext = Path.GetExtension(file.FileName);
            string objectName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{new Random().Next(10000, 99999)}{ext}";

            if (!extends.Contains(ext.ToLower()))
            {
                throw new Exception("只支持文件类型：" + string.Join('|', extends));
            }

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                return UploadFile(objectName, ms);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="localFileName"></param>
        /// <returns></returns>
        public Result UploadFile(string objectName, Stream content)
        {
            var client = new OssClient(_oSSConfig.EndPoint, _oSSConfig.AccessKeyId, _oSSConfig.AccessKeySecret);
            var uploadResult = new Result();

            try
            {
                content.Position = 0;
                var result = client.PutObject(_oSSConfig.BucketName, objectName, content);
                if (result.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    uploadResult.Message = "上传成功";
                    uploadResult.Success = 1;
                    uploadResult.Info = _imgServer.EndsWith("/") ? $"{_imgServer}{objectName}" : $"{_imgServer}/{objectName}";
                }
            }
            catch (Exception ex)
            {
                uploadResult.Success = 0;
                uploadResult.Message = ex.Message;
            }

            return uploadResult;
        }
    }
}

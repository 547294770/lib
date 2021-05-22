using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Common.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Common.Models
{
    public struct Result
    {
        /// <summary>
        /// 要对外输出的状态码
        /// </summary>
        private readonly HttpStatusCode StatusCode;

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public int Success { get; set; }

        /// <summary>
        /// 返回的信息
        /// </summary>
        [JsonProperty(PropertyName = "msg")]
        public string Message { get; set; }

        /// <summary>
        /// 需要返回的对象
        /// </summary>
        [JsonProperty(PropertyName = "info")]
        public object Info { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        [JsonIgnore]
        public bool IsException { get; set; }

        /// <summary>
        /// 要输出的类型
        /// </summary>
        public ContentType? Type
        {
            get
            {
                if (!Enum.IsDefined(typeof(ContentType), this.Success)) return null;
                return (ContentType)this.Success;
            }
        }

        public Result(HttpStatusCode statusCode, int success, string message, object info)
        {
            this.StatusCode = statusCode;
            this.Success = success;
            this.Message = message;
            this.Info = info;
            this.IsException = false;
        }

        public Result(bool success)
            : this(HttpStatusCode.OK, success ? 1 : 0, string.Empty, null)
        { }

        public Result(bool success, string message, object info)
           : this(HttpStatusCode.OK, success ? 1 : 0, message, null)
        {
            this.Info = info;
        }

        public Result(bool success, string message)
          : this(HttpStatusCode.OK, success ? 1 : 0, message, null)
        {
        }

        public Result(int success, string message)
            : this(HttpStatusCode.OK, success, message, null)
        {

        }

        /// <summary>
        /// 输出自定义类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="info"></param>
        public Result(ContentType type, object info)
            : this(HttpStatusCode.OK, 0, null, info)
        {
            this.Success = (int)type;
            this.Message = null;
            this.Info = info;
            this.IsException = false;

            switch (this.Type.Value)
            {
                case ContentType.Result:
                    try
                    {
                        JObject obj = JObject.Parse((string)this.Info);
                        this.Success = obj["success"].Value<int>();
                        this.Message = obj["msg"].Value<string>();
                        this.Info = obj["info"];
                    }
                    catch
                    {
                        this.Success = 0;
                        this.Message = (string)this.Info;
                    }
                    break;
            }
        }

        /// <summary>
        /// 原样输出内容
        /// </summary>
        /// <param name="message"></param>
        public Result(string message) : this(ContentType.HTML, message)
        {

        }

        /// <summary>
        /// 默认转化成为字符串
        /// </summary>
        /// <param name="result">当前对象</param>
        /// <returns>JSON</returns>
        public static implicit operator string(Result result)
        {
            return result.ToString();
        }

        /// <summary>
        /// 字符串输出（原样输出）
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator Result(string result)
        {
            return new Result(result);
        }

        /// <summary>
        /// 判断当前返回是成功还是失败
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator bool(Result result)
        {
            return result.Success == 1;
        }

        /// <summary>
        /// 布尔型的转换
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator Result(bool result)
        {
            return new Result(result);
        }

        /// <summary>
        /// 转化成为 MVC ResultAction 内容
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator ContentResult(Result result)
        {
            ContentType type = result.Type ?? ContentType.JSON;
            return new ContentResult()
            {
                StatusCode = (int)result.StatusCode,
                ContentType = "application/json",// type.GetAttribute<DescriptionAttribute>().Description,
                Content = result.ToString(),
            };
        }

        public override string ToString()
        {
            //if (this.Success == -1) return this.Message;
            if (this.Type == null)
            {
                if (Info != null && Info.GetType() == typeof(string))
                {
                    return string.Concat("{", "\"success\":", this.Success, ",\"msg\":\"", this.Message, "\",\"info\":", this.Info, "}");
                }
            }
            else
            {
                switch (this.Type.Value)
                {
                    case ContentType.HTML:
                    case ContentType.XML:
                    case ContentType.TEXT:
                    case ContentType.JS:
                    case ContentType.M3U8:
                    case ContentType.CSS:
                        return (string)this.Info;
                    case ContentType.JSON:
                        if (this.Info.GetType().Name == "String")
                        {
                            return (string)this.Info;
                        }
                        else
                        {
                            return JsonConvert.SerializeObject(this.Info);
                        }
                }
            }

            var serSetting = new JsonSerializerSettings();
            return JsonConvert.SerializeObject(this, serSetting);
        }
    }

    public enum ContentType
    {
        [Description("text/html")]
        HTML = -1,
        [Description("image/jpeg")]
        JPEG = 100,
        [Description("image/png")]
        PNG = 101,
        [Description("image/gif")]
        GIF = 102,
        [Description("text/xml")]
        XML = 103,
        [Description("text/paint")]
        TEXT = 105,
        [Description("application/json")]
        JSON = 106,
        /// <summary>
        /// 把Result字符串转化成为Result对象
        /// </summary>
        [Description("application/json")]
        Result = 107,
        /// <summary>
        /// gzip压缩之后的json内容
        /// </summary>
        [Description("application/json")]
        GZIP = 108,
        /// <summary>
        /// JS脚本输出
        /// </summary>
        [Description("application/x-javascript")]
        JS = 109,
        [Description("application/vnd.apple.mpegurl")]
        M3U8 = 110,
        [Description("video/mp2t")]
        TS = 111,
        /// <summary>
        /// 样式
        /// </summary>
        [Description("text/css")]
        CSS = 112,
        /// <summary>
        /// PDF文档
        /// </summary>
        [Description("application/pdf")]
        PDF = 113,
        /// <summary>
        /// 301跳转
        /// </summary>
        [Description("Redirect")]
        Redirect = 114
    }
}

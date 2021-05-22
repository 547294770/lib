using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Http;
using Common.Ioc;
using Common.Json;
using Common.Models;
using Common.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Common.Mvc
{
    public abstract class MvcControllerBase : ControllerBase
    {
        /// <summary>
        /// 只读数据库
        /// </summary>
        protected virtual IReadRepository ReadDB => IocCollection.GetService<IReadRepository>();

        /// <summary>
        /// 可读/可写数据库（不建议在Controller进行可写操作）
        /// </summary>
        protected virtual IWriteRepository WriteDB => IocCollection.GetService<IWriteRepository>();
    
        /// <summary>
        /// 当前HTTP请求的上下文对象
        /// </summary>
        protected virtual HttpContext context
        {
            get
            {
                return this.HttpContext;
            }
        }

        private Stopwatch Stopwatch { get; }

        public MvcControllerBase()
        {
            this.Stopwatch = this.context.GetItem<Stopwatch>();
            if (Stopwatch == null)
            {
                this.Stopwatch = new Stopwatch();
                this.Stopwatch.Start();
            }
        }

        /// <summary>
        /// 本次任务的执行时间
        /// </summary>
        /// <returns></returns> 
        protected virtual string StopwatchMessage()
        {
            Stopwatch.Stop();
            return string.Concat(Stopwatch.ElapsedMilliseconds, "ms");
        }


        protected virtual string QF(string name)
        {
            return this.context.QF(name);
        }

        protected virtual T QF<T>(string name, T defaultValue)
        {
            return this.context.QF<T>(name, defaultValue);
        }

        protected virtual int PageIndex
        {
            get
            {
                return this.context.QF("PageIndex", 1);
            }
        }

        protected virtual int PageSize
        {
            get
            {
                return this.context.QF("PageSize", 20);
            }
        }

        /// <summary>
        /// 自动判断成功状态的输出
        /// </summary>
        /// <param name="success"></param>
        /// <param name="successMessage"></param>
        /// <param name="info">如果状态为成功需要输出的对象</param>
        /// <returns></returns>
        protected virtual Result GetResultContent(bool success, string successMessage = "处理成功", object info = null)
        {
            if (success) return new Result(success, successMessage, info);
            string message = (MessageResult)this.context.RequestServices.GetService(typeof(MessageResult));
            if (string.IsNullOrEmpty(message)) message = "发生不可描述的错误";
            return new Result(false, message);
        }

        /// <summary>
        /// 输出一个成功的JSON数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected virtual Result GetResultContent(object data)
        {
            return new Result(true, this.StopwatchMessage(), data);
        }

        protected virtual string GetResultContent<T, TOutput>(IOrderedQueryable<T> list, Func<T, TOutput> converter = null, object data = null, Action<IEnumerable<T>> action = null) where TOutput : class
        {
            if (converter == null) converter = t => t as TOutput;
            string json = string.Empty;
            IEnumerable<T> query;
            int recordCount = list.Count();
            if (this.PageIndex == 1)
            {
                query = list.Take(this.PageSize).ToArray();
            }
            else
            {
                query = list.Skip((this.PageIndex - 1) * this.PageSize).Take(this.PageSize).ToArray();
            }
            action?.Invoke(query);
            if (converter == null)
            {
                json = query.ToJson();
            }
            else
            {
                if (typeof(TOutput).Name == "String")
                {
                    json = string.Concat("[", string.Join(",", query.Select(converter)), "]");
                }
                else
                {
                    json = query.Select(converter).ToJson();
                }
            }
            return string.Concat("{",
                $"\"RecordCount\":{ recordCount },",
                $"\"PageIndex\":{this.PageIndex},",
                $"\"PageSize\":{this.PageSize},",
                $"\"data\":{ (data == null ? "null" : data.ToJson()) },",
                $"\"list\":{json}",
                "}");
        }


        /// <summary>
        /// 返回排序数组（自带分页）
        /// 包含 RecordCount \ PageIndex \ PageSize
        /// </summary>
        /// <param name="action">提前对分页内内容的处理方法</param>
        /// <returns>返回内容JSON</returns>
        protected virtual Result GetResultList<T, TOutput>(IOrderedQueryable<T> list, Func<T, TOutput> converter = null, object data = null, Action<IEnumerable<T>> action = null) where TOutput : class
        {
            string resultData = this.GetResultContent(list, converter, data, action);
            return this.GetResultContent(resultData);
        }

        /// <summary>
        /// 分页输出（指定分页参数）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="list"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="converter"></param>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        protected virtual Result GetResultList<T, TOutput>(IOrderedQueryable<T> list, int pageindex, int pagesize, Func<T, TOutput> converter = null, object data = null, Action<IEnumerable<T>> action = null) where TOutput : class
        {
            if (converter == null) converter = t => t as TOutput;
            StringBuilder sb = new StringBuilder();
            string json = null;
            IEnumerable<T> query;
            if (pageindex == 1)
            {
                query = list.Take(pagesize).ToArray();
            }
            else
            {
                query = list.Skip((pageindex - 1) * pagesize).Take(pagesize).ToArray();
            }
            action?.Invoke(query);
            if (converter == null)
            {
                json = query.ToJson();
            }
            else
            {
                if (typeof(TOutput).Name == "String")
                {
                    json = string.Concat("[", string.Join(",", query.Select(converter)), "]");
                }
                else
                {
                    json = query.Select(converter).ToJson();
                }
            }
            return this.GetResultContent(string.Concat("{",
                $"\"RecordCount\":{ list.Count() },",
                $"\"PageIndex\":{this.PageIndex},",
                $"\"PageSize\":{this.PageSize},",
                $"\"data\":{ (data == null ? "null" : data.ToJson()) },",
                $"\"list\":{json}",
                "}"));
        }
    }
}

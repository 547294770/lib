using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Common.Ioc;
using Common.Models;
using Common.Repository;
using Common.Web;
using Microsoft.AspNetCore.Http;

namespace Common.Data
{
    /// <summary>
    /// 逻辑类/数据库连接基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DbAgent<T> : IDisposable where T : class, new()
    {
        private readonly string ConnectionString;

        /// <summary>
        /// 只读操作对象
        /// </summary>
        protected virtual IReadRepository ReadDB => IocCollection.GetService<IReadRepository>();

        /// <summary>
        /// 可写可读操作对象
        /// </summary>
        protected virtual IWriteRepository WriteDB => IocCollection.GetService<IWriteRepository>();

        /// <summary>
        /// 数据库类型
        /// </summary>
        private readonly DatabaseType DBType;

        protected DbAgent(string dbConnection, DatabaseType dbType = DatabaseType.SqlServer)
        {
            this.ConnectionString = dbConnection;
            this.DBType = dbType;
        }

        protected DbExecutor NewExecutor(IsolationLevel tranLevel = IsolationLevel.Unspecified)
        {
            return this.NewExecutor(this.ConnectionString, tranLevel);
        }

        protected DbExecutor NewExecutor(string connectionString, IsolationLevel tranLevel = IsolationLevel.Unspecified)
        {
            return DbFactory.CreateExecutor(connectionString, this.DBType, tranLevel);
        }

        /// <summary>
        /// 当前的httpContext对象（如果非web程序则为null）
        /// </summary>
        protected virtual HttpContext context
        {
            get
            {
                return Context.Current;
            }
        }

        #region ========== Message的传递处理 ===========

        /// <summary>
        /// 消息体（不支持非Web环境)
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual MessageResult Message(string msg = null)
        {
            if (this.context == null) return default;
            MessageResult result = (MessageResult)this.context.RequestServices.GetService(typeof(MessageResult));
            if (!string.IsNullOrEmpty(msg)) result.Add(msg);
            return result;
        }

        /// <summary>
        /// 返回错误信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual bool FaildMessage(string msg)
        {
            return this.FaildMessage(msg, false);
        }

        /// <summary>
        /// 错误信息（多语种，msg一定要使用format格式）
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual bool FaildMessage(string msg,params object[] args)
        {
            return this.FaildMessage(msg, false);
        }

        protected virtual TFaild FaildMessage<TFaild>(string msg,TFaild faildValue, params object[] args)
        {
            this.Message(string.Format(msg, args));
            return faildValue;
        }

        /// <summary>
        /// 使用枚举作为错误信息传递对象
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool FaildMessage<TEnum>(TEnum value) where TEnum : IComparable, IFormattable, IConvertible
        {
            this.Message(value.ToString());
            return false;
        }

        #endregion

        private static T _instance;
        public static T Instance()
        {
            if (_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }


        #region ========  公共的通用方法（工具）  ========

        #endregion

        protected DbParameter NewParam(string parameterName, object value)
        {
            return this.DBType.NewParam(parameterName, value);
        }

        public void Dispose()
        {

        }
    }
}

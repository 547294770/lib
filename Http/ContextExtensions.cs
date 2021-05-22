using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Types;
using Microsoft.AspNetCore.Http;

namespace Common.Http
{
    public static class ContextExtensions
    {

        public static T GetItem<T>(this HttpContext context)
        {
            if (context == null || !context.Items.ContainsKey(typeof(T))) return default;
            return (T)context.Items[typeof(T)];
        }

        public static void SetItem<T>(this HttpContext context, T value)
        {
            if (context == null) return;
            Type key = typeof(T);
            if (!context.Items.ContainsKey(key))
            {
                context.Items.Add(key, value);
            }
            else
            {
                context.Items[key] = value;
            }
        }

        public static string QF(this HttpContext context, string key)
        {
            if (context.Request.Method == "POST" && context.Request.HasFormContentType && context.Request.ContentLength != null && context.Request.Form.ContainsKey(key))
            {
                return context.Request.Form[key];
            }

            return null;
        }

        public static T QF<T>(this HttpContext context, string key, T t)
        {
            string value = context.QF(key);
            if (string.IsNullOrEmpty(value)) return t;
            return value.IsType<T>() ? value.GetValue<T>() : t;
        }
    }
}

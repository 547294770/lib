using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Types
{
    public static class TypeExtension
    {
        public static bool HasAttribute<T>(this Object obj) where T : Attribute
        {
            ICustomAttributeProvider custom = obj is ICustomAttributeProvider ? (ICustomAttributeProvider)obj : (ICustomAttributeProvider)obj.GetType();
            foreach (var t in custom.GetCustomAttributes(false))
            {
                if (t.GetType().Equals(typeof(T))) return true;
            }
            return false;
        }

        public static T GetAttribute<T>(this object obj)
        {
            if (obj == null) return default;
            ICustomAttributeProvider custom = obj is ICustomAttributeProvider ? (ICustomAttributeProvider)obj : (ICustomAttributeProvider)obj.GetType();
            foreach (object t in custom.GetCustomAttributes(true))
            {
                if (t.GetType().Equals(typeof(T))) return (T)t;
            }
            return default(T);
        }

        /// <summary>
        /// 把字符串类型转化成为安全类型（如果错误则返回默认值）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T GetValue<T>(this object value)
        {
            return (T)value.GetValue(typeof(T));
        }

        /// <summary>
        /// 转化成为安全类型
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetValue(this object value, Type type = null)
        {
            if (type == null) type = value.GetType();
            if (value == null || value == DBNull.Value) return type.GetDefaultValue();
            object obj = null;
            switch (value.GetType().Name)
            {
                case "DateTime":
                    obj = (DateTime)value == DateTime.MinValue ? DateTime.Parse("1900-1-1") : value;
                    break;
                case "String":
                    switch (type.Name)
                    {
                        case "Guid":
                            if (Guid.TryParse((string)value, out Guid guid))
                            {
                                obj = guid;
                            }
                            else
                            {
                                obj = Guid.Empty;
                            }
                            break;
                        case "Boolean":
                            obj = value.ToString().Equals("1") || value.ToString().Equals("true", StringComparison.CurrentCultureIgnoreCase) || value.ToString().Equals("on", StringComparison.CurrentCultureIgnoreCase);
                            break;
                        case "Decimal":
                            obj = decimal.TryParse((string)value, out decimal decimalValue) ? decimalValue : 0.00M;
                            break;
                        case "Double":
                            obj = double.TryParse((string)value, out double doubleValue) ? doubleValue : 0.00D;
                            break;
                        case "Int64":
                            obj = long.TryParse((string)value, out long longValue) ? longValue : (long)0;
                            break;
                        case "Int32":
                            obj = int.TryParse((string)value, out int intValue) ? intValue : 0;
                            break;
                        case "Int16":
                        case "Short":
                            obj = short.TryParse((string)value, out short shortValue) ? shortValue : (short)0;
                            break;
                        case "Single":
                            obj = float.TryParse((string)value, out float floatValue) ? floatValue : 0;
                            break;
                        case "Byte":
                            byte byteValue;
                            obj = byte.TryParse((string)value, out byteValue) ? byteValue : (byte)0;
                            break;
                        case "DateTime":
                            DateTime dateTime;
                            obj = DateTime.TryParse((string)value, out dateTime) ? dateTime : new DateTime(1900, 1, 1);
                            break;
                        //case "Int32[]":
                        //case "System.Int32[]":
                        //    obj = ((string)value).GetArray<int>().ToArray();
                        //    break;
                        //case "String[]":
                        //    obj = ((string)value).GetArray<string>().ToArray();
                        //    break;
                        //case "String":
                        //    if (value == null) obj = string.Empty;
                        //    break;
                        //default:
                        //    if (type.IsBaseType<ISetting>())
                        //    {
                        //        obj = Activator.CreateInstance(type, (string)value);
                        //    }
                        //    else if (type.IsEnum)
                        //    {
                        //        obj = ((string)value).ToEnum(type);
                        //    }
                        //    else if (type.IsArray)
                        //    {
                        //        Type elemType = type.GetElementType();
                        //        string[] values = ((string)value).Split(',');
                        //        System.Array array = System.Array.CreateInstance(elemType, values.Length);
                        //        for (int i = 0; i < array.Length; i++)
                        //        {
                        //            array.SetValue(values[i].GetValue(elemType), i);
                        //        }
                        //        obj = array;
                        //    }
                        //    break;
                    }
                    break;
                //default:
                //    if (type.IsBaseType<ISetting>()) obj = Activator.CreateInstance(type, value.ToString());
                //    break;
            }
            return obj == null ? value : obj;
        }

        internal static object GetDefaultValue(this Type type)
        {
            if (type == typeof(string)) return string.Empty;
            if (type == typeof(Guid)) return Guid.Empty;
            if (type == typeof(object[]) || type == typeof(int[])) return null;
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0}\nType:{1}", ex.Message, type.FullName));
            }
        }


        public static bool IsType(this string value, Type type)
        {
            bool isType;
            switch (type.Name)
            {
                case "Int32":
                    int int32;
                    isType = int.TryParse(value, out int32);
                    break;
                case "Int16":
                    short int16;
                    isType = short.TryParse(value, out int16);
                    break;
                case "Int64":
                    long int64;
                    isType = long.TryParse(value, out int64);
                    break;
                case "Guid":
                    Guid guid;
                    isType = Guid.TryParse(value, out guid);
                    break;
                case "DateTime":
                    DateTime dateTime;
                    isType = DateTime.TryParse(value, out dateTime);
                    break;
                case "Decimal":
                    decimal money;
                    isType = Decimal.TryParse(value, out money);
                    break;
                case "Double":
                    double doubleValue;
                    isType = Double.TryParse(value, out doubleValue);
                    break;
                case "String":
                    isType = true;
                    break;
                case "Boolean":
                    isType = Regex.IsMatch(value, "1|0|true|false", RegexOptions.IgnoreCase);
                    break;
                case "Byte":
                    byte byteValue;
                    isType = byte.TryParse(value, out byteValue);
                    break;
                default:
                    if (type.IsEnum)
                    {
                        isType = Enum.IsDefined(type, value);
                    }
                    else
                    {
                        throw new Exception("WebAgent.IsType 方法暂时未能检测该种类型" + type.FullName);
                    }
                    break;
            }
            return isType;
        }

        public static bool IsType<T>(this string value)
        {
            return IsType(value, typeof(T));
        }

        /// <summary>
        /// 得到安全类型
        /// 1、不能为空
        /// 2、日期大于1900
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetSafeValue(this object value, Type type)
        {
            return type.Name switch
            {
                "String" => value ?? string.Empty,
                "DateTime" => ((DateTime)value).Max(),
                _ => value
            };
        }
    }
}

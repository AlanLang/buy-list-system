using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

namespace sliontek_web
{
    /// <summary>
    /// 查询序列化
    /// 创建者：郎文达
    /// 创建时间：2018年1月23日
    /// </summary>
    public static class WhereDynami
    {
        /// <summary>
        /// 查询、排序、分页
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="nameValues"></param>
        /// <returns></returns>
        public static IQueryable<TSource> SearchPage<TSource>(this IQueryable<TSource> source,
                        NameValueCollection nameValues, out int total) where TSource : class
        {
            total = source.WhereDynamic(nameValues).Count();
            return source.WhereDynamic(nameValues).order(nameValues).Page(nameValues);
        }


        /// <summary>通过页面控件动态构建查询</summary>
        public static IQueryable<TSource> WhereDynamic<TSource>(this IQueryable<TSource> source,
                                NameValueCollection nameValues) where TSource : class
        {
            var condition = nameValues["condition"];
            if (!string.IsNullOrEmpty(condition))
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                List<JsonCondition> list = (List<JsonCondition>)jss.Deserialize(condition, typeof(List<JsonCondition>));
                //构建 c=>Body中的c
                ParameterExpression param = Expression.Parameter(typeof(TSource), "c");
                //构建c=>Body中的Body
                var body = GetExpressoinBody(param, list);
                if (body != null)
                {
                    //将二者拼为c=>Body
                    var expression = Expression.Lambda<Func<TSource, bool>>(body, param);
                    //传到Where中当做参数，类型为Expression<Func<T,bool>>
                    return source.Where(expression);
                }
            }
            return source;
        }

        private static Expression NewExpression(Expression e1, Expression e2, JsonCondition.SOP op)
        {
            switch (op)
            {
                case JsonCondition.SOP.like:
                    return Expression.Call(e1, "Contains", null, new Expression[] { e2 });
                case JsonCondition.SOP.equal:
                    return Expression.Equal(e1, e2);
                case JsonCondition.SOP.notequal:
                    return Expression.NotEqual(e1, e2);
                case JsonCondition.SOP.startwith:
                    return Expression.Call(e1, "StartsWith", null, new Expression[] { e2 });
                case JsonCondition.SOP.endwith:
                    return Expression.Call(e1, "EndsWith", null, new Expression[] { e2 });
                case JsonCondition.SOP.more:
                    return Expression.GreaterThan(e1, e2);
                case JsonCondition.SOP.less:
                    return Expression.LessThan(e1, e2);
                case JsonCondition.SOP.moreequal:
                    return Expression.GreaterThanOrEqual(e1, e2);
                case JsonCondition.SOP.lessequal:
                    return Expression.LessThanOrEqual(e1, e2);
                default:
                    return Expression.Call(e1, "Contains", null, new Expression[] { e2 });
            }
        }

        private static Expression GetExpressoinBody(ParameterExpression param, List<JsonCondition> conditions)
        {
            var Exps = new List<Expression>();
            var plist = param.Type.GetRuntimeProperties().ToDictionary(z => z.Name);//可以加缓存改善性能
            foreach (var item in conditions)
            {
                if (item.type == JsonCondition.SType.text)
                {
                    if (!string.IsNullOrEmpty(item.value) && plist.ContainsKey(item.name))
                    {
                        var e1 = Expression.Property(param, item.name);
                        var e2 = Expression.Constant(item.value);
                        var rType = plist[item.name].GetMethod.ReturnType;
                        if (rType != typeof(string))
                        {
                            object dValue;
                            if (TryParser(item.value, rType, out dValue))
                            {
                                e2 = Expression.Constant(dValue);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        Exps.Add(NewExpression(e1, e2, item.op));
                    }
                    else if (string.IsNullOrEmpty(item.value) && plist.ContainsKey(item.name))
                    {
                        var e1 = Expression.Property(param, item.name);
                        if (JsonCondition.SOP.equal == item.op)
                        {
                            Exps.Add(Expression.Equal(e1, Expression.Constant(null, e1.Type)));
                        }
                        if (JsonCondition.SOP.notequal == item.op)
                        {
                            Exps.Add(Expression.NotEqual(e1, Expression.Constant(null, e1.Type)));
                        }
                    }
                }
                if (item.type == JsonCondition.SType.date)
                {
                    var e1 = Expression.Property(param, item.name);
                    DateTime dt = DateTime.Now.Date;
                    DateTime.TryParse(item.value, out dt);
                    if (item.op == JsonCondition.SOP.equal)
                    {
                        Exps.Add(Expression.LessThanOrEqual(e1, Expression.Constant(dt.AddDays(1))));
                        Exps.Add(Expression.GreaterThanOrEqual(e1, Expression.Constant(dt)));
                    }
                    //TODO 日期动态查询
                }
            }
            return Exps.Count > 0 ? Exps.Aggregate(Expression.AndAlso) : null;
        }
        /// <summary>构建body</summary>
        private static Expression GetExpressoinBody(ParameterExpression param, NameValueCollection nameValues)
        {
            var list = new List<Expression>();
            if (nameValues.Count > 0)
            {
                var plist = param.Type.GetRuntimeProperties().ToDictionary(z => z.Name);//可以加缓存改善性能
                foreach (var item in nameValues.AllKeys)
                    if (item.EndsWith(">"))//可能大小查询
                    {
                        string key = item.TrimEnd('>');
                        if (!plist.ContainsKey(key) || nameValues[item].Length <= 0) continue;
                        var rType = plist[key].GetMethod.ReturnType;
                        if (rType == typeof(string)) continue;
                        var e1 = Expression.Property(param, key);
                        object dValue;
                        if (TryParser(nameValues[item], rType, out dValue))
                            list.Add(Expression.GreaterThan(e1, Expression.Constant(dValue)));
                    }
                    else if (item.EndsWith("<"))//可能大小查询
                    {
                        string key = item.TrimEnd('<');
                        if (!plist.ContainsKey(key) || nameValues[item].Length <= 0) continue;
                        var rType = plist[key].GetMethod.ReturnType;
                        if (rType == typeof(string)) continue;
                        var e1 = Expression.Property(param, key);
                        object dValue;
                        if (TryParser(nameValues[item], rType, out dValue))
                        {
                            if (rType == typeof(DateTime)) dValue = ((DateTime)dValue).AddDays(1);
                            list.Add(Expression.LessThan(e1, Expression.Constant(dValue)));
                        }
                    }
                    else if (plist.ContainsKey(item) && nameValues[item].Length > 0)
                    {
                        var e1 = Expression.Property(param, item);
                        var rType = plist[item].GetMethod.ReturnType;
                        if (rType == typeof(string))//可能是like查询
                        {
                            var value = nameValues[item].Trim('%');
                            var e2 = Expression.Constant(value);
                            if (nameValues[item].Length - value.Length >= 2)
                                list.Add(Expression.Call(e1, "Contains", null, new Expression[] { e2 }));
                            else if (nameValues[item].StartsWith("%"))
                                list.Add(Expression.Call(e1, "EndsWith", null, new Expression[] { e2 }));
                            else if (nameValues[item].EndsWith("%"))
                                list.Add(Expression.Call(e1, "StartsWith", null, new Expression[] { e2 }));
                            else
                                list.Add(Expression.Equal(e1, e2));
                        }

                        else if (nameValues[item].IndexOf(",") > 0)//可能是in查询
                        {
                            if (rType == typeof(short))
                            {
                                var searchList = TryParser<short>(nameValues[item]);
                                if (searchList.Any())
                                    list.Add(Expression.Call(Expression.Constant(searchList), "Contains", null, new Expression[] { e1 }));
                            }
                            else if (rType == typeof(int))
                            {
                                var searchList = TryParser<int>(nameValues[item]);
                                if (searchList.Any())
                                    list.Add(Expression.Call(Expression.Constant(searchList), "Contains", null, new Expression[] { e1 }));
                            }
                            else if (rType == typeof(long))
                            {
                                var searchList = TryParser<long>(nameValues[item]);
                                if (searchList.Any())
                                    list.Add(Expression.Call(Expression.Constant(searchList), "Contains", null, new Expression[] { e1 }));
                            }
                        }
                        else
                        {
                            object dValue;
                            if (TryParser(nameValues[item], rType, out dValue))
                                list.Add(Expression.Equal(e1, Expression.Constant(dValue)));
                        }
                    }
            }
            return list.Count > 0 ? list.Aggregate(Expression.AndAlso) : null;
        }
        private static List<T> TryParser<T>(string value)
        {
            string[] searchArray = value.Split(',');
            List<T> dList = new List<T>();
            foreach (var l in searchArray)
            {
                try
                {
                    T dValue = (T)Convert.ChangeType(l, typeof(T));
                    dList.Add(dValue);
                }
                catch { }
            }
            return dList;
        }
        private static bool TryParser(string value, Type outType, out object dValue)
        {
            try
            {
                dValue = Convert.ChangeType(value, outType);
                return true;
            }
            catch
            {
                dValue = null;
                return false;
            }
        }

        public static IQueryable<TSource> order<TSource>(this IQueryable<TSource> source, NameValueCollection nameValues) where TSource : class
        {
            string field = nameValues["field"];
            string order = nameValues["order"];
            PropertyInfo property;
            Type type = typeof(TSource);
            ParameterExpression param = Expression.Parameter(type, "c");
            string callMethod = "OrderByDescending";
            if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(order))
            {
                callMethod = "asc".Equals(order) ? "OrderBy" : "OrderByDescending";
                property = type.GetProperty(field) ?? type.GetRuntimeProperties().First();
            }
            else
            {
                property = type.GetRuntimeProperties().First();
            }
            LambdaExpression le = Expression.Lambda(Expression.MakeMemberAccess(param, property), param);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), callMethod, new[] { type, property.PropertyType }
            , source.Expression, Expression.Quote(le));
            return source.Provider.CreateQuery<TSource>(resultExp);
        }

        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, NameValueCollection nameValues) where TSource : class
        {
            string page = nameValues["page"];
            string limit = nameValues["limit"];
            if (!string.IsNullOrEmpty(page) && !string.IsNullOrEmpty(limit))
            {
                return source.Skip((Int32.Parse(page) - 1) * Int32.Parse(limit)).Take(Int32.Parse(limit));
            }
            else
            {
                return null;
            }
        }
    }
}
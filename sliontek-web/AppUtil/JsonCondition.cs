using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sliontek_web
{
    /// <summary>
    /// 返回结果类型定义
    /// 创建者：郎文达
    /// 创建时间：2018年1月23日
    /// </summary>
    public class JsonCondition
        {
            public enum SOP
            {
                /// <summary>
                /// 模糊查询
                /// </summary>
                like,
                /// <summary>
                /// 等于(精确查询)
                /// </summary>
                equal,
                /// <summary>
                /// 不等于
                /// </summary>
                notequal,
                /// <summary>
                /// 以XX开始，日
                /// </summary>
                startwith,
                /// <summary>
                /// 以XX结束，
                /// </summary>
                endwith,
                /// <summary>
                /// 大于 >
                /// </summary>
                more,
                /// <summary>
                /// 小于 <
                /// </summary>
                less,
                /// <summary>
                /// 大于等于>=
                /// </summary>
                moreequal,
                /// <summary>
                /// 小于等于<=
                /// </summary>
                lessequal
            }
            public enum SType { text, number, date, datetime }
            public SOP op { get; set; }
            public string field { get; set; }
            public string name { get; set; }
            public string value { get; set; }
            public SType type { get; set; }
            public string space
            {
                get
                {
                    return " ";
                }
            }
            private string getValueByType()
            {
                switch (type)
                {
                    case SType.text:
                    default:
                        return "'" + value + "'";
                    case SType.datetime:
                        return "to_date('" + value + "', 'yyyy-MM-dd HH24:MI:SS')";
                    case SType.date:
                        //return "to_date('" + value + "', 'yyyy-MM-dd')";
                        return "convert(varchar(10), '" + value + "', 112)";
                    case SType.number:
                        return value;
                }
            }
            public string toWhereString()
            {
                if (string.IsNullOrEmpty(value))
                    return string.Empty;
                switch (op)
                {
                    case SOP.like:
                        return likewhere();
                    case SOP.notequal:
                        return space + field + space + name + "<>" + getValueByType();
                    case SOP.startwith:
                        if (type == SType.date || type == SType.datetime)
                            return space + field + space + name + ">= " + getValueByType();
                        else
                            return field + " like '" + value + "%'";
                    case SOP.endwith:
                        if (type == SType.date || type == SType.datetime)
                            return space + field + space + name + "<= " + getValueByType();
                        else
                            return space + field + space + name + " like '%" + value + "'";
                    case SOP.more:
                        return space + field + space + name + ">" + getValueByType();
                    case SOP.moreequal:
                        return space + field + space + name + ">=" + getValueByType();
                    case SOP.less:
                        return space + field + space + name + "<" + getValueByType();
                    case SOP.lessequal:
                        return space + field + space + name + "<=" + getValueByType();
                    case SOP.equal:
                        return equalwhere();
                    default:
                        return equalwhere();
                }
            }

            public string likewhere()
            {
                if (name.Contains("|"))
                {
                    field = "or";
                    string[] ashift = name.Split('|');
                    string re = " ";
                    for (int i = 0; i < ashift.Length; i++)
                    {
                        re += space + (i == 0 ? "and (" : field) + space + ashift[i] + " like '%" + value + "%'";
                    }
                    re += ")";
                    return re;
                }
                else
                {
                    return space + field + space + name + " like '%" + value + "%'";
                }
            }

            public string equalwhere()
            {
                if (name.Contains("|"))
                {
                    field = "or";
                    string[] ashift = name.Split('|');
                    string re = "(";
                    for (int i = 0; i < ashift.Length; i++)
                    {
                        re += space + (i == 0 ? "and (" : field) + space + ashift[i] + "=" + getValueByType();
                    }
                    re += ")";
                    return re;
                }
                else
                {
                    return space + field + space + name + "=" + getValueByType();
                }
            }
        }
}
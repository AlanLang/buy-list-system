using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sliontek_web
{
    public class HtmlHelp
    {
        /// <summary>
        /// 生成用于表单的input控件
        /// </summary>
        /// <param name="name">表单name</param>
        /// <param name="titie">显示名称</param>
        /// <param name="inpclass">样式</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static MvcHtmlString Input(string name, string titie, string value, string verify)
        {
            string html = "<div class='layui-form-item'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><input type = 'text' id = '{0}' name='{0}' lay-verify='{3}' placeholder='请输入{1}' value='{2}' autocomplete='off' class='layui-input'></div></div>";
            html = string.Format(html, name, titie, value, verify);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString InputFile(string name, string titie, string key, string value, string url)
        {
            string str = "<div class='formdiv formdivn'><form class='layui-form layui-form-pane' name='apiForm' method='post' enctype='multipart/form-data' action='" + url + "'><input id='" + key + "' type='hidden' value='" + value + "' name='" + key + "' /><div class='layui-form-item'><label class='layui-form-label'>" + titie + "</label><div class='layui-input-block'><input type='file' id='" + name + "' name='" + name + "' autocomplete='off' class='layui-input'></div></div><div style='text-align: right;margin-bottom: 0px;' class='layui-form-item'><div class='layui-input-block'><button class='layui-btn submitbtn' lay-submit lay-filter='slionform'>提交</button><button type='button' id='btnclose' class='layui-btn layui-btn-primary'>关闭</button></div></div></form></div>";
            return MvcHtmlString.Create(str);
        }

        public static MvcHtmlString InputReadOnly(string name, string titie, string value, string verify)
        {
            string html = "<div class='layui-form-item'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><input type = 'text' id = '{0}' name='{0}' lay-verify='{3}' placeholder='请输入{1}' value='{2}' autocomplete='off' class='layui-input' readonly='readonly'></div></div>";
            html = string.Format(html, name, titie, value, verify);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString Select(string name, string titie, string value, string verify)
        {
            string html = "<div class='layui-form-item'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><select value='{2}' name='{0}' id='{0}' lay-verify='{3}' lay-search></select></div></div>";
            html = string.Format(html, name, titie, value, verify);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString SelectStart(string title)
        {
            string html = "<div class='layui-form-item'><label class='layui-form-label'>{0}</label><div class='layui-input-block'></div></div>";
            html = string.Format(html, title);
            return MvcHtmlString.Create(html);
        }
        public static MvcHtmlString SelectEnd()
        {
            string html = "</div></div>";
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString Password(string name, string titie, string value, string verify)
        {
            string html = "<div class='layui-form-item password'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><input type = 'password' id = '{0}' name='{0}' lay-verify='{3}' placeholder='请输入{1}' value='{2}' autocomplete='off' class='layui-input'></div></div>";
            html = string.Format(html, name, titie, value, verify);
            return MvcHtmlString.Create(html);
        }


        public static MvcHtmlString Hidden(string name, object value)
        {
            string html = "<input id='{0}' type='hidden' value='{1}' name={0} />";
            html = string.Format(html, name, value);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString Textarea(string name, string titie, string value, string verify, string height)
        {
            string html = "<div  class='layui-form-item layui-form-text'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><textarea style='height: {4}px' id='{0}' name='{0}' placeholder='请输入{1}' value='{2}' lay-verify='{3}' class='layui-textarea'>{2}</textarea></div></div>";
            html = string.Format(html, name, titie, value, verify, height);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString FormBtn(string filter = "slionform",string resetid = "btnclose")
        {
            string html = "<div style='text-align: right;margin-bottom: 10px;' class='layui-form-item'><div class='layui-input-block'><button class='layui-btn submitbtn' lay-submit lay-filter='{0}'>提交</button><button type='reset' id='{1}' class='layui-btn layui-btn-primary'>关闭</button></div></div>";
            html = string.Format(html, filter,resetid);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString FormStart()
        {
            return MvcHtmlString.Create("<div class='formdiv formdivn'><form class='layui-form layui-form-pane' action=''>");
        }

        public static MvcHtmlString FormEnd()
        {
            return MvcHtmlString.Create("</form></div>");
        }

        public static MvcHtmlString SearchFormStart()
        {
            return MvcHtmlString.Create("<form id='form1' class='layui-form layui-form-pane search-div'><div class='layui-form-item'>");
        }

        public static MvcHtmlString SearchFormEnd()
        {
            return MvcHtmlString.Create("</div></form>");
        }

        /// <summary>
        /// 生成表格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MvcHtmlString Table(string name)
        {
            string html = "<div class='p-table'><div class='table-title'>{0} <i class='fa fa-plus' style='display:none' title='添加' id='addOne'></i> <i class='fa fa-refresh' id='iRefresh' title='刷新'></i></div><table id='sliontable' lay-filter='sliontable'></table><div style='text-align:right'><div id='paginator'></div></div></div>";
            html = string.Format(html, name);
            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// 生成表格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MvcHtmlString TableOnly(string name)
        {
            string html = "<div class='p-table'><div class='table-title'>{0} <i class='fa fa-refresh' id='iRefresh' title='刷新'></i></div><table id='sliontable'></table><div style='text-align:right'><div id='paginator'></div></div></div>";
            html = string.Format(html, name);
            return MvcHtmlString.Create(html);
        }

        /// <summary>
        /// 生成表格
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MvcHtmlString TableSync(string name)
        {
            string html = "<div class='p-table'><div class='table-title'>{0} <i class='fa fa-cloud-download' title='同步' id='Sync'></i> <i class='fa fa-refresh' id='iRefresh' title='刷新'></i></div><table id='sliontable'></table><div style='text-align:right'><div id='paginator'></div></div></div>";
            html = string.Format(html, name);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString TableWithImport(string name)
        {
            string html = "<div class='p-table'><div class='table-title'>{0} <i class='fa fa-plus' title='添加' id='addOne'></i> <i class='fa fa-upload' title='导入' id='import'></i> <i class='fa fa-refresh' id='iRefresh' title='刷新'></i></div><table id='sliontable'></table><div style='text-align:right'><div id='paginator'></div></div></div>";
            html = string.Format(html, name);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString TableWithDown(string name)
        {
            string html = "<div class='p-table'><div class='table-title'>{0} <i class='fa fa-plus' title='添加' id='addOne'></i> <i class='fa fa-download' title='导出' id='download'></i> <i class='fa fa-refresh' id='iRefresh' title='刷新'></i></div><table id='sliontable'></table><div style='text-align:right'><div id='paginator'></div></div></div>";
            html = string.Format(html, name);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString SearchInput(string title, string name)
        {
            string html = "<div class='layui-inline search-item'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><input type='text' name='{0}' id = '{0}' placeholder='请输入查询条件' autocomplete='off' class='layui-input'></div></div>";
            html = string.Format(html, name, title);
            return MvcHtmlString.Create(html);
        }


        public static MvcHtmlString SearchInput(string title, string name, string placeholder)
        {
            string html = "<div class='layui-inline search-item'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><input type='text' name='{0}' id = '{0}' placeholder='{2}'  autocomplete='off' class='layui-input'></div></div>";
            html = string.Format(html, name, title, placeholder);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString SearchInput(string title, string name, string placeholder, string val)
        {
            string html = "<div class='layui-inline search-item'><label class='layui-form-label'>{1}</label><div class='layui-input-block'><input type='text' name='{0}' id = '{0}' placeholder='{2}' value = '{3}'  autocomplete='off' class='layui-input'></div></div>";
            html = string.Format(html, name, title, placeholder, val);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString SearchInputReadOnly(string title, string val)
        {
            string html = "<div class='layui-inline search-item'><label class='layui-form-label'>{0}</label><div class='layui-input-block'><input type='text' value = '{1}' readonly='readonly'  autocomplete='off' class='layui-input'></div></div>";
            html = string.Format(html, title, val);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString SearchButton(string searchid, string resetid)
        {
            string html = "<div class='layui-inline search-item'><button style='width:64px' id='{0}' type='button' class='layui-btn {0}' <button>查询</button><input id='{1}' type='reset'  value='重置' class='layui-btn layui-btn-primary {1}' /></div>";
            html = string.Format(html, searchid, resetid);
            return MvcHtmlString.Create(html);
        }

        public static MvcHtmlString LayFormStart(string id)
        {
            return MvcHtmlString.Create("<div id="+id+ " style='display:none' class='formdiv formdivn sliontek-from'><form  class='layui-form layui-form-pane' action=''>");
        }


        public static MvcHtmlString LayFormEnd()
        {
            return MvcHtmlString.Create("</form></div>");
        }
    }
}
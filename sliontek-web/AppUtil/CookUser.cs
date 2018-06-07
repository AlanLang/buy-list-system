using sliontek_web.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.SessionState;

namespace sliontek_web
{
    public class CookUser
    {
        private string cookname = "SliontekWeb";
        private HttpSessionState Session = HttpContext.Current.Session;

        private string _usercode;
        private string _username;
        private DateTime _logintime;
        private string _ip;
        private bool _valid;
        private string _shift;

        public string usercode { get { return _usercode; } }
        public string username { get { return _username; } }
        public string ip { get { return _ip; } }
        public string shift { get { return _shift; } }
        public bool valid { get { return _valid; } }
        public DateTime logintime { get { return _logintime; } }

        public CookUser()
        {
            if (Session != null && HttpContext.Current.Session["cookuser"] != null)
            {
                _usercode = (string)HttpContext.Current.Session["cookuser"];
                initUsr(_usercode);
            }
            else
            {
                HttpCookie cook = HttpContext.Current.Request.Cookies[cookname];
                if (cook != null)
                    this.Cookies = cook;
            }
            _shift = "测试班次";
        }

        public CookUser(string userc)
        {
            initUsr(userc);
            HttpContext.Current.Session["cookuser"] = userc;
        }

        private void initUsr(string code)
        {
            using (EFContext db = new Repositories.EFContext())
            {
                var user = db.SysUser.Where(m => m.UserCode == code).FirstOrDefault();
                _usercode = user.UserCode;
                _username = user.UserName;
                _logintime = DateTime.Now;
                IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
                _ip = IpEntry.AddressList[0].ToString();
                _valid = true;
            }
        }
        public HttpCookie Cookies
        {
            get
            {
                HttpCookie adCookies = new HttpCookie(cookname);
                adCookies.Values["Name"] = HttpContext.Current.Server.UrlEncode("");
                adCookies.Values["usercode"] = usercode;
                adCookies.Values["username"] = HttpUtility.UrlEncode(username); ;
                adCookies.Values["logintime"] = DateTime.Now.ToString();
                adCookies.Values["ip"] = ip;
                return adCookies;
            }
            set
            {
                HttpCookie adCookies = value;
                if (adCookies == null || adCookies.Values["Name"] == null)
                    _valid = false;
                else
                {
                    _usercode = adCookies.Values["usercode"];
                    _username = adCookies.Values["username"];
                    _ip = adCookies.Values["ip"];
                }
            }
        }
    }

    public class cuser
    {
        private string _usercode;
        private string _username;
        public string usercode { get { return _usercode; } }
        public string username { get { return _username; } }

        public cuser()
        {
            HttpCookie cook = HttpContext.Current.Request.Cookies["SliontekWeb"];
            if (cook != null)
            {
                if (!string.IsNullOrEmpty(cook.Values["usercode"]))
                {
                    _usercode = cook.Values["usercode"];
                    _username = HttpUtility.UrlDecode(cook.Values["username"]);
                }
            }
        }
    }
}
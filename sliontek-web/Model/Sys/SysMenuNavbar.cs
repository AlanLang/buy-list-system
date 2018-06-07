using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sliontek_web.Model
{
    public class SysMenuNavbar
    {
        public class navbar
        {
            private string _title;
            private string _icon;
            private bool _spread;
            private int _pmid;

            /// <summary>
            /// id
            /// </summary>
            public int pmid
            {
                get
                {
                    return _pmid;
                }
                set
                {
                    _pmid = value;
                }
            }

            /// <summary>
            /// 标题
            /// </summary>
            public string title
            {
                get
                {
                    return _title;
                }
                set
                {
                    _title = value;
                }
            }
            /// <summary>
            /// 图标
            /// </summary>
            public string icon
            {
                get
                {
                    return "";
                }
                set
                {
                    _icon = value;
                }
            }
            /// <summary>
            /// 是否展开
            /// </summary>
            public bool spread
            {
                get
                {
                    return _spread;
                }
                set
                {
                    _spread = value;
                }
            }

            public List<children> children
            {
                get
                {
                    return _children;
                }

                set
                {
                    _children = value;
                }
            }

            private List<children> _children;
        }
        public class children
        {
            private string _title;
            private string _icon;
            private string _href;
            private int _piid;

            /// <summary>
            /// id
            /// </summary>
            public int piid
            {
                get
                {
                    return _piid;
                }
                set
                {
                    _piid = value;
                }
            }

            /// <summary>
            /// 标题
            /// </summary>
            public string title
            {
                get
                {
                    return _title;
                }
                set
                {
                    _title = value;
                }
            }
            /// <summary>
            /// 图标
            /// </summary>
            public string icon
            {
                get
                {
                    return "";
                }
                set
                {
                    _icon = value;
                }
            }
            /// <summary>
            /// 地址
            /// </summary>
            public string href
            {
                get
                {
                    return _href;
                }
                set
                {
                    _href = value;
                }
            }

        }
    }
}
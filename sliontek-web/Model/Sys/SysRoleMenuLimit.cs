using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace sliontek_web.Model
{
    public class SysRoleMenuLimit
    {
        [Key]
        public int RoleMenuLimitID { get; set; }
        public int RoleID { get; set; }
        public int MenuLimitID { get; set; }

        public DateTime RoleMenuModified { get; set; }
        public DateTime RoleMenuCreate { get; set; }

        public virtual SysRole SysRole { get; set; }
        public virtual SysMenuLimit SysMenuLimit { get; set; }
    }
}
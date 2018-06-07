using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sliontek_web.Model
{
    public class SysUserRole
    {
        [Key]
        public int UserRoleID { get; set; }

        public int UserID { get; set; }
        public int RoleID { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime UserRoleModified { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime UserRoleCreate { get; set; }

        public virtual SysUser SysUser { get; set; }
        public virtual SysRole SysRole { get; set; }
    }
}
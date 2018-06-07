using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace sliontek_web.Model
{
    public class SysRole
    {
        [Key]
        public int RoleID { get; set; }
        [MaxLength(50)]
        public string RoleCode { get; set; }
        [MaxLength(50)]
        public string RoleName { get; set; }
        [MaxLength(200)]
        public string RoleDesc { get; set; }

        public DateTime RoleModified { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime RoleCreate { get; set; }

        [ScriptIgnore]
        public virtual ICollection<SysUserRole> SysUserRole { get; set; }

        [ScriptIgnore]
        public virtual ICollection<SysRoleMenuLimit> SysRoleMenuLimit { get; set; }
    }
}
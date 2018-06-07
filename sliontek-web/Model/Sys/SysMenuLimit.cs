using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace sliontek_web.Model
{
    public class SysMenuLimit
    {
        [Key]
        public int MenuLimitID { get; set; }

        public int MenuID { get; set; }

        [MaxLength(20)]
        public string MenuLimitCode { get; set; }
        [MaxLength(20)]
        public string MenuLimitName { get; set;}

        public int MenuLimitSort { get; set; }

        [JsonIgnore]
        public virtual SysMenu SysMenu {get;set;}

        [JsonIgnore]
        public virtual ICollection<SysRoleMenuLimit> SysRoleMenuLimit { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime MenuLimitModified { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime MenuLimitCreate { get; set; }
    }
}
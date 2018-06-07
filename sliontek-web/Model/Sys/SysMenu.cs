using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace sliontek_web.Model
{
    public class SysMenu
    {
        [Key]
        public int MenuID { get; set; }

        [MaxLength(20)]
        public string MenuName { get; set; }

        [MaxLength(50)]
        public string MenuDesc { get; set; }

        public int MenuFa { get; set; }

        public int MenuSort { get; set; }

        [MaxLength(50)]
        public string MenuUrl { get; set; }

        public virtual ICollection<SysMenuLimit> SysMenuLimit { get; set; }

        [ScriptIgnore]
        public DateTime MenuModified { get; set; }
        [ScriptIgnore]
        public DateTime MenuCreate { get; set; }
    }
}
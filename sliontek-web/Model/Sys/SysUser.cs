using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sliontek_web.Model
{
    public class SysUser
    {
        [Key]
        public int UserID { get; set; }
        [MaxLength(50)]
        public string UserCode { get; set; }
        [MaxLength(50)]
        public string UserName { get; set; }
        [MaxLength(35)]
        public string UserPwd { get; set; }
        [MaxLength(35)]
        public string UserMail { get; set; }
        [MaxLength(35)]
        public string UserWx { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime UserModified { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime UserCreate { get; set; }

        public virtual ICollection<SysUserRole> SysUserRole { get; set; }
    }
}
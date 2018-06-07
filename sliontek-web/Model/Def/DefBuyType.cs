using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sliontek_web.Model.Def
{
    public class DefBuyType
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(30)]
        public string TypeName { get; set; }

        [MaxLength(50)]
        public string TypeDesc { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Modified { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Create { get; set; }
    }
}
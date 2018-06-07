using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace sliontek_web.Model.Buy
{
    public class BuyNewChangeLog
    {
        [Key]
        public int ID { get; set; }
        public int BuyNewID { get; set; }

        [MaxLength(50)]
        public string LogMsg { get; set; }
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Create { get; set; }
    }
}
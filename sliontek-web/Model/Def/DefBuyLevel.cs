using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sliontek_web.Model.Def
{
    public class DefBuyLevel
    {
        public string BuyLevel { get; set; }

        public List<DefBuyLevel> GetBuyLevels()
        {
            List<DefBuyLevel> list = new List<Def.DefBuyLevel>();
            list.Add(new DefBuyLevel() { BuyLevel = "普通" });
            list.Add(new DefBuyLevel() { BuyLevel = "重要" });
            list.Add(new DefBuyLevel() { BuyLevel = "非常重要"});
            list.Add(new DefBuyLevel() { BuyLevel = "可买可不买"});
            return list;
        }
    }
}
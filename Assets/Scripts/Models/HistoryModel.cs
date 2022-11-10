using StormWarfare.Model;
using System.Collections.Generic;

namespace StormWarfare.Models 
{
    public class HistoryModel
    {
        //historyye eklenmis etkidigi kart olabilir veya sadece deploy edilmis olabilcek olan kart
        public BaseCardModel sourceCard;

        //sourcecard ile iliskisi olan kart/kartlar deneme amacli tekli sonradan liste cevrilcek
        //sourcecard birden fazla karta etkimis olabilir
        public List<EffectStruct> effectedCards;

        //etki edilen kartlardaki degisiklik durumlari bu da model yapilabilir su anlik dynamic olarak
        //tutuldu icinde sadece attackValue degeri var.
        public Dynamic deltaValues;

    }
    public struct EffectStruct{
        public BaseCardModel card;
        public BaseSpecialEffect effect;
    }
}
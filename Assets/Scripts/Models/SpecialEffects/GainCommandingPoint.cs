using StormWarfare.Gameplay;
using StormWarfare.Model;
using System.Collections.Generic;

namespace StormWarfare.Models
{
    public class GainCommandingPoint : BaseSpecialEffect
    {
        public int Count;

        public void UseEffect(ref CommanderModel myCommander)
        {
            myCommander.CommandingPoint += Count;
        }


        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            Count = json.ToInt("count");
        }
    }
}
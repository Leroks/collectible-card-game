using StormWarfare.Model;
using System.Collections.Generic;

namespace StormWarfare.Models
{
    public class EventCardModel : BaseCardModel
    {
        /// <summary>
        /// Commanding Point's Value(Card Activation Cost)
        /// </summary>
        public int CommandingPoint;
        /// <summary>
        /// List of Card's Abilities can be null if card doesn't contains any
        /// </summary>
        public List<BaseSpecialEffect> SpecialEffects;

        public List<EffectStruct> EffectedCards = new();
        public bool DidIUsedBefore;
        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            CommandingPoint = json.ToInt("commandingPoint");
            SpecialEffects = DeserializeSpecialEffects(json.ToObject("specialEffects"));
        }
    }
}
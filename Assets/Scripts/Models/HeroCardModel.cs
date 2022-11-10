using StormWarfare.Model;
using System.Collections.Generic;

namespace StormWarfare.Models
{
    public class HeroCardModel : BaseCardModel
    {
        /// <summary>
        /// Attack Point's Value
        /// </summary>
        public int AttackPoint;
        /// <summary>
        /// Health Point's Value
        /// </summary>
        public int HealthPoint;
        /// <summary>
        /// Commanding Point's Value(Card Activation Cost)
        /// </summary>
        public int CommandingPoint;
        /// <summary>
        /// List of Card's Abilities can be null if card doesn't contains any
        /// </summary>
        public List<BaseSpecialEffect> SpecialEffects;

        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            AttackPoint = json.ToInt("attackPoint");
            HealthPoint = json.ToInt("healthPoint");
            CommandingPoint = json.ToInt("commandingPoint");
            SpecialEffects = DeserializeSpecialEffects(json.ToObject("specialEffects"));
        }
    }
}
using StormWarfare.Model;
using System.Collections.Generic;

namespace StormWarfare.Models
{
    public class WeaponCardModel : BaseCardModel
    {
        /// <summary>
        /// Attack Point's Value
        /// </summary>
        public int AttackPoint;
        /// <summary>
        /// Usage Count for the weapon
        /// </summary>
        public int AmmunationPoint;
        /// <summary>
        /// Commanding Point's Value(Card Activation Cost)
        /// </summary>
        public int CommandingPoint;
        /// <summary>
        /// List of Card's Abilities can be null if card doesn't contains any
        /// </summary>
        public List<Enums.Abilities> Abilities;
        /// <summary>
        /// List of Card's Special Effect can be null if card doesn't contains any
        /// </summary>
        public List<BaseSpecialEffect> SpecialEffects;
        public bool DidIUsedBefore;

        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            AttackPoint = json.ToInt("attackPoint");
            AmmunationPoint = json.ToInt("ammunationPoint");
            CommandingPoint = json.ToInt("commandingPoint");
            Abilities =((Dynamic) json).ToIntList("abilities").ConvertAll(a => (Enums.Abilities) a);
            SpecialEffects = DeserializeSpecialEffects(json.ToObject("specialEffects"));
        }
    }
}
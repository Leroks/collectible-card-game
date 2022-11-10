using StormWarfare.Core;
using StormWarfare.Model;
using System.Collections.Generic;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Models
{
    public class CommanderModel : BaseCardModel
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
        /// Commanding Point's Value
        /// </summary>
        public int CommandingPoint;
        /// <summary>
        /// Commanders Faction
        /// </summary>
        public Faction CommanderFaction;
        /// <summary>
        /// List of Card's Abilities can be null if card doesn't contains any
        /// </summary>
        public List<BaseSpecialEffect> CommanderAbility;
        /// <summary>
        /// List of Card's Anti Class Type can be null if card doesn't contains any
        /// </summary>
        public List<int> AntiClassType;
        /// <summary>
        /// List of Card's Resistance to the Class Type can be null if card doesn't contains any
        /// </summary>
        public List<int> Resistance;
        public WeaponCardModel PlayerWeapon;
        public CommanderAbilityCardModel CommanderSpecialEffectCard;
        public int OriginalHealthPoint;
        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            AttackPoint = json.ToInt("attackPoint");
            OriginalHealthPoint = HealthPoint = json.ToInt("healthPoint");
            CommandingPoint = json.ToInt("commandingPoint");
            CommanderFaction = (Faction)(json.ToInt("faction"));
            PlayerWeapon = Deserialize<WeaponCardModel>(json.ToObject("weapon"));
            CommanderAbility = DeserializeSpecialEffects(json.ToObject("specialEffects")); 
            CommanderSpecialEffectCard = Deserialize<CommanderAbilityCardModel>(json.ToObject("abilitycard"));
        }
    }
}

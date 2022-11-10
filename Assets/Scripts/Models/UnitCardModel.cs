using System.Collections;
using System.Collections.Generic;
using StormWarfare.Interface;
using StormWarfare.Model;
using UnityEngine;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Models
{
    public class UnitCardModel : BaseCardModel
    { 
        /// <summary>
        /// Attack Point's Value
        /// </summary>
        int _AttackPoint;
        public int AttackBuffPoint;
        public int AttackPoint
        {
            get => AttackBuffPoint + _AttackPoint;
            set => _AttackPoint = value - AttackBuffPoint;
        }

        /// <summary>
        /// Health Point's Value
        /// </summary>
        int _HealthPoint;
        public int HealthBuffPoint;
        public int HealthPoint
        {
            get => HealthBuffPoint + _HealthPoint;
            set
            {
                _HealthPoint = value - HealthBuffPoint;
                CardState &= _HealthPoint > OriginalHealthPoint ? ~CardState.HealthNerfed : ~CardState.HealthBuffed;
                CardState |= _HealthPoint > OriginalHealthPoint ? CardState.HealthBuffed : CardState.HealthNerfed;
            }
        }
        public int OriginalHealthPoint;

        public bool DidIAttackBefore;
        /// <summary>
        /// Health Point's Value
        /// </summary>
        public int DeploymentRound;
        public bool CanIPlay
        {
            get => BoardModel.TotalRoundCount > DeploymentRound && !DidIAttackBefore;
            set => CanIPlay = value;
        }
        /// <summary>
        /// Commanding Point's Value(Card Activation Cost)
        /// </summary>
        public int CommandingPoint;
        /// <summary>
        /// Card's Card Class can casted from Enum.cs(ClassType)
        /// </summary>
        public Enums.ClassType ClassType;
        /// <summary>
        /// TODO: Card's Card Class can casted from Enum.cs(ClassType)
        /// </summary>
        public Enums.SubClass SubClassType;
        /// <summary>
        /// List of Card's Abilities can be null if card doesn't contains any
        /// </summary>
        public List<Enums.Abilities> Abilities;
        /// <summary>
        /// List of Card's Special Effect can be null if card doesn't contains any
        /// </summary>
        public List<BaseSpecialEffect> SpecialEffects;
        /// <summary>
        /// List of Card's Anti Class Type can be null if card doesn't contains any
        /// </summary>
        public List<int> AntiClassTypes;
        /// <summary>
        /// List of Card's Resistance to the Class Type can be null if card doesn't contains any
        /// </summary>
        public List<int> Resistance;

        public CardState CardState;

        public bool IsHeroCard;

        public List<EffectStruct> EffectedCards = new();

        public List<BaseSpecialEffect> CardBuffs = new List<BaseSpecialEffect>();

        public void ApplyBuff(BaseSpecialEffect effectData)
        {
            switch (effectData)
            {
                case GiveAttackPoint e:
                    CardState |= e.Count > 0 ? CardState.AttackBuffed : CardState.AttackNerfed;
                    CardState &= e.Count > 0 ? ~CardState.AttackNerfed : ~CardState.AttackBuffed;
                    AttackBuffPoint += e.Count;
                    break;
                case GiveDefensePoint e:
                    CardState |= e.Count > 0 ? CardState.HealthBuffed : CardState.HealthNerfed;
                    CardState &= e.Count > 0 ? ~CardState.HealthNerfed : ~CardState.HealthBuffed;
                    HealthBuffPoint += e.Count;
                    break;
                default:
                    break;
            }
        }

        public void ApplyDebuff(BaseSpecialEffect effectData)
        {
            switch (effectData)
            {
                case DealDamage e:
                    CardState |= e.Count > 0 ? CardState.HealthBuffed : CardState.HealthNerfed;
                    CardState &= e.Count > 0 ? ~CardState.HealthNerfed : ~CardState.HealthBuffed;
                    HealthPoint -= e.Count;
                    break;
                default:
                    break;
            }
        }

        public void RemoveBuffDebuff(BaseSpecialEffect effectData)
        {
            /*for (int i = 0; i < effectData.Count; i++)
            {
                var d = effectData[i];*/
                switch (effectData)
                {
                    case GiveAttackPoint e:
                        CardState |= e.Count > 0 ? CardState.AttackNerfed : CardState.AttackBuffed;
                        CardState &= e.Count > 0 ? ~CardState.AttackBuffed : ~CardState.AttackNerfed;
                        AttackBuffPoint -= e.Count;
                        break;
                    case GiveDefensePoint e:
                        CardState |= e.Count > 0 ? CardState.HealthNerfed : CardState.HealthBuffed;
                        CardState &= e.Count > 0 ? ~CardState.HealthBuffed : ~CardState.HealthNerfed;
                        HealthBuffPoint -= e.Count;
                        break;
                    default:
                        break;
                }
            /*}*/
        }

        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            AttackPoint = json.ToInt("attackPoint");
            HealthPoint = json.ToInt("healthPoint");
            OriginalHealthPoint = json.ToInt("healthPoint");
            ClassType = (Enums.ClassType) json.ToInt("classType");
            SubClassType = (Enums.SubClass)(json.ToInt("classType") * 10 + json.ToInt("subClassType"));
            CommandingPoint = json.ToInt("commandingPoint");
            Abilities =((Dynamic) json).ToIntList("abilities").ConvertAll(a => (Enums.Abilities) a);
            SpecialEffects = DeserializeSpecialEffects(json.ToObject("specialEffects"));
            AntiClassTypes = ((Dynamic) json).ToIntList("antiClassTypes");
            Resistance = ((Dynamic) json).ToIntList("resistances");
            CardState = (CardState) json.ToInt("cardstate");
            IsHeroCard = json.ToBool("ishero");
        }
    }
}
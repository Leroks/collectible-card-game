using StormWarfare.Core;
using StormWarfare.Model;
using System.Collections;
using System.Collections.Generic;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Models
{
    public class BaseCardModel : JsonModel
    {
        /// <summary>
        /// Unique id for the card
        /// </summary>
        public int Id;
        /// <summary>
        /// Level number of the card
        /// </summary>
        public int Level;
        /// <summary>
        /// Faction Id for the card, can casted from Enum.cs(Faction)
        /// </summary>
        public Enums.Faction Faction;
        /// <summary>
        /// Rarity of the card
        /// </summary>
        public int Rarity;
        /// <summary>
        /// Name of the card
        /// </summary>
        public string Name;
        /// <summary>
        /// Texture path/remote url for the card
        /// </summary>
        public string Texture;
        public string Sound;

        /// <summary>
        /// Card description
        /// </summary>
        public string Description;

        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            Id = json.ToInt("id");
            Level = json.ToInt("level");
            Faction = (Enums.Faction) json.ToInt("faction");
            Rarity = json.ToInt("rarity");
            Name = json.ToString("name") ?? "";
            Texture = Faction.ToString() + "/" + json.ToString("texture");
            Description = json.ToString("description") ?? "";
            Sound = json.ToString("texture");
        }

        protected List<BaseSpecialEffect> DeserializeSpecialEffects(object specialEffectData)
        {
            List<BaseSpecialEffect> result = new List<BaseSpecialEffect>();
            if (specialEffectData == null) return result; 
            for (int i = 0; i < ((IList) specialEffectData).Count; i++)
            {
                var item = ((IList)specialEffectData)[i];
                switch ((SpecialEffectTypes)item.ToInt("type"))
                {
                    case SpecialEffectTypes.DealDamage:
                        result.Add(Deserialize<DealDamage>(item));
                        break;
                    case SpecialEffectTypes.GiveDefensePoint:
                        result.Add(Deserialize<GiveDefensePoint>(item));
                        break;
                    case SpecialEffectTypes.GiveAttackPoint:
                        result.Add(Deserialize<GiveAttackPoint>(item));
                        break;
                    case SpecialEffectTypes.GainCommandingPoint:
                        result.Add(Deserialize<GainCommandingPoint>(item));
                        break;
                    case SpecialEffectTypes.DrawCard:
                        result.Add(Deserialize<DrawCard>(item));
                        break;
                    case SpecialEffectTypes.Retreat:
                        result.Add(Deserialize<Retreat>(item));
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

    }
}

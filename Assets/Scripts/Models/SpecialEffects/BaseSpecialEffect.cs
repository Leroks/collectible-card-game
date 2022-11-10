using StormWarfare.Core;
using StormWarfare.Models;
using System;
using System.Collections.Generic;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Model
{
    public class BaseSpecialEffect : JsonModel
    {
        public SpecialEffectTypes Type;

        public List<SpecialEffectConditionModel> Conditions;

        public override void Deserialize(object json)
        {
            Type = (SpecialEffectTypes)json.ToInt("type");
            Conditions = new List<SpecialEffectConditionModel>();
        }
    }

    /// <summary>
    /// todo: conditionlari olabilir su anki amerikan destesinde yok almanda denk gelinirse eklenicek
    /// </summary>
    public class SpecialEffectConditionModel : JsonModel 
    {
        public SpecialEffectCondition Type;

        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            Type = (SpecialEffectCondition) json.ToInt("type");
        }
    }
}
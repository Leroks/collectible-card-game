using StormWarfare.Model;
using System;
using System.Collections.Generic;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Models
{
    public class Retreat : BaseSpecialEffect
    {
        public SpecialEffectTarget Target;

        public List<ClassType> TargetClassType;

        public int Count;

        public List<int> UseEffect(ref BoardModel model)
        {
            List<int> result = new List<int>();
            var targetCards = model.boardCards[model.OtherTurn];
            var targetHand = model.IsMyTurn ? model.OppHand : model.MyHand;
            /*
            var effectedCards = new List<BaseCardModel>(targetCards);
            for (int i = 0; i < TargetClassType.Count; i++)
            {
                var targetClassType = TargetClassType[i];
                effectedCards.RemoveAll(t => (t as UnitCardModel).ClassType != targetClassType);
            }
            */
            switch (Target)
            {
                case SpecialEffectTarget.Unit:
                    var unit = targetCards[^1];
                    for (int i = 0; i < (unit as UnitCardModel).EffectedCards.Count; i++)
                    {
                        var effectedCard = (unit as UnitCardModel).EffectedCards[i];
                        (effectedCard.card as UnitCardModel).RemoveBuffDebuff(effectedCard.effect);
                    }
                    (unit as UnitCardModel).CardState = 0;
                    targetHand.Insert(0, unit);
                    targetCards.RemoveAt(targetCards.Count - 1);
                    result.Add(targetCards.Count - 1);
                    break;
                case SpecialEffectTarget.RandomUnit:
                    var rand = new Random();
                    var maxCount = Math.Min(Count, targetCards.Count);
                    while (result.Count < maxCount)
                    {
                        var randIndex = rand.Int(0, targetCards.Count - 1);
                        if (result.Contains(randIndex)) continue;
                        var choosenUnit = targetCards[randIndex];
                        for (int i = 0; i < (choosenUnit as UnitCardModel).EffectedCards.Count; i++)
                        {
                            var effectedCard = (choosenUnit as UnitCardModel).EffectedCards[i];
                            (effectedCard.card as UnitCardModel).RemoveBuffDebuff(effectedCard.effect);
                        }
                        (choosenUnit as UnitCardModel).CardState = 0;
                        targetHand.Add(choosenUnit);
                        result.Add(randIndex);
                    }
                    result.Sort();
                    for (int i = result.Count; i --> 0;)
                    {
                        targetCards.RemoveAt(result[i]);
                    }

                    break;
                case SpecialEffectTarget.Board:
                    break;
                default:
                    break;
            }

            return result;
        }


        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            Target = (SpecialEffectTarget)json.ToInt("target");
            TargetClassType = ((Dynamic)json).ToIntList("targetClasses").ConvertAll(a => (ClassType)a);
            Count = json.ToInt("count");
        }
    }
}
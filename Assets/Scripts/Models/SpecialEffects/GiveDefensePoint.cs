using StormWarfare.Model;
using System.Collections.Generic;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Models
{
    public class GiveDefensePoint : BaseSpecialEffect
    {
        public SpecialEffectTarget Target;
        public List<ClassType> TargetClassType; 
        public int Count;

        public void UseEffect(ref List<BaseCardModel> targetCards, ref CommanderModel commander, BaseCardModel sourceCard, int targetIndex = 0)
        {
            /*
            var effectedCards = new List<BaseCardModel>(targetCards);
            for (int i = 0; i < TargetClassType.Count; i++)
            {
                var targetClassType = TargetClassType[i];
                effectedCards.RemoveAll(t => (t as UnitCardModel).ClassType != targetClassType);
            }
            */

            var str = new EffectStruct() { effect = this };
            switch (Target)
            {
                case SpecialEffectTarget.Unit:
                    //bir tane method calistir target olarak index donsun targetCards icerisinde o indexteki kartin verilerini guncelle
                    //su anlik en sondaki indexi hedef aliyor degistir
                    var unit = targetCards[targetIndex] as UnitCardModel;
                    //unit.CardState |= CardState.HealthBuffed;
                    //unit.HealthPoint += Count;
                    str.card = unit;
                    (sourceCard as UnitCardModel)?.EffectedCards.Add(str);
                    (sourceCard as EventCardModel)?.EffectedCards.Add(str);
                    unit.ApplyBuff(this);
                    break;
                case SpecialEffectTarget.RandomUnit:
                    //targetCards icerisinden random bir kart sec
                    var rand = new Random();
                    if (targetCards.Count == 0) return;
                    var randUnit = targetCards[rand.Int(0, targetCards.Count - 1)] as UnitCardModel;
                    //randUnit.CardState |= CardState.HealthBuffed;
                    //randUnit.HealthPoint += Count;
                    str.card = randUnit;
                    (sourceCard as UnitCardModel)?.EffectedCards.Add(str);
                    (sourceCard as EventCardModel)?.EffectedCards.Add(str);
                    randUnit.ApplyBuff(this);
                    break;
                case SpecialEffectTarget.Board:
                    //butun targetcards i guncelle
                    for (int i = 0; i < targetCards.Count; i++)
                    {
                        var c = targetCards[i];
                        (c as UnitCardModel).ApplyBuff(this);
                        str.card = c;
                        (sourceCard as UnitCardModel)?.EffectedCards.Add(str);
                        (sourceCard as EventCardModel)?.EffectedCards.Add(str);
                    }
                    //targetCards.ForEach(c => { (c as UnitCardModel).ApplyBuffDebuff(this);});
                    break;
                case SpecialEffectTarget.MyCommander:
                    //todo: karsi commanderin degerlerini guncelle
                    commander.HealthPoint += Count;
                    break;
                default:
                    break;
            }
        }

        public override void Deserialize(object json)
        {
            base.Deserialize(json);
            Target = (SpecialEffectTarget)json.ToInt("target");
            TargetClassType = ((Dynamic) json).ToIntList("targetClasses").ConvertAll(a => (ClassType) a);
            Count = json.ToInt("count");
        }
    }
}
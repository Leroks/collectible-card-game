using StormWarfare.Model;
using System.Collections.Generic;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Models
{
    public class DealDamage : BaseSpecialEffect
    {
        public SpecialEffectTarget Target;

        public List<ClassType> TargetClassType; 

        public int Count;

        //todo: formulden gecirir hale getir sonradan asagidaki checki kaldir
        public SpecialEffectCountFormula CountFormula;

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
                    str.card = unit;
                    (sourceCard as UnitCardModel)?.EffectedCards.Add(str);
                    (sourceCard as EventCardModel)?.EffectedCards.Add(str);
                    unit.HealthPoint -= Count;
                    if (unit.HealthPoint <= 0) targetCards.RemoveAt(targetIndex);
                    break;
                case SpecialEffectTarget.RandomUnit:
                    if(CountFormula == SpecialEffectCountFormula.NumberOfUnits)
                    {
                        Count *= targetCards.Count;
                    }
                    //targetCards icerisinden random bir kart sec
                    var rand = new Random();
                    if (targetCards.Count == 0) return;
                    var randIndex = rand.Int(0, targetCards.Count - 1);
                    var randUnit = targetCards[randIndex] as UnitCardModel;
                    
                    str.card = randUnit;
                    (sourceCard as UnitCardModel)?.EffectedCards.Add(str);
                    (sourceCard as EventCardModel)?.EffectedCards.Add(str);

                    randUnit.HealthPoint -= Count;
                    if(randUnit.HealthPoint <= 0) targetCards.RemoveAt(randIndex);
                    break;
                case SpecialEffectTarget.Board:
                    if(CountFormula == SpecialEffectCountFormula.NumberOfUnits)
                    {
                        Count *= targetCards.Count;
                    }
                    //butun targetcards i guncelle
                    for (int i = 0; i < targetCards.Count; i++)
                    {
                        var c = targetCards[i];
                        (c as UnitCardModel).HealthPoint -= Count;
                        if((c as UnitCardModel).HealthPoint <= 0)
                        {
                            targetCards.RemoveAt(i);
                            i--;
                        }
                        str.card = c;
                        (sourceCard as UnitCardModel)?.EffectedCards.Add(str);
                        (sourceCard as EventCardModel)?.EffectedCards.Add(str);
                    }
                    break;
                case SpecialEffectTarget.OppCommander:
                    //todo: karsi commanderin degerlerini guncelle
                    commander.HealthPoint -= Count;
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
            CountFormula = (SpecialEffectCountFormula) json.ToInt("countFormula");
        }
    }
}
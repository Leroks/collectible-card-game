using StormWarfare.Card;
using StormWarfare.Core;
using UnityEngine;
using Enums = StormWarfare.Models.Enums;
using System.Collections;
using System.Collections.Generic;
using StormWarfare.Models;
using StormWarfare.Gameplay;
using StormWarfare.Model;

namespace StormWarfare.Card
{
    public class CardContainer : MonoBehaviour
    {
        [SerializeField]
        float _horizontalGap;
        [SerializeField]
        float _cardWidth;

        public List<Card> Cards = new List<Card>();
        public Vector3 CardContainerPos;

        int _lastIndex = -1;
        public bool IsOpponent;

        void Start()
        {
            GetChilds();
            if (Cards.Count < 1)
                return;
            SetChildPositions(-1);
        }
        public void GetChilds()
        {
            Cards.Clear();
            var count = this.transform.childCount;
            for (int i = 0; i < count; i++)
                Cards.Add(this.transform.GetChild(i).GetComponent<Card>());
        }

        public void RemoveChild(Card card)
        {
            Cards.Remove(card);
        }
        public void SetChildPositions(int index)
        {
            var count = Cards.Count;
            var totalWidth = count * _cardWidth + (count - 1) * _horizontalGap;
            for (int i = 0; i < count; i++)
            {
                if (i == index) continue;
                var tempPos = CardContainerPos;
                tempPos.x = 0;
                tempPos.x = (_cardWidth - totalWidth) * .5f + i * (_horizontalGap + _cardWidth) + CardContainerPos.x;
                Cards[i].MoveCard(tempPos, .2f);
                Cards[i].Index = i;
            }
        }

        public Vector3 GetChildPosition(int index)
        {
            var count = Cards.Count;
            var totalWidth = count * _cardWidth + (count - 1) * _horizontalGap;
            var tempPos = Cards[index].transform.position;
            tempPos.x = 0;
            tempPos.y = .5f;
            tempPos.x = (_cardWidth - totalWidth) * .5f + index * (_horizontalGap + _cardWidth);
            return tempPos;
        }

        public void SimulateCardHover(Card card)
        {
            var count = Cards.Count;
            if (count < 1 || count > 6) return;
            var totalWidth = count * _cardWidth + (count - 1) * _horizontalGap;
            var index = GetCardIndexFromCardPosX((card.transform.position.x + totalWidth * .5f).ToInt());
            if (_lastIndex == index) return;
            Cards.Insert(index, card);
            _lastIndex = index;
            SetChildPositions(index);
            Cards.RemoveAt(index);
        }

        public int SimulateCardAttack(float posX)
        {
            var count = Cards.Count;
            if (count < 1) return -1;
            var totalWidth = count * _cardWidth + (count - 1) * _horizontalGap;
            return GetCardIndexFromCardPosX((posX + totalWidth * .5f).ToInt());
        }
        public void EndSimulateCardHover()
        {
            _lastIndex = -1;
            SetChildPositions(-1);
        }
        public int GetCardIndexFromCardPosX(int cardPosX)
        {
            var count = Cards.Count;
            if (count < 1) return 0;
            var totalWidth = (count * _cardWidth + (count - 1) * _horizontalGap).ToInt();
            return System.Math.Clamp((cardPosX + (totalWidth / count)) / (totalWidth / count), 0, count);
        }

        //todo: ya boardcontroller icerisindeki placecardtobattleground methodunu cagir ya da bunu buradan tetikleme
        public void PlaceNewCardToBattleGround(Card card, UnitCardModel model, bool opponent = false)
        {
            if(model == null)
            {
                SetChildPositions(-1);
                return;
            }
            var count = Cards.Count;
            var unitCard = Instantiate(Resources.Load("Prefabs/2D/UnitCardBoard", typeof(UnitCardBoard)) as UnitCardBoard, card.transform.position, Quaternion.identity, transform);
            unitCard.InitCard(model);
            unitCard.FirstTime = true;
            Destroy(card.gameObject);
            int index = opponent ? 0 : GetCardIndexFromCardPosX((card.transform.position.x + (count * _cardWidth + (count - 1) * _horizontalGap).ToInt() * .5f).ToInt());
            Cards.Insert(index, unitCard);
            GameManager.Instance.BoardController.PlaceCardToBattleGround(model, index);
            SetChildPositions(-1);
        }

        public void PlaceNewCard()
        {
            GetChilds();
            if (Cards.Count < 1)
                return;
            SetChildPositions(-1);
        }


        /// <summary>
        /// todo: buradan asagi kalan kismi duzenle begenmedim kendim yazip cognitive complexity cok fazla
        /// </summary>
        public void ClearKilledChildren()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                var c = Cards[i] as UnitCardBoard;
                if (c.IsKilled)
                {
                    /*                    for (int j = 0; j < c.Model.SpecialEffects.Count; j++)
                                        {
                                            ClearSpecialEffectBuffs(i, c.Model);
                                        }*/
                    RemoveChild(c);
                }
            }
/*
            for (int i = 0; i < Cards.Count; i++)
            {
                var c = Cards[i] as UnitCardBoard;
                c.UpdateCardData(c.Model);
            }*/
        }

        /// <summary>
        /// todo: special effect targeti board ise sadece uygulasin random veya secmeli ise uygulamasin
        /// daha guzel sekilde coz 
        /// </summary>
        /// <param name="c"></param>
        public void ApplySpecialEffectBuffs(BaseCardModel c)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                var se = (Cards[i] as UnitCardBoard).Model.SpecialEffects;
                for (int j = 0; j < se.Count ; j++)
                {
                    var e = se[j];
                    var eligible = false;
                    switch (e)
                    {
                        case GiveAttackPoint g:
                            eligible = g.Target == Enums.SpecialEffectTarget.Board;
                            break;
                        case GiveDefensePoint g:
                            eligible = g.Target == Enums.SpecialEffectTarget.Board;
                            break;
                        default:
                            break;
                    }
                    if(eligible)
                    {
                        (c as UnitCardModel).ApplyBuff(e);
                        (Cards[i] as UnitCardBoard).Model.EffectedCards.Add(new EffectStruct() { card = c, effect = e });
                    }
                }
            } 
        }

        /*void ClearSpecialEffectBuffs(int exclude, BaseCardModel killedCard)
        {
            var effectedCards = (killedCard as UnitCardModel).EffectedCards;
            for (int i = 0; i < effectedCards.Count; i++)
            {
                for (int j = 0; j < (killedCard as UnitCardModel).SpecialEffects.Count; j++)
                {
                    (effectedCards[i] as UnitCardModel).RemoveBuffDebuff((killedCard as UnitCardModel).SpecialEffects[j]);
                }
            }
        }*/

        public void UpdateChildDatas()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                var c = Cards[i] as UnitCardBoard;
                c.UpdateCardData(c.Model);
            }

            ClearKilledChildren();
        }
    }
}

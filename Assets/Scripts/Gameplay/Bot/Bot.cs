using StormWarfare.Card;
using StormWarfare.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static StormWarfare.Models.Enums;

namespace StormWarfare.Gameplay.Bot
{
    public struct Move
    {
        public double score;
        public List<Action> actions;
    }


    public struct State
    {
        public GameManager gm;
        public List<Move> move;
        public int ptr;
    }

    public class Bot
    {
        static GameManager _gm;
        BoardController _board;

        //todo: optimize if needed
        private const int chunkSize = 7;
        private const int maxActionCount = 3000;
        Move[] moves = new Move[maxActionCount];

        public Bot()
        {
            _gm = GameManager.Instance;
            _board = GameManager.Instance.BoardController;
        }

        private struct CardScore
        {
            public BaseCardModel card;
            public double score;
            public int index;
        }

        double hpk = 0.25d;
        double apk = 0.25d;
        double cpk = -0.4d;
        double sek = 0.1d;

        double DealDamage = 15;
        double GiveDefensePoint = 20;
        double GiveAttackPoint = 20;
        double GainCommandingPoint = 10;
        double DrawCard = 10;
        double Retreat = 50;

        
        private List<CardScore> GetCardScores(bool checkHand = true)
        {
            var result = new List<CardScore>();
            var cards = checkHand ? _gm.BoardController.BoardModel.OppHand : _gm.BoardController.BoardModel.boardCards[1];

            for (int i = 0; i < cards.Count; i++)
            {
                var res = CalculateToPlayCardScore(cards[i]);
                res.index = i;
                result.Add(res);
            }

            return result;
        }

        private CardScore CalculateToPlayCardScore(BaseCardModel card, int toCheckPlayerPlayground = 1, bool checkSEScore = true)
        {
            double score = 0;
            double hpscore = 0;
            double apscore = 0;
            double sescore = 0;
            double cpscore = 0;
            if(card is UnitCardModel u)
            {
                if (_gm.BoardController.BoardModel.boardCards[toCheckPlayerPlayground].Count > 0 && checkSEScore)
                    for (int i = 0; i < u.SpecialEffects.Count; i++)
                    {
                        var e = u.SpecialEffects[i];
                        switch (e)
                        {
                            case DealDamage h:
                                sescore += h.Count * DealDamage;
                                break;
                            case Retreat h:
                                sescore += h.Count * Retreat;
                                break;
                            case GainCommandingPoint h:
                                sescore += h.Count * GainCommandingPoint;
                                break;
                            case GiveAttackPoint h:
                                sescore += h.Count * GiveAttackPoint;
                                break;
                            case GiveDefensePoint h:
                                sescore += h.Count * GiveDefensePoint;
                                break;
                            default:
                                break;
                        }
                    }

                hpscore += u.HealthPoint;
                apscore += u.AttackPoint;
                cpscore += u.CommandingPoint;
            }
            else if(card is EventCardModel e)
            {
                if (_gm.BoardController.BoardModel.boardCards[toCheckPlayerPlayground].Count > 0 && checkSEScore)
                    for (int i = 0; i < e.SpecialEffects.Count; i++)
                    {
                        var j = e.SpecialEffects[i];
                        switch (j)
                        {
                            case DealDamage h:
                                sescore += h.Count * DealDamage;
                                break;
                            case Retreat h:
                                sescore += h.Count * Retreat;
                                break;
                            case GainCommandingPoint h:
                                sescore += h.Count * GainCommandingPoint;
                                break;
                            case GiveAttackPoint h:
                                sescore += h.Count * GiveAttackPoint;
                                break;
                            case GiveDefensePoint h:
                                sescore += h.Count * GiveDefensePoint;
                                break;
                            default:
                                break;
                        }
                    }
                cpscore += e.CommandingPoint;
            }

            return new CardScore { card = card, score = hpscore * hpk + apscore * apk + cpscore * cpk + sescore * sek };
        }

        public Move CheckValidMoves()
        {
            var result = new Move() { actions = new List<Action>() };
            int commandingPoint = _gm.Player1.Model.CommandingPoint;

            if (commandingPoint <= 0) return result;

            var cardScores = GetCardScores();
            cardScores = cardScores.OrderByDescending(c => c.score).ToList();
            for (int i = 0; i < cardScores.Count; i++)
            {
                var cardScore = cardScores[i];   
                if(cardScore.card is UnitCardModel u && _gm.Player1.CardContainer.Cards.Count < 7)
                {
                    if(commandingPoint >= u.CommandingPoint)
                    {
                        Debug.Log($"BOT PLAYED CARD: {u.Name} AP: {u.AttackPoint} HP: {u.HealthPoint}");
                        commandingPoint -= u.CommandingPoint;
                        result.actions.Add(GetPlayCardAction(_gm.Player1.BaseContainer.Hand[cardScore.index]));
                    }
                }  
                else if(cardScore.card is EventCardModel e)
                {
                    Debug.Log($"BOT PLAYED CARD: {e.Name}");
                    if(commandingPoint >= e.CommandingPoint)
                    {
                        commandingPoint -= e.CommandingPoint;
                        result.actions.Add(GetPlayCardAction(_gm.Player1.BaseContainer.Hand[cardScore.index]));
                    }
                }
            }

            return result;
        }
        
        public Move CheckIfAttackToKillCard()
        {
            var result = new Move() { actions = new List<Action>() };
            var myCards = _gm.Player1.CardContainer.Cards;
            var oppCards = _gm.Player0.CardContainer.Cards;
            var killedIndexes = new List<int>();

            for (int i = 0; i < myCards.Count; i++)
            {
                var card = myCards[i] as UnitCardBoard;
                //eger coklu kart saldirmasini istiyorsan count checki kaldir ama index hatasi oluyor
                if(card.Model.CanIPlay && killedIndexes.Count == 0)
                {
                    //eger karta saldirdigimda kart oluyor ben olmuyorsam karta saldir
                    //kalan olmeyen kartlar icin 2 kartla saldirdigimda olabilcek olanlari hesapla
                    //aksi durumda commandera saldir
                    for (int j = 0; j < oppCards.Count; j++)
                    {
                        if (killedIndexes.Contains(j)) continue;
                        var oc = oppCards[j] as UnitCardBoard;
                        var targetHP = oc.Model.HealthPoint;
                        var targetAP = oc.Model.AttackPoint;
                        var sourceHP = card.Model.HealthPoint;
                        var sourceAP = card.Model.AttackPoint;

                        var isCardDiesButNotMe = sourceAP > targetHP && targetAP < sourceHP;
                        if (isCardDiesButNotMe)
                        {
                            result.actions.Add(() => 
                            {
                                _gm.BoardController.AttackCursor.AIAttack(card.transform.position, oc.transform.position);
                                _gm.BoardController.AttackCursor.SetAttackCursorSprite(card.Model, oc, null);
                                AudioManager.PlaySound("Cards/Attack-" + card.Model.Sound);
                                GetAttackCardWithCardAction(card.Index, oc.Index, card, oc).Invoke();
                            });
                            killedIndexes.Add(j);
                            break;
                        }
                    }
                }
            }
            
            return result;
        }

        static List<int> GetBestAttackValueCardIndexes(int W, int[] wt,int[] val, int n)
        {
            int i, w;
            int[,] K = new int[n + 1, W + 1];
            var result = new List<int>();
            for (i = 0; i <= n; i++)
            {
                for (w = 0; w <= W; w++)
                {
                    if (i == 0 || w == 0)
                        K[i, w] = 0;
                    else if (wt[i - 1] <= w)
                        K[i, w] = Math.Max(val[i - 1] +
                                K[i - 1, w - wt[i - 1]], K[i - 1, w]);
                    else
                        K[i, w] = K[i - 1, w];
                }
            }
            int res = K[n, W];

            w = W;
            for (i = n; i > 0 && res > 0; i--)
            {
                if (res == K[i - 1, w])
                    continue;
                else
                {
                    result.Add(i - 1);
                    res = res - val[i - 1];
                    w = w - wt[i - 1];
                }
            }

            if (result.Count <= 0)
            {
                int[] tempList = new int[n];
                Array.Copy(wt, tempList, n);
                Array.Sort(tempList);
                for (int j = 0; j < tempList.Length; j++)
                {
                    if (tempList[j] >= W)
                    {
                        var index = Array.IndexOf(wt, tempList[j]);
                        if(!result.Contains(index))
                            result.Add(index);
                        break;
                    }
                }
            }

            return result;
        }

        //eger karta saldirdiginda olduremicekse ve kendi olucekse commandera saldirsin
        //eger ikisi de olucekse saldirdiginda saldirsin
        public Move CheckIfAttackToStrongestCard()
        {
            var result = new Move() { actions = new List<Action>() };
            var myCards = new List<Card.Card>() {  };
            myCards.AddRange(_gm.Player1.CardContainer.Cards);
            myCards.RemoveAll(c => !(c as UnitCardBoard).Model.CanIPlay);
            var oppCards = new List<Card.Card>() {  };
            oppCards.AddRange(_gm.Player0.CardContainer.Cards);

            if(oppCards.Count > 0 && myCards.Count > 0)
            {
                bool resetIndex = false;
                oppCards = oppCards.OrderByDescending(c => (c as UnitCardBoard).Model.AttackPoint).ThenBy(c => (c as UnitCardBoard).Model.HealthPoint).ToList();
                for (int i = 0; i < oppCards.Count && myCards.Count > 0; i++)
                {
                    List<int> indexes = new List<int>();
                    var oc = oppCards[i] as UnitCardBoard;
                    var ocHealth = oc.Model.HealthPoint;
                    /*var res = findMin(myCards, myCards.Count, ocHealth);*/
                    var wt = myCards.Select(c => (c as UnitCardBoard).Model.AttackPoint).ToArray();
                    var res = GetBestAttackValueCardIndexes(ocHealth, wt, wt, wt.Length);
                    for (int j = 0; j < res.Count && myCards.Count > 0; j++)
                    {
                        var rc = myCards[res[j]] as UnitCardBoard;

                        if (rc.Model.HealthPoint <= oc.Model.AttackPoint && oc.Model.HealthPoint > rc.Model.AttackPoint)
                            break;

                        //var rc = res[j] as UnitCardBoard;
                        var ocDied = ocHealth - rc.Model.AttackPoint <= 0;
                        ocHealth -= rc.Model.AttackPoint;
                        Debug.Log($"OCDIED: {ocDied} OCHEALTH:{oc.Model.HealthPoint}");
                        result.actions.Add(() =>
                        {
                            Debug.Log($"RCINDEX: {rc.Index} RCNAME: {rc.Model.Name} RCINDEX:{rc.Index} RCAP:{rc.Model.AttackPoint} RCHP:{rc.Model.HealthPoint}");
                            Debug.Log($"OCINDEX: {oc.Index} OCNAME: {oc.Model.Name} OCINDEX:{oc.Index} OCAP:{oc.Model.AttackPoint} OCHP:{oc.Model.HealthPoint}");
                            _gm.BoardController.AttackCursor.AIAttack(_gm.Player1.CardContainer.Cards[rc.Index].transform.position, _gm.Player0.CardContainer.Cards[oc.Index].transform.position);
                            _gm.BoardController.AttackCursor.SetAttackCursorSprite(rc.Model, _gm.Player0.CardContainer.Cards[oc.Index] as UnitCardBoard, null);
                            AudioManager.PlaySound("Cards/Attack-" + rc.Model.Sound);
                            GetAttackCardWithCardAction(rc.Index, oc.Index, _gm.Player1.CardContainer.Cards[rc.Index] as UnitCardBoard, _gm.Player0.CardContainer.Cards[oc.Index] as UnitCardBoard).Invoke();
                        });

                        myCards.Remove(rc);
                        if (ocDied)
                        {
                            oppCards.RemoveAt(i--);
                            resetIndex = false;
                        }
                        else if (!ocDied && myCards.Count > 0) resetIndex = true;
                    }

                    if (resetIndex)
                    {
                        resetIndex = false;
                        i--;
                    }
                }                
            }

            return result;
        }

        public Move CheckIfAttackToCommander()
        {
            var result = new Move() { actions = new List<Action>() };
            var myCards = _gm.Player1.CardContainer.Cards;

            for (int i = 0; i < myCards.Count; i++)
            {
                var c = myCards[i] as UnitCardBoard;
                if (!c.Model.CanIPlay) continue;
                result.actions.Add(() =>
                {
                    _gm.BoardController.AttackCursor.AIAttack(c.transform.position, _gm.Player0.transform.position);
                    _gm.BoardController.AttackCursor.SetAttackCursorSprite(c.Model, null, _gm.Player0);
                    AudioManager.PlaySound("Cards/Attack-" + c.Model.Sound);
                    GetAttackCommanderWithCardAction(c.Index).Invoke();
                });
            }

            return result;
        }

        public Move CheckIfUseWeapon()
        {
            var result = new Move() { actions = new List<Action>() };
            if (_gm.Player1.Model.PlayerWeapon.AmmunationPoint <= 0) return result;
            var score = -50;
            SpecialEffectTarget target = SpecialEffectTarget.Unit;
            var oppPlayer = _gm.Player0;
            var oppCards = _gm.BoardController.BoardModel.boardCards[0];
            //karsi commanderin canina bak eger 1 ise score += 50
            //karsi oyuncunun kartlarina bak eger cani 1 olan kart varsa ve ammo sayim 1 den buyukse score += 50
            //degilse weapon kullanma

            CardScore bestCardScore = new();
            if(_gm.Player1.Model.PlayerWeapon.AmmunationPoint > 1)
                for (int i = 0; i < oppCards.Count; i++)
                {
                    var c = oppCards[i] as UnitCardModel;
                    if(c.HealthPoint == 1)
                    {
                        var cs = CalculateToPlayCardScore(c, 0);
                        cs.index = i;
                        if (cs.score > bestCardScore.score)
                            bestCardScore = cs;
                        score += 55;
                        target = SpecialEffectTarget.Unit;
                    }
                }

            if(oppPlayer.Model.HealthPoint <= 1)
            {
                score += 100;
                target = SpecialEffectTarget.OppCommander;
            }

            if(score > 0)
            {
                result.actions.Add(() =>
                {
                    _gm.BoardController.AttackCursor.AIAttack(_gm.Player1.Weapon.transform.position, target == SpecialEffectTarget.OppCommander ? _gm.Player0.transform.position : _gm.Player0.CardContainer.Cards[bestCardScore.index].transform.position );
                    if(target == SpecialEffectTarget.OppCommander)
                        _gm.BoardController.AttackCursor.SetAttackCursorSprite(_gm.Player1.Model.PlayerWeapon, null, _gm.Player0);
                    else
                        _gm.BoardController.AttackCursor.SetAttackCursorSprite(_gm.Player1.Model.PlayerWeapon, _gm.Player0.CardContainer.Cards[bestCardScore.index] as UnitCardBoard, null);
                    GetAttackWeaponAction(target, bestCardScore.index).Invoke();
                    _gm.Player1.DidIUsedWeaponSprite = true;
                    _gm.Player1.WeaponCover(false);
                });
            }

            return result;
        }

        //eger yerdeki kart sayim 3 ten buyukse ve cp > 3 se special effect kullan 
        //cani ve score u en yuksek olan karta buff ver

        //elimde hic kartim kalmadi ve 2cp veya daha fazla cp var ise
        public Move CheckIfUseSpecialAbility()
        {
            var result = new Move() { actions = new List<Action>() };
            var myCards = _gm.BoardController.BoardModel.boardCards[1];

            if ((_gm.Player1.Model.CommandingPoint - _gm.Player1.Model.CommanderSpecialEffectCard.CommandingPoint) >= 0)
            {
                var cardScoreList = GetCardScores(false);
                cardScoreList = cardScoreList.OrderBy(c => c.score).ThenBy(c=> (c.card as UnitCardModel).HealthPoint).ToList();

                if(cardScoreList.Count > 0)
                {
                    var c = cardScoreList[0];
                    result.actions.Add(() =>
                    {
                        AudioManager.PlaySound("CommanderAbility-DE");
                        Debug.Log($"SPEICAL ABILITY C INDEX: {c.index} CARD NAME: {c.card.Name} CARD AP: {(c.card as UnitCardModel).AttackPoint} CARD HP: {(c.card as UnitCardModel).HealthPoint}");
                        _gm.BoardController.AttackCursor.AIAttack(_gm.Player1.SpecialAbility.transform.position, _gm.Player1.CardContainer.Cards[c.index].transform.position, false);
                        GetUseSpeicalAbilityAction(c.index).Invoke();
                        _gm.Player1.DidIUsedSpecialAbilitySprite = true;
                        _gm.Player1.SpecialAbilityCover(false);
                    });
                }
            }

            return result;
        }

        #region ACTIONREGION

        public Action GetMulliganAction()
        {
            //mulligan kartlarini al 4 tane
            //cp si 4 olanlari desteye ekle
            //yerlerine discarded kadar kart al
            //returnle
            var mulliganCards = _gm.BoardController.BoardModel.GetMulliganCards(4, false);
            var toDiscardIndexes = new List<int>();
            for (int i = mulliganCards.Count - 1; i >= 0; i--)
            {
                var c = mulliganCards[i];
                if (c is UnitCardModel u && u.CommandingPoint > 3 || c is EventCardModel e && e.CommandingPoint > 3)
                    toDiscardIndexes.Add(i);
            }

            toDiscardIndexes.ForEach(i => 
            {
                var card = mulliganCards[i];
                _gm.BoardController.BoardModel.AddToDeck(card, 0, false);
                mulliganCards.Remove(card); 
            });

            mulliganCards.ForEach(c => _gm.BoardController.BoardModel.AddToDeck(c, _gm.BoardController.BoardModel.OppDeck.Count, false));

            //sonraki el eline gelicek 2 cp lik kart olsun diye 1 tane random secilmis karti ekliyoruz 5.indexe
            var nextRoundCard = _gm.BoardController.BoardModel.GetRandomCardWithCP(2, false);
            _gm.BoardController.BoardModel.RemoveCardFromDeck(nextRoundCard, false);
            _gm.BoardController.BoardModel.AddToDeck(nextRoundCard, 4, false);

            return new Action(() =>
            {
                for (int i = 0; i < 4; i++)
                {
                    _gm.BoardController.PickCard();
                }
            });
        }

        public Action GetAttackWeaponAction(Enums.SpecialEffectTarget target, int targetIndex = 0)
        {
            if (target == Enums.SpecialEffectTarget.Unit)
            {
                return new Action(() =>
                {
                    _board.AttackWeaponToCard(targetIndex, _board.Player0.CardContainer.Cards[targetIndex] as UnitCardBoard);
                });
            }
            else if(target == Enums.SpecialEffectTarget.OppCommander)
            {
                return new Action(() =>
                {
                    _board.AttackWeaponToPlayer();
                });
            }

            return GetDefaultAction;
        }

        private Action GetAttackCommanderWithCardAction(int index)
        {
            return new Action(() =>
            {
                _gm.BoardController.AttackCardToPlayer(index);
            });
        }

        private Action GetAttackCardWithCardAction(int sourceIndex, int targetIndex, UnitCardBoard sourceCard, UnitCardBoard targetCard)
        {
            return new Action(() =>
            {
                _gm.BoardController.AttackCardToCard(sourceIndex, targetIndex, sourceCard, targetCard);
            });
        }


        private Action GetPlayCardAction(Card.Card card)
        {
            return new Action(() =>
            {
                _gm.Player1.BaseContainer.GoToBattleField(card);
            });
        }
        private Action GetUseSpeicalAbilityAction(int targetIndex)
        {
            return new Action(() =>
            {
                _gm.BoardController.UpdateEventCardUsedCP(_gm.Player1.Model.CommanderSpecialEffectCard);
                _gm.BoardController.ApplyCardSpecialEffect(_gm.Player1.Model.CommanderSpecialEffectCard, targetIndex);
            });
        }

        public Action GetEndTurnAction()
        {
            return new Action(() => _board.EndTurn());
        }

        private Action GetDefaultAction => new(() => { });

        #endregion
    }
}

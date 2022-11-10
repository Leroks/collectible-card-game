using StormWarfare.Card;
using StormWarfare.Gameplay.Bot;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormWarfare.Gameplay
{
    public class AIPlayer : Commander
    {
        static CardContainer _cardContainer;
        static BaseContainer _baseContainer;

        public Bot.Bot Bot;

        public GameObject Weapon;
        public GameObject SpecialAbility;

        private void Start()
        {
            if (_cardContainer == null)
                _cardContainer = GameManager.Instance.Player1.CardContainer;
            if (_baseContainer == null)
                _baseContainer = GameManager.Instance.Player1.BaseContainer;
            Bot = new();
            //EventManager.OnPickNewCard += DefaultPositionChanged;
        }
        //karsi oyuncunun demo icin AI kart alma methodu
        public void PickCard()
        {
            //ters sekilde bir kart yarat karsi oyuncunun deck pozisyonunda adamin elindeki kartlarina ekle
            GameManager.Instance.BoardController.PickCard();
        }

        public void PlaceCardToBattleGround()
        {
            //karsi oyuncu AI simdilik elindeki son karti modelini alip prefabini olusturup board uzerindeki containeri icerisine koysun
            var model = GameManager.Instance.BoardController.BoardModel;
            var nextAiCardModel = model.OppHand[^1];
            Card.Card nextAiCard = _baseContainer.Hand[^1] as UnitCardHand;
            // IF UNIT CARD IS NULL WHICH MEANS CARD IS EVENT CARD, USE ABILITY AND DESTROY
            if (nextAiCard == null)
            {
                nextAiCard = _baseContainer.Hand[^1] as EventCard;
                _cardContainer.PlaceNewCardToBattleGround(nextAiCard, null);
                _baseContainer.CardPlacedToBattleGround(nextAiCard, null);
                Destroy(nextAiCard.transform.gameObject);
                return;
            }
            _cardContainer.PlaceNewCardToBattleGround(nextAiCard, (nextAiCard as UnitCardHand).Model);
            _baseContainer.CardPlacedToBattleGround(nextAiCard, (nextAiCard as UnitCardHand).Model);
        }

        public void Play() => StartCoroutine(PlayAction());

        IEnumerator PlayAction()
        {
            bool waitPrevMove = false;
            Random rand = new Random();
            if(GameManager.Instance.BoardController.BoardModel.BoardState == Models.BoardModel.BoardStates.Mulligan)
            {
                Debug.Log("BOT PLAYED MULLIGAN");
                Bot.GetMulliganAction().Invoke();
                yield break;
            }

            //enemy turndeyken kart oynamasin yere
            yield return new WaitForSeconds(4f);
            foreach (var action in Bot.CheckValidMoves().actions)
            {
                Debug.Log("BOT PLAYED CARD");
                waitPrevMove = true;
                yield return new WaitForSeconds(rand.Int(2,4));
                action.Invoke();
            }

            if(waitPrevMove)
            {
                yield return new WaitForSeconds(1.3f);
                waitPrevMove = false;
            }
            foreach (var action in Bot.CheckIfUseSpecialAbility().actions)
            {
                Debug.Log("BOT USED SPECIAL ABILITY");
                waitPrevMove = true;
                yield return new WaitForSeconds(1);
                action.Invoke();
            }

            if(waitPrevMove)
            {
                yield return new WaitForSeconds(1.3f);
                waitPrevMove = false;
            }
            foreach (var action in Bot.CheckIfAttackToKillCard().actions)
            {
                Debug.Log("BOT ATTACKED AND KILLED CARD");
                waitPrevMove = true;
                yield return new WaitForSeconds(rand.Int(1, 3));
                action.Invoke();
            }

            if(waitPrevMove)
            {
                yield return new WaitForSeconds(2.5f);
                waitPrevMove = false;
            }
            foreach (var action in Bot.CheckIfAttackToStrongestCard().actions)
            {
                Debug.Log("BOT ATTACKED STRONGEST CARD");
                waitPrevMove = true;
                yield return new WaitForSeconds(rand.Int(2,4));
                action.Invoke();
            }

            if(waitPrevMove)
            {
                yield return new WaitForSeconds(2.5f);
                waitPrevMove = false;
            }

            foreach (var action in Bot.CheckIfAttackToCommander().actions)
            {
                Debug.Log("BOT ATTACKED TO COMMANDER");
                if (GameManager.Instance.BoardController.BoardModel.BoardState == Models.BoardModel.BoardStates.Finish)
                    yield break;
                yield return new WaitForSeconds(rand.Int(2, 4));
                action.Invoke();
            }

            foreach (var action in Bot.CheckIfUseWeapon().actions)
            {
                Debug.Log("BOT ATTACKED WITH WEAPON");
                if (GameManager.Instance.BoardController.BoardModel.BoardState == Models.BoardModel.BoardStates.Finish)
                    yield break;
                yield return new WaitForSeconds(1);
                action.Invoke();
            }

            Bot.GetEndTurnAction().Invoke();
        }
    }
}
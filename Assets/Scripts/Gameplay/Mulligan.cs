using StormWarfare.Card;
using StormWarfare.Gameplay;
using StormWarfare.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormWarfare.Gameplay
{
    public class Mulligan : MonoBehaviour
    {
        [SerializeField] int _cardWidth;
        [SerializeField] Transform _spawnPoint;
        [SerializeField] GameObject _background;
        Camera _cam;

        [SerializeField] BoxCollider2D _col;
        GameManager _gm;
        BoardModel _bm;
        List<BaseCardModel> _mulliganCards;
        List<BaseCardModel> _finalCards = new List<BaseCardModel>();
        List<int> _choosenCards;
        // Use this for initialization
        List<Card.Card> _cards = new List<Card.Card>();
        public void SetChildPositions(bool hideChoosen = false)
        {
            var count = _mulliganCards.Count + _finalCards.Count;
            var totalWidth = count * _cardWidth;
            for (int i = 0; i < count; i++)
            {
                var tempPos = transform.position;
                tempPos.x = 0;
                tempPos.x = (_cardWidth - totalWidth) * .5f + i * _cardWidth;
                var card = _cards[i];
                card.MoveMulliganCard(tempPos, !hideChoosen);
                if (hideChoosen)
                {
                    card.MulliganChoose.SetActive(false);
                    card.NotChoose.SetActive(false);
                }
            }
            var tempVec2 = new Vector2(0, 540f);
            tempVec2.x = totalWidth;
            _col.size = tempVec2;
        }

        private void OnMouseDown()
        {
            var index = GetCardIndexFromMousePosX();
            if (index == -1) return;
            ChooseDiscardedCard(index);
            var isChoosen = _choosenCards[index] == 0;
            var selectedCard = _cards[index];
            AudioManager.PlaySound("MulliganCardChoose");
            selectedCard.MulliganChoose.SetActive(isChoosen);
            selectedCard.NotChoose.SetActive(!isChoosen);
        }
        Vector3 MouseToWorldPoint(Camera cam, float distance)
        {
            var m = Input.mousePosition;
            m.z = distance;
            return cam.ScreenToWorldPoint(m);
        }
        public int GetCardIndexFromMousePosX()
        {
            var count = _cards.Count;
            if (count < 1) return -1;
            Vector3 mousePoint = MouseToWorldPoint(_cam, -_cam.transform.position.z);
            mousePoint.x += _col.size.x / 2;
            return Mathf.Clamp(mousePoint.x.ToInt() / _cardWidth.ToInt() ,0, count - 1);
        }

        void Start()
        {
            _cam = _cam == null ? Camera.main : _cam;
            _gm = GameManager.Instance;
            _bm = _gm.BoardController.BoardModel;
            _mulliganCards = _bm.GetMulliganCardsRandomized(_bm.IsMyTurn ? 3 : 4);
            _choosenCards = _bm.IsMyTurn ? new List<int>() { 0, 0, 0 } : new List<int>() { 0, 0, 0, 0 };
            ShowMulliganCards();
            AudioManager.PlaySound("MulliganStart");
        }

        void ShowMulliganCards()
        {
            for (int i = 0; i < _mulliganCards.Count; i++)
            {
                var cardModel = _mulliganCards[i];
                switch (cardModel)
                {
                    case UnitCardModel:
                        var unitCard = Instantiate(Resources.Load("Prefabs/2D/UnitCardHand", typeof(UnitCardHand)) as UnitCardHand, _spawnPoint.localPosition, Quaternion.identity, transform);
                        unitCard.InitCard(cardModel as UnitCardModel);
                        _cards.Add(unitCard);
                        break;
                    case EventCardModel:
                        var eventCard = Instantiate(Resources.Load("Prefabs/2D/EventCardHand", typeof(EventCard)) as EventCard, _spawnPoint.localPosition, Quaternion.identity, transform);
                        eventCard.InitCard(cardModel as EventCardModel);
                        _cards.Add(eventCard);
                        break;
                }
            }
            SetChildPositions();
        }

        /// <summary>
        /// her sectigi kart icin icin bu methodu cagir indexiyle birlikte eger kart secildiyse secimi kaldir
        /// </summary>
        /// <param name="index"></param>
        public void ChooseDiscardedCard(int index)
        {
            var isChoosen = _choosenCards[index] == 1;
            _choosenCards[index] = isChoosen ? 0 : 1;
        }

        public void CompleteMulligan()
        {
            int discardedCount = 0;
            for (int i = 0; i < _choosenCards.Count; i++)
            {
                if (_choosenCards[i] != 1) continue;
                ReplaceDiscardedCard(_mulliganCards[i]);
                _bm.AddToDeck(_mulliganCards[i], _bm.MyDeck.Count);
                _mulliganCards.RemoveAt(i);
                _choosenCards.RemoveAt(i);
                var card = _cards[i];
                _cards.RemoveAt(i);
                Destroy(card.gameObject);
                i--;
                discardedCount++;
            }
            _finalCards.AddRange(_bm.GetMulliganCards(discardedCount));
            FinishMulligan();
            AudioManager.PlaySound("MulliganEnd");
        }

        public void ReplaceDiscardedCard(BaseCardModel card)
        {
            var cpCard = new BaseCardModel();
            if(card is UnitCardModel c)
                cpCard = _bm.GetRandomCardWithCP(c.CommandingPoint == 1 ? c.CommandingPoint : -1); 
            else if(card is EventCardModel e)
                cpCard = _bm.GetRandomCardWithCP(e.CommandingPoint == 1 ? e.CommandingPoint : -1); 

            _bm.RemoveCardFromDeck(cpCard);
            _bm.AddToDeck(cpCard, 0);
        }

        /// <summary>
        /// _finalCards icerisindeki kartlari instantiate et ekranda goster sonrasinda elime ekle
        /// </summary>
        void FinishMulligan()
        {
            _bm.MyHand.AddRange(_finalCards);
            for (int i = 0; i < _finalCards.Count; i++)
            {
                var cardModel = _finalCards[i];
                switch (cardModel)
                {
                    case UnitCardModel:
                        var unitCard = Instantiate(Resources.Load("Prefabs/2D/UnitCardHand", typeof(UnitCardHand)) as UnitCardHand, _spawnPoint.localPosition, Quaternion.identity, transform);
                        unitCard.InitCard(cardModel as UnitCardModel);
                        _cards.Add(unitCard);
                        break;
                    case EventCardModel:
                        var eventCard = Instantiate(Resources.Load("Prefabs/2D/EventCardHand", typeof(EventCard)) as EventCard, _spawnPoint.localPosition, Quaternion.identity, transform);
                        eventCard.InitCard(cardModel as EventCardModel);
                        _cards.Add(eventCard);
                        break;
                }
            }
            SetChildPositions(true);
            StartCoroutine(EventCardWaitForOneSecond());
        }

        IEnumerator EventCardWaitForOneSecond()
        {
            yield return new WaitForSeconds(1f);
            var container = _gm.BoardController.Player0.BaseContainer;
            for (int i = 0; i < _cards.Count; i++)
            {
                _cards[i].transform.SetParent(container.transform);
            }
            gameObject.SetActive(false);
            _gm.BoardController.EndMulligan();
            container.Hand.AddRange(_cards);
            container.SetChildPositions(true);
        }
    }
}
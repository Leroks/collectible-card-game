using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StormWarfare.Card;
using StormWarfare.Gameplay;
using StormWarfare.Models;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseContainer : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject Anchor;
    public float VerticalMargin = 600;
    public bool IsOpponent;
    public List<Card> Hand;

    readonly float _zRot = 0.13f;
    readonly float _cardAngle = 8f;
    readonly float radius = 229.2f;
    readonly int _screenHeight = Screen.height;
    public int cardcount;
    public int ColliderCardWidth;
    float _totalWidth;
    [SerializeField] Vector3 _hoverScale;
    [SerializeField] BoxCollider2D _col;
    internal Camera _cam;
    int _lastIndex = -1;
    float _lastPercentage;

    Vector3 _screenPoint;
    [SerializeField] CardContainer _cardContainer;
    public Card DraggingCard;

    bool _simulate;

    public Transform EventCardCheckPoint;
    public Transform PickCardCheckPoint;
    Commander _cm;
    Command baseMechanism;
    [SerializeField] Animator animator;
    [SerializeField] string animationString;
    void Start()
    {
        Hand = new List<Card>();
        var colsize = _col.size;
        _totalWidth = colsize.x = ColliderCardWidth * cardcount;
        _col.size = colsize;
        SetChildPositions();
        _cam = _cam == null ? Camera.main : _cam;
        _cm = transform.parent.GetComponent<Commander>();
        //baseMechanism = new BaseMechanism(animator, animationString);
    }

    public void CardRetreat(Card card)
    {
        Hand.Add(card);
        SetChildPositions();
    }
    public void NewCardPicked(Card card, int drawCount = 0)
    {
        if (IsOpponent)
        {
            Hand.Add(card);
            StartCoroutine(CardPickWaitForOneSecondAI(card));
            return;
        }
        if (_drawCardCount == 0 && drawCount > 0)
            _tempDrawCardCount =  _drawCardCount = drawCount;
        StartCoroutine(CardPickWaitForOneSecond(card, drawCount));
        //baseMechanism.Execute();
    }
    public void MulliganEnd()
    {
        SetChildPositions();
        //baseMechanism.Execute();
    }
    IEnumerator CardPickWaitForOneSecondAI(Card card)
    {
        if (GameManager.Instance.BoardController.BoardModel.BoardState == BoardModel.BoardStates.Mulligan)
        {
            yield return new WaitForSeconds(1f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
            AudioManager.PlaySound("CardPick");
        }
        card.gameObject.SetActive(true);
        SetChildPositions();
    }
    int _drawCardCount;
    int _tempDrawCardCount;

    IEnumerator CardPickWaitForOneSecond(Card card, int drawCount = 0)
    {
        var cardInitDelay = 2f;
        if (drawCount > 0)
            cardInitDelay = ((drawCount - _drawCardCount--) * 1.7f) ;
        yield return new WaitForSeconds(cardInitDelay);
        card.MoveCardEase(PickCardCheckPoint.localPosition, Quaternion.identity, false, true);
        yield return new WaitForSeconds(1.7f);
        _tempDrawCardCount--;
        card.NotHoverCard();
        Hand.Add(card);
        if (drawCount > 0)
            SetChildPositions(false, _tempDrawCardCount);
        else
            SetChildPositions();
        GameManager.Instance.BoardController.SceneBlocker.SetActive(false);
    }
    public void CardPlacedToBattleGround(Card card, UnitCardModel model)
    {
        Hand.Remove(card);
        SetChildPositions();
        //baseMechanism.Execute();
    }

    public void SetChildPositions(bool mulliganDelay = false, int positionChildCount = 0)
    {
        var count = Hand.Count;
        if (count < 1) return;
        count += 1;
        int pieAnchor;
        Quaternion rotOffset = Quaternion.identity;
        if (IsOpponent)
        {
            rotOffset.y = _zRot;
            pieAnchor = 270;
        }
        else
        {
            rotOffset.y = _zRot;
            pieAnchor = 90;
        }
        for (int i = 1; i < count; i++)
        {
            var pieAngle = _cardAngle * (count);
            float theta;
            var tempPos = Vector3.zero;
            theta = ((pieAngle * i / count) + (pieAnchor - (pieAngle / 2))) * Mathf.Deg2Rad;
            tempPos.x = -Mathf.Cos(theta) * radius;
            tempPos.z = 0;
            tempPos.y = (Mathf.Sin(theta) * radius) + VerticalMargin;
            Vector3 difference = Anchor.transform.localPosition - tempPos;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            var card = Hand[i - 1];
            card.GetComponent<SortingGroup>().sortingOrder = i - 1;
            if (positionChildCount > 0 && i == count - 1)
                card.DefaultPositionChanged(tempPos, Quaternion.Euler(new Vector3(0, 0, rotationZ + 90)));
            card.MoveCard(tempPos, Quaternion.Euler(new Vector3(0, 0, rotationZ + 90)), true, false, mulliganDelay);
            Card model = (card as UnitCardHand);
            if (model == null)
            {
                card.CanIPlayThisCard = _cm.Model.CommandingPoint >= (card as EventCard).Model.CommandingPoint;
                card.CanIPlayThisCardLight.SetActive(card.CanIPlayThisCard);
                continue;
            }
            card.CanIPlayThisCard = _cm.Model.CommandingPoint >= (card as UnitCardHand).Model.CommandingPoint;
            card.CanIPlayThisCardLight.SetActive(card.CanIPlayThisCard);
        }
        var colsize = _col.size;
        count -= 1;
        _totalWidth = colsize.x = Mathf.Sqrt(2) * radius * Mathf.Sqrt(1 - MathF.Cos(_cardAngle * count * Mathf.Rad2Deg));
        _col.size = colsize;
        var pos = Vector3.zero;
        pos.y = (Mathf.Sin(((_cardAngle) + (pieAnchor - (_cardAngle / 2))) * Mathf.Deg2Rad) * radius) + VerticalMargin;
    }
    public void GoToBattleField(Card kard)
    {
        Card card = kard.GetComponent<Card>() as UnitCardHand;
        // IF UNIT CARD IS NULL WHICH MEANS CARD IS EVENT CARD, USE ABILITY AND DESTROY
        if (card == null)
        {
            card = kard.GetComponent<Card>() as EventCard;
            if ((card as EventCard).Model.CommandingPoint > _cm.Model.CommandingPoint)
            {
                SetChildPositions();
                _cardContainer.SetChildPositions(-1);
                return;
            }
            _cardContainer.PlaceNewCardToBattleGround(card, null);
            CardPlacedToBattleGround(card, null);
            card.MoveAICard(EventCardCheckPoint.localPosition, new Quaternion(-1f, 0f, 0f, 1f), false, true, true);
            card.PlaySound = true;
            GameManager.Instance.BoardController.BoardModel.RemoveEventCardFromHand(kard.GetComponent<EventCard>().Model);
            StartCoroutine(EventCardWaitForOneSecond(card, kard.GetComponent<EventCard>().Model));
            return;
        }
        if (_cardContainer.Cards.Count >= 7)
        {
            SetChildPositions();
            _cardContainer.SetChildPositions(-1);
            return;
        }
        if ((card as UnitCardHand).Model.CommandingPoint > _cm.Model.CommandingPoint)
        {
            SetChildPositions();
            _cardContainer.SetChildPositions(-1);
            return;
        }
        _cardContainer.PlaceNewCardToBattleGround(card, (card as UnitCardHand).Model, true);
        CardPlacedToBattleGround(card, (card as UnitCardHand).Model);
    }

    public void GoToBattleField()
    {
        Card card = DraggingCard.GetComponent<Card>() as UnitCardHand;
        // IF UNIT CARD IS NULL WHICH MEANS CARD IS EVENT CARD, USE ABILITY AND DESTROY
        if (card == null)
        {
            card = DraggingCard.GetComponent<Card>() as EventCard;
            if ((card as EventCard).Model.CommandingPoint > _cm.Model.CommandingPoint)
            {
                SetChildPositions();
                _cardContainer.SetChildPositions(-1);
                return;
            }
            _cardContainer.PlaceNewCardToBattleGround(card, null);
            CardPlacedToBattleGround(card, null);
            card.MoveCard(EventCardCheckPoint.localPosition, Quaternion.identity, false, true);
            card.PlaySound = true;
            GameManager.Instance.BoardController.BoardModel.RemoveEventCardFromHand(DraggingCard.GetComponent<EventCard>().Model);
            StartCoroutine(EventCardWaitForOneSecond(card, DraggingCard.GetComponent<EventCard>().Model));
            return;
        }
        if (_cardContainer.Cards.Count >= 7)
        {
            DraggingCard = null;
            SetChildPositions();
            _cardContainer.SetChildPositions(-1);
            return;
        }
        if ((card as UnitCardHand).Model.CommandingPoint > _cm.Model.CommandingPoint)
        {
            SetChildPositions();
            _cardContainer.SetChildPositions(-1);
            return;
        }
        _cardContainer.PlaceNewCardToBattleGround(card, (card as UnitCardHand).Model);
        CardPlacedToBattleGround(card, (card as UnitCardHand).Model);
    }

    IEnumerator EventCardWaitForOneSecond(Card card, EventCardModel model)
    {
        GameManager.Instance.BoardController.UpdateEventCardUsedCP(model);
        GameManager.Instance.BoardController.ApplyCardSpecialEffect(model);
        yield return new WaitForSeconds(2.2f);
        //todo modeli direk guncelle ama sonradan base containeri refresh et
        Destroy(card.transform.gameObject);
    }
    public void GoToPlayerField()
    {
        DraggingCard.NotHoverCard();
    }


    void OnMouseDown()
    {
        if (GameManager.Instance.BoardController.AttackCursor.IsAttackShowing) return;
        if (IsOpponent || _lastIndex < 0 || !(GameManager.Instance.BoardController.BoardModel.IsMyTurn && GameManager.Instance.BoardController.BoardModel.IsPlaying)) return;
        DraggingCard = Hand[GetCardIndexFromMousePosX()];
        if (!DraggingCard.CanIPlayThisCard)
        {
            DraggingCard = null;
            return;
        }
        DraggingCard.StopCurrentCoRoutine();
        DraggingCard.transform.localScale = Vector3.one;
        _screenPoint = _cam.WorldToScreenPoint(DraggingCard.transform.position);
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.BoardController.AttackCursor.IsAttackShowing) return;
        if (IsOpponent || DraggingCard == null || !(GameManager.Instance.BoardController.BoardModel.IsMyTurn && GameManager.Instance.BoardController.BoardModel.IsPlaying)) return;
        if (_lastPercentage >= 30)
            GoToBattleField();
        else
            GoToPlayerField();
        DraggingCard = null;
        _lastIndex = -1;
    }
    void OnMouseDrag()
    {
        if (GameManager.Instance.BoardController.AttackCursor.IsAttackShowing) return;
        if (IsOpponent || !(GameManager.Instance.BoardController.BoardModel.IsMyTurn && GameManager.Instance.BoardController.BoardModel.IsPlaying) || DraggingCard == null)
        {
            if (DraggingCard != null)
            {
                SetChildPositions();
                DraggingCard = null;
            }
            return;
        }
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        DraggingCard.transform.position = curPosition;
        _lastPercentage = MousePosToScreenHeightPercentage(Input.mousePosition.y);
        if (!_simulate && _lastPercentage < 30)
        {
            _cardContainer.EndSimulateCardHover();
            _simulate = true;
        }
        else if (_lastPercentage >= 30)
        {
            _cardContainer.SimulateCardHover(DraggingCard);
            _simulate = false;
        }
    }
    void OnMouseOver()
    {
        if (GameManager.Instance.BoardController.AttackCursor.IsAttackShowing) return;
        if (IsOpponent || DraggingCard != null) return;
        var index = GetCardIndexFromMousePosX();
        if (_lastIndex != index)
        {
            if (_lastIndex > -1)
                Hand[_lastIndex].NotHoverCard();
            Hand[index].HoverCard();
            //baseMechanism.Execute();
            _lastIndex = index;
        }
    }
    void OnMouseExit()
    {
        if (GameManager.Instance.BoardController.AttackCursor.IsAttackShowing) return;
        if (IsOpponent || DraggingCard != null || _lastIndex == -1) return;
        Hand[_lastIndex].NotHoverCard();
        _lastIndex = -1;
    }

    public int GetCardIndexFromMousePosX()
    {
        var count = Hand.Count;
        if (count < 1) return -1;
        Vector3 mousePoint = MouseToWorldPoint(_cam, -_cam.transform.position.z);
        mousePoint.x += _col.size.x / 2;
        return Mathf.Clamp(mousePoint.x.ToInt() / (_col.size.x / count).ToInt(), 0, count - 1);
    }
    float MousePosToScreenHeightPercentage(float mousePosY) => mousePosY / _screenHeight * 100;
    Vector3 MouseToWorldPoint(Camera cam, float distance)
    {
        var m = Input.mousePosition;
        m.z = distance;
        return cam.ScreenToWorldPoint(m);
    }
}
using StormWarfare.Gameplay;
using StormWarfare.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace StormWarfare.Card
{
    public class MouseBehaviour : MonoBehaviour
    {
        internal int _screenHeight = Screen.height;
        float _mZCoord;
        float _lastPercentage;
        internal bool _onMouseEnterFired;
        internal static bool _onMouseDownFired;
        internal bool _isMouseDragging;
        static CardContainer _cardContainer;
        static BaseContainer _baseContainer;

        [SerializeField]
        Vector3 _hoverScale;
        Vector3 _targetPos;

        public Vector3 DefaultPosition;
        float _animSpeed = 20;
        internal Vector3 _defaultScale;
        Quaternion _targetRot, _defaultRot;
        Vector3 _startingPos;
        [SerializeField]
        Vector3 _mOffset;

        BoxCollider _col;
        internal Camera _cam;
        [SerializeField]
        private Card _card;
        Coroutine _co;
        public string DefaultLayer;
        public string MouseDragLayer;
        public SortingGroup SortingGroup;

        private void Start()
        {
            if (_cardContainer == null)
                _cardContainer = GameManager.Instance.Player0.CardContainer;
            if (_baseContainer == null)
                _baseContainer = GameManager.Instance.Player0.BaseContainer;
            _cam = _cam == null ? Camera.main : _cam;
            _col = GetComponent<BoxCollider>();
            _defaultScale = transform.parent.localScale;
            SortingGroup = transform.parent.GetComponent<SortingGroup>();
            //EventManager.OnPickNewCard += DefaultPositionChanged;
        }
        public void DefaultPositionChanged(Vector3 pos, Quaternion rot)
        {
            DefaultPosition = pos;
            _defaultRot = _targetRot = rot;
            _targetRot.y = 0;
            _targetRot.z = 0;
            _targetRot.x = 0;
            _targetPos = pos;
            _targetPos.y += 250;
            _targetPos.z -= 50;
        }
        float MousePosToScreenHeightPercentage(float mousePosY) => mousePosY / _screenHeight * 100;

        public void GoToBattleField()
        {
            Card card = transform.parent.GetComponent<Card>() as UnitCardHand;
            // IF UNIT CARD IS NULL WHICH MEANS CARD IS EVENT CARD, USE ABILITY AND DESTROY
            if (card == null)
            {
                card = transform.parent.GetComponent<Card>() as EventCard;
                _cardContainer.PlaceNewCardToBattleGround(card, null);
                _baseContainer.CardPlacedToBattleGround(card, null);
                GameManager.Instance.BoardController.UpdateEventCardUsedCP(card.GetComponent<EventCard>().Model);
                GameManager.Instance.BoardController.ApplyCardSpecialEffect(card.GetComponent<EventCard>().Model);
                Destroy(transform.parent.gameObject);
                return;
            }
            _cardContainer.PlaceNewCardToBattleGround(card, (card as UnitCardHand).Model);
            _baseContainer.CardPlacedToBattleGround(card, (card as UnitCardHand).Model);
        }

        public void GoToPlayerField()
        {
            transform.parent.localScale = _defaultScale;
            transform.parent.localRotation = _defaultRot;
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(SmoothTranslation(DefaultPosition, 10));
        }

        Vector3 GetMouseAsWorldPoint()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = _mZCoord;
            return _cam.ScreenToWorldPoint(mousePoint);
        }
        void OnMouseDown()
        {
            _onMouseDownFired = true;
            if (_co != null) StopCoroutine(_co);
            transform.parent.localScale = _defaultScale;
            _startingPos = transform.parent.position;
            _mZCoord = _cam.WorldToScreenPoint(_startingPos).z;
        }

        void OnMouseUp()
        {
            _onMouseEnterFired = _isMouseDragging = _onMouseDownFired = false;
            if (_lastPercentage >= 30 && _cardContainer.Cards.Count < 7)
                GoToBattleField();
            else
                GoToPlayerField();
        }
        void OnMouseDrag()
        {
            _isMouseDragging = true;
            _lastPercentage = MousePosToScreenHeightPercentage(Input.mousePosition.y);
            transform.parent.position = GetMouseAsWorldPoint() + _mOffset;
            if (_lastPercentage < 30)
            {
                _cardContainer.EndSimulateCardHover();
            }
            else
            {
                if (_cardContainer.Cards.Count > 6) return; 
                _cardContainer.SimulateCardHover(_card);
            }
        }
        void OnMouseExit()
        {
            if (_isMouseDragging || _onMouseDownFired || !_onMouseEnterFired) return;
            Debug.Log("EXIT");
            _onMouseEnterFired = false;
            if (_co != null) StopCoroutine(_co);
            SortingGroup.sortingLayerName = DefaultLayer;
            _co = StartCoroutine(ScaleDown(_defaultScale, _animSpeed));
        }

        void OnMouseOver()
        {
            if (_isMouseDragging || _onMouseDownFired || _onMouseEnterFired /*|| _isMouseDragging*/) return;
            //Debug.Log("OVER");
            _onMouseEnterFired = true;
            if (_co != null) StopCoroutine(_co);
            SortingGroup.sortingLayerName = MouseDragLayer;
            _co = StartCoroutine(ScaleUp(_hoverScale, _animSpeed));
        }

        IEnumerator SmoothTranslation(Vector3 target, float speed)
        {
            while (transform.parent.localPosition != target) 
            {
                //Debug.Log("MOVING1");
                transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, target, Time.deltaTime * speed);
                yield return null;
            }
            yield break;
        }

        IEnumerator ScaleDown(Vector3 target, float speed)
        {
            while (transform.parent.localScale != target)
            {
                //Debug.Log("MOVING2");
                transform.parent.localRotation = Quaternion.Lerp(transform.parent.localRotation, _defaultRot, Time.deltaTime * speed);
                transform.parent.localScale = Vector3.Lerp(transform.parent.localScale, target, Time.deltaTime * speed);
                transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, DefaultPosition, Time.deltaTime * speed);
                yield return null;
            }
            yield break;
        }
        IEnumerator ScaleUp(Vector3 target, float speed)
        {
            while (transform.parent.localScale != target)
            {
                //Debug.Log("MOVING3");
                transform.parent.localRotation = Quaternion.Lerp(transform.parent.localRotation, _targetRot, Time.deltaTime * speed);
                transform.parent.localScale = Vector3.Lerp(transform.parent.localScale, target, Time.deltaTime * speed);
                transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, _targetPos, Time.deltaTime * speed);
                yield return null;
            }
            yield break;
        }
    }
}
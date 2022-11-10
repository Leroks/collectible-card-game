using StormWarfare.Interface;
using StormWarfare.Models;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace StormWarfare.Card
{
    public class EventCard : Card, IEventCard
    {
        public EventCardModel Model;

        [SerializeField] private TMPro.TextMeshPro CommandingPoint;
        [SerializeField] float _moveAnimationSecond;
        [SerializeField] float _hoverAnimationSecond;
        [SerializeField] SpriteRenderer Flag;
        [SerializeField] SpriteRenderer BackFaction;
        [SerializeField] SpriteRenderer HoverShadow;

        Vector3 _defaultPosition, _targetPosition;
        Quaternion _defaultRot;
        public override void DefaultPositionChanged(Vector3 pos, Quaternion rot)
        {
            _defaultRot = rot;
            _defaultPosition = _targetPosition = pos;
            _targetPosition.y += 290;
        }
        public EventCard(EventCardModel data)
        {
            Model = data;
        }

        public void InitCard(EventCardModel model)
        {
            Model = model;
            CommandingPoint.text = Model.CommandingPoint.ToString();
            CommandingPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.NeutralColor;
            Description.text = Model.Description.ToString();
            Image.sprite = Resources.Load<Sprite>($"Textures/{Model.Texture}");
            Flag.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/Card/Front/fl-{Model.Faction}");
            BackFaction.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/Cards/Card/Back/{Model.Faction}");
            Name.text = Model.Name.ToString();
            SetRarity(Model.Rarity.ToInt());
        }
        public void InitCard(CommanderAbilityCardModel model)
        {
            Model = model;
            CommandingPoint.text = model.CommandingPoint.ToString();
            CommandingPoint.color = Enums.NeutralColor;
            Description.text = model.Description.ToString();
            Image.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/CommanderParts/" + (model.Faction == Enums.Faction.DE ? "Opponent" : "MyPlayer") + "/specialEffect");
            Name.text = model.Name.ToString();
        }
        public override void MoveMulliganCard(Vector3 target, bool showMulliganChoose)
        {
            base.MoveMulliganCard(target, showMulliganChoose);
            if (_moving)
            {
                _moving = false;
                StopCoroutine(_co);
            }
            _sortingGroup.sortingLayerName = UICardLayer;
            _co = StartCoroutine(SmoothMulligan(target, showMulliganChoose));
        }
        IEnumerator SmoothMulligan(Vector3 target, bool showMulliganChoose)
        {
            float startTime = 0.0f;
            Vector3 startPos = transform.localPosition;
            Vector3 startScale = transform.localScale;
            HoverShadow.gameObject.SetActive(false);
            while (startTime < _hoverAnimationSecond)
            {
                transform.localScale = Vector3.Lerp(startScale, _hoverScale, startTime / _moveAnimationSecond);
                transform.localPosition = Vector3.Lerp(startPos, target, startTime / _moveAnimationSecond);
                startTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (showMulliganChoose)
                MulliganChoose.SetActive(true);
            transform.localScale = _hoverScale;
            transform.localPosition = target;
            yield break;
        }
        public override void MoveCard(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false, bool slowed = true)
        {
            base.MoveCard(targetPos, targetRot, setDefaultPos);
            if (_moving)
            {
                _moving = false;
                StopCoroutine(_co);
            }
            _co = StartCoroutine(SmoothTranslation(targetPos, targetRot, setDefaultPos, scaleUp, false, slowed));
        }
        public override void MoveCardEase(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false)
        {
            base.MoveCardEase(targetPos, targetRot, setDefaultPos, scaleUp);
            if (_moving)
            {
                _moving = false;
                StopCoroutine(_co);
            }
            _co = StartCoroutine(SmoothTranslationEase(targetPos, targetRot, setDefaultPos, scaleUp));
        }

        public override void MoveAICard(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false, bool flip = false)
        {
            base.MoveAICard(targetPos, targetRot, setDefaultPos, scaleUp, flip);
            if (_moving)
            {
                _moving = false;
                StopCoroutine(_co);
            }
            _co = StartCoroutine(SmoothTranslation(targetPos, targetRot, setDefaultPos, scaleUp, flip));
        }
        public override void HoverCard()
        {
            base.HoverCard();
            if (_moving || _defaultPosition == Vector3.zero) return;
            if (_co != null) StopCoroutine(_co);
            _sortingGroup.sortingLayerName = MouseDragLayer;
            _co = StartCoroutine(ScaleUp());
        }

        public override void NotHoverCard()
        {
            base.NotHoverCard();
            if (_moving || _defaultPosition == Vector3.zero) return;
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(ScaleDown());
            _sortingGroup.sortingLayerName = DefaultLayer;

        }

        public static float EaseIn(float t)
        {
            return t * t * t * t;
        }

        public IEnumerator SmoothTranslationEase(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false)
        {
            float startTime = 0.0f;
            Vector3 startPos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            Vector3 startScale = Vector3.one;
            Vector3 endScale = startScale;
            _moving = true;
            if (scaleUp)
            {
                HoverShadow.gameObject.SetActive(false);
                _sortingGroup.sortingLayerName = MouseDragLayer;
                endScale = _hoverScale;
            }
            while (startTime < _moveAnimationSecond)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetPos, EaseIn(startTime / _moveAnimationSecond));
                transform.localRotation = Quaternion.Lerp(rot, targetRot, EaseIn(startTime / _moveAnimationSecond));
                transform.localScale = Vector3.Lerp(startScale, endScale, EaseIn(startTime / _hoverAnimationSecond));
                startTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localScale = endScale;
            transform.localPosition = targetPos;
            transform.localRotation = targetRot;
            if (setDefaultPos)
                DefaultPositionChanged(targetPos, targetRot);
            if(!scaleUp)
                _sortingGroup.sortingLayerName = DefaultLayer;
            _moving = false;
            HoverShadow.gameObject.SetActive(true);
        }

        public IEnumerator SmoothTranslation(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false, bool flip = false, bool slowed = false)
        {
            float startTime = 0.0f;
            Vector3 startPos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            Vector3 startScale = transform.localScale;
            if (flip)
            {
                var tempRot = targetRot.eulerAngles;
                tempRot.y = -180;
                targetRot.eulerAngles = tempRot;
            }
            bool flipped = false;
            Vector3 endScale;
            _moving = true;
            if (scaleUp)
            {
                HoverShadow.gameObject.SetActive(false);
                _sortingGroup.sortingLayerName = MouseDragLayer;
                endScale = _hoverScale;
            }
            else
                endScale = Vector3.one;
            float moveSpeed = _moveAnimationSecond;
            float hoverSpeed = _hoverAnimationSecond;
            if (slowed)
            {
                moveSpeed *= 1.5f;
                hoverSpeed *= 1.5f;
            }
            while (startTime < moveSpeed)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetPos, startTime / moveSpeed);
                transform.localRotation = Quaternion.Lerp(rot, targetRot, startTime / moveSpeed);
                transform.localScale = Vector3.Lerp(startScale, endScale, startTime / hoverSpeed);
                startTime += Time.deltaTime;
                if((!flipped || Mathf.Abs(transform.eulerAngles.x - 270) < 5f) && flip)
                {
                    flipped = true;
                    var tempFront = transform.GetChild(0).gameObject;
                    tempFront.SetActive(true);
                    tempFront.transform.localRotation = targetRot;
                    transform.GetChild(1).gameObject.SetActive(false);
                }
                yield return new WaitForEndOfFrame();
            }
            transform.localRotation = targetRot;
            transform.localScale = endScale;
            transform.localPosition = targetPos;
            if (setDefaultPos)
                DefaultPositionChanged(targetPos, targetRot);
            if (!scaleUp)
                _sortingGroup.sortingLayerName = DefaultLayer;
            if (PlaySound)
            {
                AudioManager.PlaySound("Cards/Deploy-" + Model.Sound);
                PlaySound = false;
            }

            _moving = false;
            HoverShadow.gameObject.SetActive(true);
        }

        IEnumerator ScaleUp()
        {
            float startTime = 0.0f;
            HoverShadow.gameObject.SetActive(false);
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;
            Vector3 startScale = transform.localScale;
            while (startTime < _hoverAnimationSecond)
            {
                transform.localRotation = Quaternion.Lerp(startRot, _rotZero, startTime / _hoverAnimationSecond);
                transform.localScale = Vector3.Lerp(startScale, _hoverScale, startTime / _hoverAnimationSecond);
                transform.localPosition = Vector3.Lerp(startPos, _targetPosition, startTime / _hoverAnimationSecond);
                startTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localRotation = _rotZero;
            transform.localScale = _hoverScale;
            transform.localPosition = _targetPosition;
            yield break;
        }
        IEnumerator ScaleDown()
        {
            float startTime = 0.0f;
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;
            Vector3 startScale = transform.localScale;
            while (startTime < _hoverAnimationSecond)
            {
                transform.localRotation = Quaternion.Lerp(startRot, _defaultRot, startTime / _hoverAnimationSecond);
                transform.localScale = Vector3.Lerp(startScale, Vector3.one, startTime / _hoverAnimationSecond);
                transform.localPosition = Vector3.Lerp(startPos, _defaultPosition, startTime / _hoverAnimationSecond);
                startTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localRotation = _defaultRot;
            transform.localScale = Vector3.one;
            transform.localPosition = _defaultPosition;
            HoverShadow.gameObject.SetActive(true);
            yield break;
        }
    }
}
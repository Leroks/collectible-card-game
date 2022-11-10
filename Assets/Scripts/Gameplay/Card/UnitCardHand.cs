using StormWarfare.Interface;
using StormWarfare.Models;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace StormWarfare.Card
{
    public class UnitCardHand : Card    {
        public UnitCardModel Model;

        [SerializeField] TMPro.TextMeshPro AttackPoint;
        [SerializeField] TMPro.TextMeshPro HealthPoint;
        [SerializeField] TMPro.TextMeshPro CommandingPoint;
        [SerializeField] SpriteRenderer Flag;
        [SerializeField] SpriteRenderer UnitType;
        [SerializeField] SpriteRenderer BackFaction;
        [SerializeField] float _moveAnimationSecond;
        [SerializeField] float _hoverAnimationSecond;
        [SerializeField] SpriteRenderer HoverShadow;

        Vector3 _defaultPosition;
        Quaternion _defaultRot;
        public override void DefaultPositionChanged(Vector3 pos, Quaternion rot)
        {
            _defaultRot = rot;
            _defaultPosition = TargetPosition2 = pos;
            TargetPosition2.y += 290;
        }
        public void InitCard(UnitCardModel model)
        {
            Model = model;
            AttackPoint.text = Model.AttackPoint.ToString();
            HealthPoint.text = Model.HealthPoint.ToString();
            CommandingPoint.text = Model.CommandingPoint.ToString();
            Description.text = Model.Description.ToString();
            Image.sprite = Resources.Load<Sprite>($"Textures/{Model.Texture}");
            Flag.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/Cards/Card/Front/fl-{Model.Faction}");
            UnitType.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/Cards/Card/Front/ico-{Model.ClassType}");
            BackFaction.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/Cards/Card/Back/{Model.Faction}");
            Name.text = Model.Name.ToString();
            SetRarity(Model.Rarity.ToInt());
            UpdateCardState();
        }

        void UpdateCardState()
        {
            if((Model.CardState & Enums.CardState.Neutral) == Enums.CardState.Neutral)
                CommandingPoint.GetComponent<TMPro.TextMeshPro>().color = AttackPoint.GetComponent<TMPro.TextMeshPro>().color = HealthPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.NeutralColor;

            if ((Model.CardState & Enums.CardState.AttackBuffed) == Enums.CardState.AttackBuffed)
                AttackPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.BuffedColor;

            if ((Model.CardState & Enums.CardState.AttackNerfed) == Enums.CardState.AttackNerfed)
                AttackPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.NerfedColor;

            if ((Model.CardState & Enums.CardState.HealthBuffed) == Enums.CardState.HealthBuffed)
                HealthPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.BuffedColor;

            if ((Model.CardState & Enums.CardState.HealthNerfed) == Enums.CardState.HealthNerfed)
                HealthPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.NerfedColor;
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
            HoverShadow.gameObject.SetActive(false);
            Vector3 startPos = transform.localPosition;
            Vector3 startScale = transform.localScale;
            while (startTime < _hoverAnimationSecond)
            {
                transform.localScale = Vector3.Lerp(startScale, _hoverScale, startTime / _moveAnimationSecond);
                transform.localPosition = Vector3.Lerp(startPos, target, startTime / _moveAnimationSecond);
                startTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if(showMulliganChoose)
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
            _co = StartCoroutine(SmoothTranslation(targetPos, targetRot, setDefaultPos, scaleUp, slowed));
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

        public IEnumerator SmoothTranslation(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false, bool slowed = false)
        {
            float startTime = 0.0f;
            Vector3 startPos = transform.localPosition;
            Quaternion rot = transform.localRotation;
            Vector3 startScale = transform.localScale;
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
                transform.localPosition = Vector3.Lerp(startPos, TargetPosition2, startTime / _hoverAnimationSecond);
                startTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            transform.localRotation = _rotZero;
            transform.localScale = _hoverScale;
            transform.localPosition = TargetPosition2;
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
using System.Collections;
using System.Collections.Generic;
using StormWarfare.Gameplay;
using StormWarfare.Model;
using StormWarfare.Models;
using UnityEngine;
using UnityEngine.Rendering;

namespace StormWarfare.Card
{
    public class Card : MonoBehaviour
    {

        public string DefaultLayer;
        public string MouseDragLayer;
        public string UICardLayer;
        [SerializeField] protected Transform _rarity;
        [SerializeField] protected TMPro.TextMeshPro Name;
        [SerializeField] protected TMPro.TextMeshPro Description;
        [SerializeField] protected SpriteRenderer Image;
        [SerializeField] internal GameObject MulliganChoose;
        [SerializeField] internal GameObject NotChoose;
        [SerializeField] internal GameObject CanIPlayThisCardLight;
        public bool PlaySound;
        public int Index;
        [SerializeField]
        protected Vector3 _hoverScale;
        protected Quaternion _rotZero { get { return new Quaternion { x = 0, y = 0, z = 0, w = 1 }; } }

        protected Coroutine _co;
        protected SortingGroup _sortingGroup;
        protected bool _moving;

        public bool IsRetreat;
        internal bool CanIPlayThisCard;
        public Vector3 TargetPosition;
        public Vector3 TargetPosition2;

        public ApplySpecialEffectStruct ApplySpecialEffectData = new ApplySpecialEffectStruct() { effectData = new List<BaseSpecialEffect>()};

        public struct ApplySpecialEffectStruct
        {
            public List<BaseSpecialEffect> effectData;
            public bool IsDealDamage => effectData.Exists((e) => e is DealDamage);
        }
        public virtual void DefaultPositionChanged(Vector3 pos, Quaternion rot)
        {

        }

        public virtual void MoveCard(Vector3 target, float speed)
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
        }

        public virtual void MoveMulliganCard(Vector3 target, bool showMulliganChoose)
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
        }
        public virtual void MoveAICard(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false, bool flip = false)
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
        }

        public virtual void MoveCard(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false, bool slowed = false)
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
        }
        public virtual void MoveCardEase(Vector3 targetPos, Quaternion targetRot, bool setDefaultPos = false, bool scaleUp = false)
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
            AudioManager.PlaySound("CardPick");
        }
        void Awake()
        {
            _sortingGroup = transform.GetComponent<SortingGroup>();
        }

        public virtual void SetRarity(int level)
        {
            for (int i = 0; i < level + 1; i++)
            {
                _rarity.GetChild(i).gameObject.SetActive(true);
            }
        }
        public virtual void HoverCard()
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
        }
        public virtual void NotHoverCard()
        {
            if (!gameObject.activeInHierarchy)
                gameObject.SetActive(true);
        }
        public void StopCurrentCoRoutine()
        {
            if (_co != null) StopCoroutine(_co);
        }
    }
}
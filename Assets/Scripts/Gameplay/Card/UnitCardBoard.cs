using System.Collections;
using System.Collections.Generic;
using StormWarfare.Gameplay;
using StormWarfare.Interface;
using StormWarfare.Models;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace StormWarfare.Card
{
    public class UnitCardBoard : Card, IUnitCard
    {
        public UnitCardModel Model;

        [SerializeField] private TMPro.TextMeshPro AttackPoint;
        [SerializeField] private TMPro.TextMeshPro HealthPoint;
        [SerializeField] SpriteRenderer UnitType;
        [SerializeField] UnitCardHand _hoverUnitCard;
        public GameObject DamagePoint;
        public SpriteRenderer Skull;

        public bool IsKilled;
        public ParticleSystem CardPlacedToBattleGround, CardDamage, CardDeathOne,
        CardDeathTwo, CardDeathThree, CardAttackDowngrade, CardHealthDowngrade, WeopenOne, WeopenTwo;
        public bool FirstTime;
        public GameObject Hover;
        Command damage, death, healthDowngrade, attackDowngrade, weopen;
        CardContainer _cardContainer;
        void OnMouseEnter()
        {
            if (GameManager.Instance.BoardController.AttackCursor.IsAttackShowing || GameManager.Instance.BoardController.Player0.BaseContainer.DraggingCard != null) return;
            _sortingGroup.sortingLayerName = MouseDragLayer;
            if (Index > 4)
            {
                var temp = Hover.transform.GetChild(0).transform.localPosition;
                if(temp.x > 0)
                {
                    temp.x *= -1;
                    Hover.transform.GetChild(0).transform.localPosition = temp;
                }
            }
            Hover.SetActive(true);

        }

        void OnMouseExit()
        {
            if (GameManager.Instance.BoardController.AttackCursor.IsAttackShowing) return;
            _sortingGroup.sortingLayerName = DefaultLayer;
            Hover.SetActive(false);
        }

        public void InitCard(UnitCardModel model)
        {
            Model = model;
            AttackPoint.text = Model.AttackPoint.ToString();
            HealthPoint.text = Model.HealthPoint.ToString();
            Image.sprite = Resources.Load<Sprite>($"Textures/{Model.Texture}");
            UnitType.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/Cards/Card/Front/ico-{Model.ClassType}");
            var tempPos = Skull.transform.localPosition;
            if(model.Faction == Enums.Faction.DE)
            {
                tempPos.y = 0;
                var temp = new Vector3(1.4f, 1.4f, 1);
                Skull.transform.localScale = temp;
                Skull.transform.localPosition = tempPos;
            }
            damage = new CardDamage(CardDamage);
            death = new CardDeath(new List<ParticleSystem> { CardDeathOne, CardDeathTwo, CardDeathThree });
            healthDowngrade = new HealthDowngrade(CardHealthDowngrade);
            attackDowngrade = new AttackDowngrade(CardAttackDowngrade);
            weopen = new Weopen(new List<ParticleSystem> { WeopenOne, WeopenTwo });
            _hoverUnitCard.InitCard(model);
            UpdateCardState();
            _cardContainer = transform.parent.GetComponent<CardContainer>();
        }

        public bool UpdateCardData(UnitCardModel model)
        {
            AttackPoint.text = model.AttackPoint.ToString();
            HealthPoint.text = model.HealthPoint.ToString();
            CanIPlayThisCardLight.SetActive(model.CanIPlay && GameManager.Instance.BoardController.BoardModel.IsMyTurn && !_cardContainer.IsOpponent);
            UpdateCardState();
            if (model.HealthPoint <= 0)
            {
                IsKilled = true;
                ExecuteAnimation("death");
                StartCoroutine(DestroyCard());
                return true;
            }
            return false;
        }

        private IEnumerator DestroyCard()
        {
            yield return new WaitForSeconds(.5f);
            AudioManager.PlaySound("CardDeath");
            yield return new WaitForSeconds(1f);
            Destroy(transform.gameObject);
        }

        void UpdateCardState()
        {
            if ((Model.CardState & Enums.CardState.Neutral) == Enums.CardState.Neutral)
                AttackPoint.GetComponent<TMPro.TextMeshPro>().color = HealthPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.NeutralColor;

            if ((Model.CardState & Enums.CardState.AttackBuffed) == Enums.CardState.AttackBuffed)
                AttackPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.BuffedColor;

            if ((Model.CardState & Enums.CardState.AttackNerfed) == Enums.CardState.AttackNerfed)
                AttackPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.NerfedColor;

            if ((Model.CardState & Enums.CardState.HealthBuffed) == Enums.CardState.HealthBuffed)
                HealthPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.BuffedColor;

            if ((Model.CardState & Enums.CardState.HealthNerfed) == Enums.CardState.HealthNerfed)
                HealthPoint.GetComponent<TMPro.TextMeshPro>().color = Enums.NerfedColor;
        }
        public override void MoveCard(Vector3 target, float speed)
        {
            base.MoveCard(target, speed);
            if (_moving)
            {
                _moving = false;
                StopCoroutine(_co);
            }
            TargetPosition = target;
            _co = StartCoroutine (SmoothTranslation (target, speed));
        }
        IEnumerator SmoothTranslation(Vector3 targetPos, float seconds)
        {
            _moving = true;
            float startTime = 0.0f;
            Vector3 startPos = transform.localPosition;
            while (startTime < seconds)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetPos, startTime / seconds);
                startTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (FirstTime)
            {
                CardPlacedToBattleGround.transform.position = targetPos;
                CardPlacedToBattleGround.Play();
                AudioManager.PlaySound("CardDeploy");
                AudioManager.PlaySound("Cards/Deploy-"+Model.Sound);
                FirstTime = false;
            }
            transform.localPosition = targetPos;
            _moving = false;
            _sortingGroup.sortingLayerName = DefaultLayer;
            if (ApplySpecialEffectData.effectData.Count > 0)
            {
                GameManager.Instance.BoardController.AttackCursor.SetPositionForSpecialEffects(targetPos, this, ApplySpecialEffectData.effectData);
                Hover.SetActive(false);
            }
            yield break;
        }
        public void AttackToCard(int cardUniqueId) { }
        public void AttackToCommander() { }
        public void UseAbility() { }
        public void PlaceToBattleField(int positionalIndex) { }

        // TODO: replace with enum
        public void ExecuteAnimation(string anim)
        {
            switch (anim)
            {
                case "damage":
                    damage.Execute();
                    healthDowngrade.Execute();
                    attackDowngrade.Execute();
                    break;
                case "death":
                    death.Execute();
                    break;
                case "weopen":
                    weopen.Execute();
                    break;
                default:
                    break;
            }
        }
    }
}
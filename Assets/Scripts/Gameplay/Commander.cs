using System.Collections;
using System.Collections.Generic;
using StormWarfare.Card;
using StormWarfare.Models;
using UnityEngine;

namespace StormWarfare.Gameplay
{
    public class Commander : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshPro HealthPoint;
        [SerializeField] private TMPro.TextMeshPro CommandingPoint;
        [SerializeField] private TMPro.TextMeshPro WeaponAmmunation;
        [SerializeField] private TMPro.TextMeshPro WeaponAttackPoint;
        [SerializeField] private TMPro.TextMeshPro SECommandingPoint;
        [SerializeField] SpriteRenderer Image;
        [SerializeField] SpriteRenderer WeaponImage;
        [SerializeField] SpriteRenderer SpecialEffectImage;
        [SerializeField] CommandingPointController CommandingPointsController;

        public WeaponCard WeaponCardHover;
        public EventCard SpecialAbilityHover;

        public int MaxCommandingPoint = 10;
        public int MaxHealthPoint = 30;

        public CardContainer CardContainer;
        public BaseContainer BaseContainer;
        public CommanderModel Model;
        public Transform SpawnPoint;
        public ParticleSystem BaseDamageOne, BaseDamageTwo, BaseDamageThree, BaseDamageFour;
        public GameObject Indicator;
        Command baseDamage;
        private int baseDamageCounter = 0;
        public bool DidIUsedWeaponSprite;
        public bool DidIUsedSpecialAbilitySprite;
        [SerializeField] SpriteSheet WeaponSpriteSheet0;
        [SerializeField] SpriteSheet WeaponSpriteSheet1;
        [SerializeField] SpriteSheet WeaponSpriteSheet2;
        [SerializeField] SpriteSheet SpecialAbiliySpriteSheet0;
        [SerializeField] SpriteSheet SpecialAbiliySpriteSheet1;
        public bool HasEnoughCPToUseSpecialEffect()
        {
            return Model.CommandingPoint >= Model.CommanderSpecialEffectCard.CommandingPoint;
        }

        /// <summary>
        /// oyuncunun gorselini modeldeki veriye gore gunceller
        /// </summary>
        public void Refresh()
        {

        }
        public void SpecialAbilityCover(bool open)
        {
            SpecialAbiliySpriteSheet0.Reverse = SpecialAbiliySpriteSheet1.Reverse = open;
            SpecialAbiliySpriteSheet0.Play = SpecialAbiliySpriteSheet1.Play = true;
        }

        public void WeaponCover(bool open)
        {
            WeaponSpriteSheet0.Reverse = WeaponSpriteSheet1.Reverse = WeaponSpriteSheet2.Reverse = open;
            WeaponSpriteSheet0.Play = WeaponSpriteSheet1.Play = WeaponSpriteSheet2.Play = true;
        }

        public void InitPlayer(CommanderModel model)
        {
            Model = model;
            var isOpponent = Model.CommanderFaction == Enums.Faction.DE;
            CardContainer.IsOpponent = BaseContainer.IsOpponent = isOpponent;
            Image.sprite = Resources.Load<Sprite>($"Textures/{Model.Texture}");
            WeaponImage.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/CommanderParts/" + (isOpponent ? "Opponent" : "MyPlayer") + "/weapon");
            SpecialEffectImage.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/CommanderParts/" + (isOpponent ? "Opponent" : "MyPlayer") + "/specialEffect");
            baseDamage = new BaseDamage(particleSystems: new List<ParticleSystem> { BaseDamageOne, BaseDamageTwo, BaseDamageThree, BaseDamageFour });
            UpdatePlayerData(model);
            WeaponCardHover.InitCard(Model.PlayerWeapon);
            SpecialAbilityHover.InitCard(Model.CommanderSpecialEffectCard);
            CommandingPointsController.CommandingPoint = model.CommandingPoint;
        }

        public bool UpdatePlayerData(CommanderModel model)
        {
            if (HealthPoint.text.ToInt() > Model.HealthPoint)
            {
                AudioManager.PlaySound("StormWarfare-World War 2-Commander Health-Patlama");
                StartCoroutine(BaseDamageDelay());
            }
            HealthPoint.text = Model.HealthPoint.ToString();
            if (Model.HealthPoint > model.OriginalHealthPoint)
                HealthPoint.color = Enums.BuffedColor;
            if (Model.HealthPoint == model.OriginalHealthPoint)
                HealthPoint.color = Color.white;
            if (Model.HealthPoint < model.OriginalHealthPoint)
                HealthPoint.color = Enums.NerfedColor;
            CommandingPoint.text = ((Model.CommandingPoint > MaxCommandingPoint ? MaxCommandingPoint : Model.CommandingPoint) + "/" + MaxCommandingPoint).ToString();
            WeaponAmmunation.text = Model.PlayerWeapon.AmmunationPoint.ToString();
            WeaponAttackPoint.text = Model.PlayerWeapon.AttackPoint.ToString();
            SECommandingPoint.text = Model.CommanderSpecialEffectCard.CommandingPoint.ToString();
            CommandingPointsController.CommandingPoint = model.CommandingPoint;
            var commanderBaseWall = gameObject.transform.Find("CommanderBaseWall");
            for (int i = 0; i < commanderBaseWall.childCount; i++)
            {
                var child = commanderBaseWall.GetChild(i);
                child.gameObject.SetActive(i == GetHealthPercentIndex(model.HealthPoint, MaxHealthPoint));

                switch (GetHealthPercentIndex(model.HealthPoint, MaxHealthPoint))
                {
                    case 3:

                        if (baseDamageCounter == 0)
                        {
                            StartCoroutine(BaseDamageDelay());
                            baseDamageCounter++;
                        }
                        break;
                    case 2:
                        if (baseDamageCounter == 1)
                        {
                            StartCoroutine(BaseDamageDelay());
                            baseDamageCounter++;
                        }
                        break;
                    case 1:
                        if (baseDamageCounter == 2)
                        {
                            StartCoroutine(BaseDamageDelay());
                            baseDamageCounter++;
                        }
                        break;
                    case 0:
                        if (baseDamageCounter == 3)
                        {
                            StartCoroutine(BaseDamageDelay());
                            baseDamageCounter++;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (model.HealthPoint <= 0)
            {
                return true;
            }
            return false;
        }

        int GetHealthPercentIndex(int currHealth, int maxHealth, int partCount = 5)
        {
            var res = Mathf.Clamp(Mathf.FloorToInt(currHealth * partCount / maxHealth), 0, partCount - 1);
            return res;
        }

        IEnumerator BaseDamageDelay()
        {
            yield return new WaitForSeconds(0.5f);
            baseDamage.Execute();
        }
    }
}
using StormWarfare.Interface;
using StormWarfare.Models;
using UnityEngine;

namespace StormWarfare.Card
{
    public class WeaponCard : Card
    {
        public WeaponCardModel Model;

        [SerializeField] private TMPro.TextMeshPro CommandingPoint;
        [SerializeField] private TMPro.TextMeshPro AmmunationPoint;
        [SerializeField] private TMPro.TextMeshPro AttackPoint;

        public WeaponCard(WeaponCardModel data) => Model = data;

        public void InitCard(WeaponCardModel model)
        {
            Model = model;
            var isOpponent = Model.Faction == Enums.Faction.DE;
            Image.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/CommanderParts/" + (isOpponent ? "Opponent" : "MyPlayer") + "/weapon");
            CommandingPoint.text = Model.CommandingPoint.ToString();
            AmmunationPoint.text = Model.AmmunationPoint.ToString();
            AttackPoint.text = Model.AttackPoint.ToString();
            Description.text = Model.Description.ToString();
            Name.text = Model.Name.ToString();
            AttackPoint.color = AmmunationPoint.color = CommandingPoint.color = Enums.NeutralColor;
        }

        public void UpdateCardData(WeaponCardModel model)
        {
            Model = model;
            AmmunationPoint.text = Model.AmmunationPoint.ToString();
            AttackPoint.text = Model.AttackPoint.ToString();
        }
    }
}
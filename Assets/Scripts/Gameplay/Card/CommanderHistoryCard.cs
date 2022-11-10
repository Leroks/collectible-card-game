using StormWarfare.Interface;
using StormWarfare.Models;
using UnityEngine;

namespace StormWarfare.Card
{
    public class CommanderHistoryCard : Card
    {
        public CommanderModel Model;

        [SerializeField] private TMPro.TextMeshPro HealthPoint;

        public CommanderHistoryCard(CommanderModel data) => Model = data;

        public void InitCard(CommanderModel model)
        {
            Model = model;
            HealthPoint.text = Model.HealthPoint.ToString();
            Image.sprite = Resources.Load<Sprite>($"Textures/{Model.Texture}");
            Description.text = Model.Description.ToString();
            Name.text = Model.Name.ToString();
        }

        public void UpdateCardData(CommanderModel model)
        {
            Model = model;
            HealthPoint.text = model.HealthPoint.ToString();
        }
    }
}
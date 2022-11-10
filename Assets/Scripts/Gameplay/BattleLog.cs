using StormWarfare.Model;
using StormWarfare.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormWarfare.Gameplay
{
    public class BattleLog : MonoBehaviour
    {
        public List<HistoryModel> Log = new List<HistoryModel>();

        //To assign container in the scene
        [SerializeField]
        HistoryCardContainer historyCardContainer;

        //To add history cards one by one
        private int index = 0;

        //gecici olarak eklendi oyuncularin desteleri hazirlandiktan sonra olusan modellerle 
        //gecici veri ekler

        // public void CreateTempHistoryData()
        // {
        //     var sourceDeck = GameManager.Instance.BoardController.BoardModel.MyDeck; 
        //     var targetDeck = GameManager.Instance.BoardController.BoardModel.OppDeck;

        //     for (int i = 0; i < 3; i++)
        //     {
        //         LogToHistory(sourceDeck[i], null, null);
        //         LogToHistory(targetDeck[i], null, null);
        //         LogToHistory(sourceDeck[i], targetDeck[i], new Dynamic() { ["attackValue"] = 5 });
        //     }
        // }

        /// <summary>
        /// bu method icerisinde yukaridaki Log listesini donup icindeki historymodel verilerine gore prefablari yaratilcak
        /// gelebilcek degerler icin historymodel icerisine bakilabilir
        /// </summary>

        public void CreateHistoryPrefabs()
        {
            historyCardContainer.AddHistory(Log[index].sourceCard, Log[index].effectedCards, Log[index].deltaValues);
            index++;
        }

        public void LogEventCardEffect(EventCardModel sCard)
        {
            LogToHistory(sCard, sCard.EffectedCards, new Dynamic());
        }

        public void LogAttackToCard(UnitCardModel sCard, UnitCardModel tCard)
        {
            LogToHistory(sCard, new List<EffectStruct>() { new EffectStruct() { card = tCard } }, new Dynamic() { ["attackValue"] = sCard.AttackPoint });
        }

        public void LogAttackToCard(WeaponCardModel sCard, UnitCardModel tCard)
        {
            LogToHistory(sCard, new List<EffectStruct>() { new EffectStruct() { card = tCard } }, new Dynamic() { ["attackValue"] = sCard.AttackPoint });
        }

        public void LogAttackToCommander(UnitCardModel sCard, CommanderModel tCommander)
        {
            LogToHistory(sCard, new List<EffectStruct>() { new EffectStruct() { card = tCommander } }, new Dynamic() { ["attackValue"] = sCard.AttackPoint });
        }
        public void LogAttackToCommander(WeaponCardModel sCard, CommanderModel tCommander)
        {
            LogToHistory(sCard, new List<EffectStruct>() { new EffectStruct() { card = tCommander } }, new Dynamic() { ["attackValue"] = sCard.AttackPoint });
        }


        public void LogPlaceCardToPlayGround(UnitCardModel sCard)
        {
            if (sCard.SpecialEffects.Count > 0)
            {
                // TEMP DEBUG 
                LogToHistory(sCard, sCard.EffectedCards, new Dynamic());
                //LogToHistory(sCard, null, null);

            }
            else
            {
                LogToHistory(sCard, null, null);

            }
        }

        public void LogToHistory(BaseCardModel s, List<EffectStruct> t, Dynamic delta)
        {
            // if(s is EventCardModel){
            //     var eventCard = (s as EventCardModel);
            // } else if(s is UnitCardModel){
            //     var unitCard = (s as UnitCardModel);
            //var baseCard = (s as BaseCardModel);
            // }
            var data = new HistoryModel() { sourceCard = s, effectedCards = t, deltaValues = delta };
            Log.Add(data);
            CreateHistoryPrefabs();
        }
    }
}
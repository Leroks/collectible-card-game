using System;
using System.Collections.Generic;
using UnityEngine;

namespace StormWarfare.Models
{
    /// <summary>
    /// Masanin statelerini tutan sinif
    /// Current turn, canlar vs guncel veri
    /// </summary>
    public class BoardModel
    {
        public enum BoardStates
        {
            Mulligan,
            Playing,
            Finish
        }

        public BoardStates BoardState;

        public bool IsPlaying => BoardState == BoardStates.Playing;

        public List<BaseCardModel> MyDeck = new();
        public List<BaseCardModel> OppDeck = new();

        public List<BaseCardModel> MyHand = new();
        public List<BaseCardModel> OppHand = new();

        public EndGameModel EndGameData;


        //todo: nullreference duzelt ileride
        public BaseCardModel GetNextCard(int player = 0)
        {
            var deck = player == (int)Enums.Faction.US ? MyDeck : OppDeck;
            if (deck.Count > 0) return deck[^1];

            return null;
        }

        public List<BaseCardModel>[] boardCards = new List<BaseCardModel>[2] { new List<BaseCardModel>(), new List<BaseCardModel>() };

        /// <summary>
        /// siranin su anda kimde oldugu
        /// </summary>
        public int CurrentTurn;
        public int OtherTurn => (CurrentTurn + 1) % 2;
        public int RoundTurnTimer = 45;
        public int MulliganTurnTimer = 20;

        public static int TotalRoundCount = 2;
        public int GetNextCommandingPoint => TotalRoundCount / 2;

        //todo: biz demoda amerikayi tarafi olucagimiz icin boyle kontrol ettim degismeli!
        public bool IsMyTurn => CurrentTurn == (int)Enums.Faction.US;

        public CommanderModel[] Players = new CommanderModel[2];

        public int[] PunishCounter = new int[2] {1 , 1};

        /*
        public static int MySeatNo;
        public static int CurrentTurn;
        public static int TurnTimer;
        public static int BoardCardCount;
        public static int[] MyCards;
        public static int[,] TableCards;
        
        //debug icin eklendi
        public static int[] OppCards;

        public static Dynamic[] Players => Game.ToList<Dynamic>("players").ToArray();
        public static Dynamic Game = new Dynamic();

        public Player OppPlayer;
        public static Player MyPlayer;
        */

        /// <summary>
        /// oyuncu masaya oturdugunda tam paketi aldiginda masa bilgilerini bu method icerisinde tek seferligine
        /// guncelle sonrasinda bu paket gelmicek setup methodu sonraki masa guncellemeleri UpdateGame() methodundan
        /// ilerlicek
        /// </summary>
        public static void RefreshBoard(Dynamic data)
        {
        }

        /// <summary>
        /// Sunucudan gelen pakete gore modeli gunceller
        /// </summary>
        /// <param name="data"></param>
        public static void UpdateGame(Dynamic data)
        {
            //scenedeki refresh methodunu cagir modeli guncelledikten sonra
            /*
            switch ((BoardStates) data.ToInt("a"))
            {
                case BoardStates.WaitingForPlayer:
                    break;
                case BoardStates.Starting:
                    break;
                case BoardStates.Playing:
                    break;
                case BoardStates.Finish:
                    break;
            }
            var action = (PlayerActions) data.ToInt("a");
            switch (action)
            {
                case PlayerActions.AttackToCard:
                    break;
                case PlayerActions.AttackToOpponent:
                    MyPlayer.HealthPoints -= data.ToInt("av");
                    break;
                case PlayerActions.Pass:
                    break;
                case PlayerActions.PickCard:
                    break;
            }
            */
        }

        /// <summary>
        /// Oyuncu karsi oyuncunun kartina saldirdi lokalimde verileri guncelle ve sunucuya adamin kartina saldirdim
        /// paketi gonder
        /// sunucu oyuncunun kendine paket yollamicak
        /// </summary>
        /// <param name="card"></param>
        public void AttackToCard(int sourceIndex, int targetIndex)
        {
            var attackingCardData = (UnitCardModel)boardCards[CurrentTurn][sourceIndex];
            var defendingCardData = (UnitCardModel)boardCards[OtherTurn][targetIndex];

            attackingCardData.HealthPoint -= defendingCardData.AttackPoint;
            defendingCardData.HealthPoint -= attackingCardData.AttackPoint;

            if (attackingCardData.HealthPoint <= 0) boardCards[CurrentTurn].RemoveAt(sourceIndex);
            if (defendingCardData.HealthPoint <= 0) boardCards[OtherTurn].RemoveAt(targetIndex);
        }

        public void AttackCardToPlayer(int sourceIndex)
        {
            var attackingCardData = (UnitCardModel)boardCards[CurrentTurn][sourceIndex];
            var targetPlayer = Players[OtherTurn];

            targetPlayer.HealthPoint -= attackingCardData.AttackPoint;

            //todo: eger oyuncu olduyse listeden cikarilsin ?
        }

        public void AttackWeaponToPlayer()
        {
            var attackingWeaponData = Players[CurrentTurn].PlayerWeapon;
            var targetPlayer = Players[OtherTurn];

            attackingWeaponData.AmmunationPoint--;
            targetPlayer.HealthPoint -= attackingWeaponData.AttackPoint;

            //todo: eger oyuncu olduyse listeden cikarilsin ?
        }

        public void AttackWeaponToCard(int targetIndex)
        {
            var attackingWeaponData = Players[CurrentTurn].PlayerWeapon;
            var defendingCardData = (UnitCardModel)boardCards[OtherTurn][targetIndex];

            attackingWeaponData.AmmunationPoint--;
            defendingCardData.HealthPoint -= attackingWeaponData.AttackPoint;

            if (defendingCardData.HealthPoint <= 0) boardCards[OtherTurn].RemoveAt(targetIndex);
        }

        /// <summary>
        /// Oyuncu karsidaki oyuncuya dogrudan saldirdi adamin canini guncelle ve sunucuya oyuncuya saldirdi paketini
        /// yolla
        /// sunucu oyuncunun kendine paket yollamicak
        /// </summary>
        // public void AttackToPlayer(Card card)
        // {
        //      OppPlayer.HealthPoints -= card.AttackPoint;
        //      GameServer.GameServer.UpdateGame(new Dynamic()
        //      {
        //          ["a"] = PlayerActions.AttackToOpponent,
        //          ["av"] = card.AttackPoint
        //      });
        // }

        /// <summary>
        /// Oyuncu passladi veya hamle yapmadi turn sirasinda sunucuya pass paketi gonder sirayi obur oyuncuya gecirsin
        /// sunucu her iki oyuncuya da guncel sira ve masa durumunu yollicak
        /// </summary>
        public void Pass()
        {

        }

        /// <summary>
        /// Oyuncu destesinden kart aldi oyuncunun destesini guncelle sonrasinda sunucuya kart cektim paketini yolla
        /// sunucu oyuncunun kendine paket yollamicak
        /// </summary>
        public void PickCard()
        {
            var deck = IsMyTurn ? MyDeck : OppDeck;
            var hand = IsMyTurn ? MyHand : OppHand;
            hand.Add(GetNextCard(CurrentTurn));
            deck.RemoveAt(deck.Count - 1);
        }

        public void PlaceCardToBattleGround(UnitCardModel card, int index)
        {
            var hand = IsMyTurn ? MyHand : OppHand;
            hand.Remove(card);
            boardCards[CurrentTurn].Insert(index, card);
            Players[CurrentTurn].CommandingPoint -= card.CommandingPoint;
        }

        //su anki sirayi diger oyuncuya gecirir.
        public void AdvanceNextPlayer()
        {
            CurrentTurn = (CurrentTurn + 1) % 2;
            TotalRoundCount++;
        }

        public void UpdateCommandingPoint()
        {
            Players[CurrentTurn].CommandingPoint = Math.Min(GetNextCommandingPoint, 10);
        }

        public void RemoveEventCardFromHand(EventCardModel cardData)
        {
            var hand = IsMyTurn ? MyHand : OppHand;
            hand.Remove(cardData);
        }

        public void UseEventCard(EventCardModel cardData)
        {
            Players[CurrentTurn].CommandingPoint -= cardData.CommandingPoint;
        }

        public void AddToDeck(BaseCardModel card, int index, bool isMyDeck = true)
        {
            var deck = isMyDeck ? MyDeck : OppDeck;
            deck.Insert(index, card);
        }

        public List<BaseCardModel> GetMulliganCards(int count = 3, bool isMyDeck = true)
        {
            var deck = isMyDeck ? MyDeck : OppDeck;
            var result = deck.GetRange(0, count);
            deck.RemoveRange(0, count);
            return result;
        }

        //todo: sonradan degistirilmesi lazim 2 tane 1 1 tane 2 cp lik kart vericek sekilde
        public List<BaseCardModel> GetMulliganCardsRandomized(int count = 3, bool isMyDeck = true)
        {
            var deck = isMyDeck ? MyDeck : OppDeck;
            var result = new List<BaseCardModel>();

            var card = GetRandomCardWithCP(1);
            result.Add(card);
            RemoveCardFromDeck(card);

            for (int i = 0; i < 2; i++)
            {
                var card2 = GetRandomCardWithCP(-1);
                result.Add(card2);
                RemoveCardFromDeck(card2);
            }

            return result;
        }


        public BaseCardModel GetRandomCardWithCP(int cp, bool isMyDeck = true)
        {
            Random rand = new();
            var deck = isMyDeck ? MyDeck : OppDeck;
            var cpList = deck.FindAll(c => (c as UnitCardModel)?.CommandingPoint == cp);
            return cpList.Count > 0 ? cpList.Count > 1 ? cpList[rand.Int(0, cpList.Count - 1)] : cpList[0] : deck[rand.Int(0, deck.Count - 1)];
        }

        public void RemoveCardFromDeck(BaseCardModel card, bool isMyDeck = true)
        {
            var deck = isMyDeck ? MyDeck : OppDeck;
            deck.Remove(card);
        }

        public void ShuffleDecks()
        {
            Random rand = new Random();
            if(MyDeck.Count > 2)
                rand.Shuffle(ref MyDeck);
            if(OppDeck.Count > 2)
                rand.Shuffle(ref OppDeck);
        }

        public void ResetCardAttack()
        {
            for (int i = 0; i < boardCards[0].Count; i++)
                (boardCards[0][i] as UnitCardModel).DidIAttackBefore = false;
            for (int i = 0; i < boardCards[1].Count; i++)
                (boardCards[1][i] as UnitCardModel).DidIAttackBefore = false;
            Players[0].PlayerWeapon.DidIUsedBefore = false;
            Players[1].PlayerWeapon.DidIUsedBefore = false;
            Players[0].CommanderSpecialEffectCard.DidIUsedBefore = false;
            Players[1].CommanderSpecialEffectCard.DidIUsedBefore = false;
        }

        public bool PunishOutOfDeckPlayer()
        {
            var deck = IsMyTurn ? MyDeck : OppDeck;
            if (deck.Count <= 0)
            {
                Players[CurrentTurn].HealthPoint -= PunishCounter[CurrentTurn];
                return true;
            }

            return false;
        }
    }
}
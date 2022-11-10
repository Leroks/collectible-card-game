using System.Collections.Generic;
using StormWarfare.Models;

namespace StormWarfare.GameServer
{
    public class GameServer
    {
        public enum PlayerActions
        {
            Pass,
            PickCard,
            AttackToOpponent,
            AttackToCard
        }

        public static int CurrentTurn;
        public static int TurnTimer = 10000;
        private static Board _board;
        private static Player[] _players;

        public static Dynamic State = new Dynamic();

        public static void StartDemoGame()
        {
            var data = new Dynamic();
            _players = new Player[] { new(100, 20, 0), new(80, 20, 0) };

            var players = new List<Dynamic>()
            {
                new()
                {
                    ["hp"] = _players[0].HealthPoints,
                    ["mp"] = _players[0].MagicPoints,
                    ["dp"] = _players[0].DefensePoints,
                },
                new()
                {
                    ["hp"] = _players[1].HealthPoints,
                    ["mp"] = _players[1].MagicPoints,
                    ["dp"] = _players[1].DefensePoints,
                }
            };
            data["game"] = new Dynamic()
            {
                ["players"] = players
            };
            
            data["turnTime"] = TurnTimer;

            State = data;
        }
        
        /// <summary>
        /// demo icin eklendi test icin oyunu baslat her iki oyuncuya da tek seferlik oyuncu bilgilerini ve masa
        /// durumunu gonder burdan sonra bu raw veriyi update ederek ilerliycez
        /// </summary>
        public static void StartGame()
        {
            BoardModel.RefreshBoard(State);
        }

        private static int GetNextPlayer => (CurrentTurn + 1) % 2;

        /// <summary>
        /// a = action
        /// s = state
        /// t = turntime
        /// TODO: DYNAMIC YOLLANMICAK SIMDILIK EKLENDI SONRASINDA OBJEYE CEVIR
        /// </summary>
        public static void UpdateGame(Dynamic data)
        {
            //scenedeki refresh methodunu cagir modeli guncelledikten sonra
            //current turn u guncelle
            //sendtoplayers ya da yollayan oyuncu haric su gelen paketi yolla
            var action = (PlayerActions) data.ToInt("a");
            switch (action)
            {
                case PlayerActions.AttackToCard:
                    break;
                case PlayerActions.AttackToOpponent:
                    PlayerAttackedToOpponent(data.ToInt("av"));
                    break;
                case PlayerActions.Pass:
                    break;
                case PlayerActions.PickCard:
                    break;
            }
        }
        
        /// <summary>
        /// oyunculara veriyi iletir 
        /// </summary>
        /// <param name="data">gonderilcek veri</param>
        /// <param name="excludePlayerId">dahil olmamasini istedigimiz oyuncu genelde hamle yapan oyuncunun kendi</param>
        public static void SendToPlayers(Dynamic data, int excludePlayerId = -1)
        { 
            
        }

        /// <summary>
        /// gelen pakete gore sunucudaki players listesindeki oyuncularin verilerini duzenle
        /// </summary>
        /// <param name="attackValue"></param>
        public static void PlayerAttackedToOpponent(int attackValue)
        {
            _players[GetNextPlayer].HealthPoints -= attackValue;
            //SendToPlayers();
        }

        /// <summary>
        /// oyuncu karsi oyuncunun kartina saldirdi sunucudaki oyun verisini guncelle diger oyunculara
        /// guncel durumu gonder
        /// </summary>
        public static void PlayerAttackedToCard()
        {
            
        }

        /// <summary>
        /// oyuncu timeout a dustu sirayi guncelleyip her iki oyuncuya da guncel masanin hepsini yolla
        /// </summary>
        public static void PlayerPass()
        {

        }

        /// <summary>
        /// oyuncu kart cekti sunucudaki kart sayisini guncelleyip diger oyuncuya guncel durumu ilet
        /// </summary>
        public static void PlayerPickedCard()
        {

        }
    }
}

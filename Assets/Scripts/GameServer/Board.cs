using System.Collections.Generic;

namespace StormWarfare.GameServer
{
    public class Board
    {
        public List<int> BoardCards;
        public int CurrentTurn;
        public List<Player> Players;
        
        public enum BoardStates
        { 
            WaitingForPlayer,
            Starting,
            Playing,
            Finish
        }

        public const int NORMALGAME_TIMER = 19000;
        //const int FIRSTHAND_EXTRA_TIME = 7000;
        public const int NEXT_TURN_TIME = 5000;
        //oyun sonunda animasyon suresi degisikligini duzenlemek icin
        private const int FINISH_ANIMATION_TIME = 0;
        
        public Board(List<int> cards)
        {
            BoardCards = cards;
            CurrentTurn = 0;
        }
    }
}
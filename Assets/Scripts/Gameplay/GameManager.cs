using StormWarfare.Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StormWarfare.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }
            
        public Commander Player0;
        public AIPlayer Player1;
        public BoardController BoardController;
        public BattleLog BattleLog;

        void OnDestroy()
        {
            if (this == _instance)
                _instance = null;
        }
        void Awake()
        {

            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
        }

        public void PlayToPlayground()
        {
            Player1.PlaceCardToBattleGround();
        }
    }
}

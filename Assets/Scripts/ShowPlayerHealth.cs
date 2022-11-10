using UnityEngine;
using TMPro;

namespace StormWarfare.Gameplay
{
    public class ShowPlayerHealth : MonoBehaviour
    {

        private bool once, once1 = true;
        private int healthPlayer0, healthPlayer1;

        [SerializeField]
        TMP_Text healthPlayer0Text, healthPlayer1Text;

        [SerializeField]
        BoardController boardController;

        Commander player0;

        //Player player1;
        AIPlayer player1;

        public int GetPlayerHealth0{
            get{
                return healthPlayer0;
            }
        }
        public int GetPlayerHealth1{
            get{
                return healthPlayer1;
            }
        }
        

        //TO DO// *********  Attach this to change health function to avoid updates
        void Update()
        {
           /* player0 = boardController.Player0;
            player1 = boardController.Player1;
            healthPlayer0 = player0.HealthPoints;
            healthPlayer1 = player1.HealthPoints;
            healthPlayer0Text.text = healthPlayer0.ToString();
            healthPlayer1Text.text = healthPlayer1.ToString();*/
        }
        
    }

}
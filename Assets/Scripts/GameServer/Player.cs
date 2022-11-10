using UnityEngine;

namespace StormWarfare.GameServer
{
    /// <summary>
    /// TODO: bu classa gerek olmayabilir object icerisinde player verileri tutulabilir
    /// </summary>
    public class Player : MonoBehaviour
    {
        public int HealthPoints;
        public int MagicPoints;
        public int DefensePoints;

        public Player(int healthPoints, int magicPoints, int defensePoints)
        {
            HealthPoints = healthPoints;
            MagicPoints = magicPoints;
            DefensePoints = defensePoints;
        }
    }
}

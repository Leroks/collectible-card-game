using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StormWarfare.Gameplay;


public class BoardManager : MonoBehaviour
{
    [SerializeField]
    Renderer boardLowerPlate;
    //Board objects named with health value
    [SerializeField]
    Material board_100;

    [SerializeField]
    Material board_80;
    [SerializeField]
    Material board_60;
    [SerializeField]
    Material board_40;
    [SerializeField]
    Material board_20;
    public ShowPlayerHealth playerHealths;

    //TO DO// ***************** make this section a function not Update
    void Update()
    {
        /*int commanderHealth = playerHealths.GetPlayerHealth0;

        if (commanderHealth >= 80){
            
            boardLowerPlate.material = board_100;
        }
        else if (commanderHealth <= 80 && commanderHealth > 60){
            
            boardLowerPlate.material = board_80;
        }
        else if (commanderHealth <= 60 && commanderHealth > 40){
            
            boardLowerPlate.material = board_60;
            
        }
        else if (commanderHealth <= 40 && commanderHealth > 20){
            
            boardLowerPlate.material = board_40;
            
        }
        else if (commanderHealth <= 20){
           boardLowerPlate.material = board_20;
          
        }*/
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOnBoard : MonoBehaviour
{

    [SerializeField]
    GameObject _plane;

    public int Index;

    void OnMouseEnter(){
        _plane.SetActive(true);
        //extended.SetActive(true);
        
    }
    void OnMouseExit(){
        _plane.SetActive(false);
        //extended.SetActive(false);
        

    }

    void OnMouseDown()
    {



    }

}

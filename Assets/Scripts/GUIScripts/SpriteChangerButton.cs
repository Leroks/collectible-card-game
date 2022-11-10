using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChangerButton : MonoBehaviour
{
    [SerializeField] GameObject pressed;

    void OnMouseDown(){
        pressed.SetActive(true);
    }
    void OnMouseUp(){
        pressed.SetActive(false);
    }
    
}

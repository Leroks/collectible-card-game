using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOnHand : MonoBehaviour
{

    [SerializeField]
    Vector3 zoom =  new Vector3(3f, 3f, 1f);

    [SerializeField]
    GameObject Fbx;
   
    private Vector3 startPosition;
    private Vector3 startScale;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = Fbx.transform.position;
        startScale = Fbx.transform.localScale; 
    }

    void OnMouseEnter(){
        Fbx.transform.localScale = new Vector3(3f, 3f, 1f);
        Fbx.transform.position = new Vector3(startPosition.x, 0.5f, startPosition.z );


    }

    void OnMouseExit(){
        Fbx.transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z);
        Fbx.transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z);
       

    }
}

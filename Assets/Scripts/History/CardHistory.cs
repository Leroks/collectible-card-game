using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StormWarfare.Card;
using StormWarfare.Models;


public class CardHistory : MonoBehaviour
{
    
    // [SerializeField]
    // GameObject historyPlane;

    // [SerializeField]
    // public List<RawImage> Images;
    // public List<Texture> ImagesStored;

    // [SerializeField]
    // public List<TextMeshProUGUI> Texts;
    // public List<string> TextsStored;

    // [SerializeField]
    // public List<GameObject> historyCards;
    [SerializeField] private HistoryCardContainer historyLocationContainer;

    public SpriteRenderer thisCard;
    //public List<GameObject> effectedCards;

    //int index, index_text, index_cards = 0;
    public SpriteRenderer myFrame;
    public int myIndex = 0;
    public int myStaticIndex;

    void OnMouseEnter(){

        // foreach(GameObject i in historyCards){
        
        //     i.SetActive(false);
        // }
        // historyCards[myStaticIndex].SetActive(true);
        
        historyLocationContainer.LogActivator(myStaticIndex);

    }
    void OnMouseExit(){

        //historyCards[myStaticIndex].SetActive(false);
        historyLocationContainer.LogDisabler(myStaticIndex);
    }

    // public void DeleteMyHistoryLog(){
    //     for(int i=0; i < historyCards[myStaticIndex].transform.childCount-1; i++){
    //         Destroy(historyCards[myStaticIndex].transform.GetChild(2).gameObject);
    //     }
    //     //Debug.Log(historyCards[myStaticIndex].transform.childCount);
        
    // }
}

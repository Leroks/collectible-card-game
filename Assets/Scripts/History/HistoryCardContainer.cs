using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StormWarfare.Models;
using StormWarfare.Model;
using TMPro;
using StormWarfare.Card;
using StormWarfare.Gameplay;
using UnityEngine.Rendering;



public class HistoryCardContainer : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> historyCardList;

    [SerializeField]
    private GameObject iconPlacer;

    [SerializeField]
    private Animator historyMechanism;
    [SerializeField] List<Transform> _spawnPointList;
    [SerializeField] List<Transform> _spawnPointListLarge;
    List<Transform> activeList;

    [SerializeField] Vector3 spawnSize = new Vector3(2, 2, 2);

    //Vector3 iconSetterForDead = new Vector3(-3, 0, 0);
    Vector3 iconSetter = new Vector3(-3, 140, 0);

    private int index = -1;

    public int historyLogSize = -1;

    private bool firstCardOut = false, notEvent = true;

    private Vector3 startPos = new Vector3(-696, 167, -4);

    
    [SerializeField]
    Sprite Buff, Dead, Damaged, BuffPlate;

    //NEW HISTORY MECHANICS
    [SerializeField] private List<Sprite> historyCardBorders;
    [SerializeField] private List<Transform> historyCardSpawnLocation;
    [SerializeField] private List<GameObject> historyLogs;
    [SerializeField] private GameObject LogBackground;

    public bool isPause = false;

    void Start()
    {
        foreach (GameObject a in historyCardList)
        {
            //a.transform.position = startPos;
            a.transform.position = historyCardSpawnLocation[0].position;
        }
        
    }

    public void AddHistory(BaseCardModel sourceCard, List<EffectStruct> effectedCards, Dynamic deltaValues)
    {

        historyLogSize++;

        if(historyLogSize < 14){
            historyMechanism.SetInteger("Part", historyLogSize);
        }
        
        index++;
        if (index > 14)
        {
            index = 0;
        }

        firstCardOut = false;
        foreach (GameObject a in historyCardList)
        {
            
            if (a.activeInHierarchy)
            {
                //a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y, a.transform.position.z +50);
                //a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y - 50, a.transform.position.z);
                //a.transform.position -= a.transform.up * 45;
                if(a.GetComponent<CardHistory>().myIndex == 13){
                    DeleteMyHistoryLog(a.GetComponent<CardHistory>().myStaticIndex);
                    a.SetActive(false);
                    MiddlePointSetter(index, false);
                    //DeleteMyHistoryLog(a.GetComponent<CardHistory>());
                    //SmoothMovement.SmoothTranslation(a, historyCardSpawnLocation[0].position, 1f);
                    a.transform.position = historyCardSpawnLocation[0].position;
                    a.GetComponent<CardHistory>().myIndex = 0;
                    a.GetComponent<CardHistory>().myFrame.sprite = historyCardBorders[0];
                }
                else{
                    //a.transform.position = historyCardSpawnLocation[a.GetComponent<CardHistory>().myIndex + 1].position;
                    StartCoroutine (SmoothMovement.SmoothTranslationHistory (a, historyCardSpawnLocation[a.GetComponent<CardHistory>().myIndex + 1].position, .27f));
                    a.GetComponent<CardHistory>().myIndex ++;
                    a.GetComponent<CardHistory>().myFrame.sprite = historyCardBorders[a.GetComponent<CardHistory>().myIndex + 1];
                    a.GetComponent<CardHistory>().thisCard.sortingOrder++; a.GetComponent<CardHistory>().myFrame.sortingOrder++;
                }
                
            }
            else if (!a.activeInHierarchy && !firstCardOut)
            {
                //a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y, a.transform.position.z +50);
                //a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y - 50, a.transform.position.z);
                //a.transform.position -= a.transform.up * 45;
                if(a.GetComponent<CardHistory>().myIndex == 0){
                    a.transform.position = historyCardSpawnLocation[0].position;
                    a.GetComponent<CardHistory>().myIndex = 0;
                    a.GetComponent<CardHistory>().thisCard.sortingOrder = 3; a.GetComponent<CardHistory>().myFrame.sortingOrder = 4;
                    a.SetActive(true);
                    firstCardOut = true;
                    
                }
            }
        }

        
        setHistoryCards(sourceCard, effectedCards, deltaValues);
    }

    private void setHistoryCards(BaseCardModel sourceCard, List<EffectStruct> effectedCards, Dynamic deltaValues)
    {
        notEvent = true;

        CardHistory thisCard = historyCardList[index].GetComponent<CardHistory>();

        thisCard.thisCard.sprite = Resources.Load<Sprite>($"Textures/{sourceCard.Texture}");

        Transform parentTransform = historyLogs[index].transform;

        var sourceCardModel = sourceCard;

        switch (sourceCardModel)
        {
            case UnitCardModel:
                var unitCard = Instantiate(Resources.Load("Prefabs/2D/UnitCardHand", typeof(UnitCardHand)) as UnitCardHand, _spawnPointList[0].localPosition, Quaternion.identity, parentTransform);
                unitCard.InitCard(sourceCardModel as UnitCardModel);
                unitCard.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
                unitCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";

                if (effectedCards != null)
                {
                    //Middle Arrow
                    MiddlePointSetter(index, true);

                    if (unitCard.Model.HealthPoint <= 0)
                    {
                        GameObject iconPlacerClone;
                        iconPlacerClone = Instantiate(iconPlacer, _spawnPointList[0].localPosition - iconSetter, Quaternion.identity, unitCard.transform) as GameObject;
                        iconPlacerClone.GetComponent<SpriteRenderer>().sprite = Dead;
                    }
                    //Buff Info Plate
                    if (unitCard.Model.SpecialEffects != null && deltaValues.ToInt("attackValue") <= 0)
                    {
                         
                        GameObject iconPlacerClone;
                        iconPlacerClone = Instantiate(iconPlacer, _spawnPointList[0].localPosition - iconSetter - new Vector3(0, 120, 0), Quaternion.identity, unitCard.transform) as GameObject;
                        iconPlacerClone.GetComponent<SpriteRenderer>().sprite = BuffPlate;
                        iconPlacerClone.GetComponentInChildren<TMP_Text>().characterSpacing = 0;
                        iconPlacerClone.GetComponentInChildren<TMP_Text>().text = "Does something when you play it";
                        iconPlacerClone.GetComponentInChildren<TMP_Text>().color = Color.white;
                        iconPlacerClone.GetComponentInChildren<TMP_Text>().fontSize = 80;

                        //Middle Arrow
                        MiddlePointSetter(index, false);
                     
                    }

                }

                break;

            case CommanderAbilityCardModel:
                notEvent = false;
                //Middle Arrow
                //MiddlePointSetter(index, true);
                Debug.Log("ability");
                var abilityCard = Instantiate(Resources.Load("Prefabs/2D/SpecialAbilityCard", typeof(EventCard)) as EventCard, _spawnPointList[0].localPosition, Quaternion.identity, parentTransform);
                abilityCard.InitCard(sourceCardModel as CommanderAbilityCardModel);
                var isOpponent = abilityCard.Model.Faction == Enums.Faction.DE;
                thisCard.thisCard.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/CommanderParts/" + (isOpponent ? "Opponent" : "MyPlayer") + "/specialEffectHistory");
                abilityCard.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
                abilityCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";

                break;

            case EventCardModel:
                Debug.Log("event");
                var eventCard = Instantiate(Resources.Load("Prefabs/2D/EventCardHand", typeof(EventCard)) as EventCard, _spawnPointList[0].localPosition, Quaternion.identity, parentTransform);
                eventCard.InitCard(sourceCardModel as EventCardModel);
                eventCard.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
                eventCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";
                //HaveEffects
                GameObject iconPlacerCloneEvent;
                iconPlacerCloneEvent = Instantiate(iconPlacer, _spawnPointList[0].localPosition - iconSetter - new Vector3(0, 120, 0), Quaternion.identity, eventCard.transform) as GameObject;
                iconPlacerCloneEvent.GetComponent<SpriteRenderer>().sprite = BuffPlate;
                iconPlacerCloneEvent.GetComponentInChildren<TMP_Text>().characterSpacing = 0;
                iconPlacerCloneEvent.GetComponentInChildren<TMP_Text>().text = "Does something when you play it";
                iconPlacerCloneEvent.GetComponentInChildren<TMP_Text>().color = Color.white;
                iconPlacerCloneEvent.GetComponentInChildren<TMP_Text>().fontSize = 80;

                break;

            case WeaponCardModel:
                var weaponCard = Instantiate(Resources.Load("Prefabs/2D/WeaponCard", typeof(WeaponCard)) as WeaponCard, _spawnPointList[0].localPosition, Quaternion.identity, parentTransform);
                weaponCard.InitCard(sourceCardModel as WeaponCardModel);
                var isOpponent1 = weaponCard.Model.Faction == Enums.Faction.DE;
                thisCard.thisCard.sprite = Resources.Load<Sprite>($"Prefabs/2D/Textures/CommanderParts/" + (isOpponent1 ? "Opponent" : "MyPlayer") + "/weaponHistory");
                weaponCard.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
                weaponCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";

                //Middle Arrow
                MiddlePointSetter(index, true);
               
        
                break;
               


            default: Debug.Log("not found card model"); break;
        }

        if (effectedCards != null && notEvent)
        {
            if (effectedCards.Count <= 6)
            {
                activeList = _spawnPointList;
                spawnSize = new Vector3(2, 2, 2);
            }
            else if (effectedCards.Count > 6)
            {
                activeList = _spawnPointListLarge;
                spawnSize = new Vector3(1, 1, 1);

            }
            for (int i = 0; i < effectedCards.Count; i++)
            {
                var cardModel = effectedCards[i].card;
                switch (cardModel)
                {
                    case UnitCardModel:
                        var unitCard = Instantiate(Resources.Load("Prefabs/2D/UnitCardHand", typeof(UnitCardHand)) as UnitCardHand, activeList[i + 1].localPosition, Quaternion.identity, parentTransform);
                        unitCard.InitCard(cardModel as UnitCardModel);
                        unitCard.GetComponent<RectTransform>().localScale = spawnSize;
                        unitCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";

                        if (unitCard.Model.HealthPoint <= 0)
                        {
                            GameObject iconPlacerClone;
                            iconPlacerClone = Instantiate(iconPlacer, activeList[i + 1].localPosition - iconSetter, Quaternion.identity, unitCard.transform) as GameObject;
                            iconPlacerClone.GetComponent<SpriteRenderer>().sprite = Dead;

                        }
                        else if (effectedCards[i].effect != null)
                        {
                           
                            unitCard.gameObject.SetActive(false);

                            //Middle Arrow
                            historyLogs[index].transform.GetChild(0).gameObject.SetActive(false);

                            // if (effectedCards[i].effect.Type == Enum.SpecialEffectTypes.GiveAttackPoint ||
                            //     effectedCards[i].effect.Type == Enum.SpecialEffectTypes.Retreat ||
                            //     effectedCards[i].effect.Type == Enum.SpecialEffectTypes.GiveDefensePoint)
                            // {
                            //     GameObject iconPlacerClone;
                            //     //Just buff info for mvp
                            //     iconPlacerClone = Instantiate(iconPlacer, _spawnPointList[0].localPosition - iconSetter - new Vector3(0, 50, 0), Quaternion.identity, unitCard.transform) as GameObject;
                            //     iconPlacerClone.GetComponent<SpriteRenderer>().sprite = BuffPlate;
                            //     iconPlacerClone.GetComponentInChildren<TMP_Text>().text = "Does something when you play it";
                            // }

                        }
                        else if (deltaValues != null)
                        {
                            int attackValue = deltaValues.ToInt("attackValue");
                            if (attackValue > 0)
                            {
                                GameObject iconPlacerClone;
                                iconPlacerClone = Instantiate(iconPlacer, activeList[i + 1].localPosition - iconSetter + new Vector3(25, 27, 0), Quaternion.identity, unitCard.transform) as GameObject;
                                iconPlacerClone.GetComponent<SpriteRenderer>().sprite = Damaged;
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().text = "-" + attackValue.ToString();
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().color = Color.red;
                                //iconPlacerClone.GetComponentInChildren<TMP_Text>().rectTransform.position = new Vector3(50,75,0);
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().rectTransform.position = activeList[i + 1].localPosition - iconSetter + new Vector3(42, 63, 0);
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().fontSize = 200;
                                
                            }

                        }
                        break;
                    // case EventCardModel:
                    //     var eventCard = Instantiate(Resources.Load("Prefabs/2D/EventCardHand", typeof(EventCard)) as EventCard, activeList[i + 1].localPosition, Quaternion.identity, parentTransform);
                    //     eventCard.InitCard(cardModel as EventCardModel);
                    //     eventCard.GetComponent<RectTransform>().localScale = spawnSize;
                    //     eventCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";
                    // break;
                    
                    case CommanderModel:
                        var commanderCard = Instantiate(Resources.Load("Prefabs/2D/CommanderHistoryCard", typeof(CommanderHistoryCard)) as CommanderHistoryCard,  activeList[i + 1].localPosition, Quaternion.identity, parentTransform);
                        commanderCard.InitCard(cardModel as CommanderModel);
                        commanderCard.GetComponent<RectTransform>().localScale = spawnSize;
                        commanderCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";

                        //Attack to Commander - Damaged Commander
                        if (deltaValues != null)
                        {
                            int attackValue = deltaValues.ToInt("attackValue");
                            if (attackValue > 0)
                            {
                                GameObject iconPlacerClone;
                                iconPlacerClone = Instantiate(iconPlacer, activeList[i + 1].localPosition - iconSetter + new Vector3(25, 27, 0), Quaternion.identity, commanderCard.transform) as GameObject;
                                iconPlacerClone.GetComponent<SpriteRenderer>().sprite = Damaged;
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().text = "-" + attackValue.ToString();
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().color = Color.red;
                                //iconPlacerClone.GetComponentInChildren<TMP_Text>().rectTransform.position = new Vector3(50,75,0);
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().rectTransform.position = activeList[i + 1].localPosition - iconSetter + new Vector3(42, 63, 0);
                                iconPlacerClone.GetComponentInChildren<TMP_Text>().fontSize = 200;
                                
                            }

                        }

                    break;
                    
                    // case CommanderModel:
                    //     var commanderCard = Instantiate(Resources.Load("Prefabs/2D/CommanderCardHand", typeof(Commander)) as Commander, _spawnPointList[0].localPosition, Quaternion.identity, parentTransform);
                    //     commanderCard.InitCard(sourceCardModel as CommanderModel);
                    //     commanderCard.GetComponent<RectTransform>().localScale = new Vector3(2, 2, 2);
                    //     commanderCard.GetComponent<SortingGroup>().sortingLayerName = "UICard";
                    //     break;

                    default: Debug.Log("not found card model"); break;

                }
            }


        }

        historyCardList[index].SetActive(true);

    }

    //Open - Close History Panel
    public void LogActivator(int staticIndexOfCard){
        if(!isPause){
            foreach(GameObject i in historyLogs){
        
            i.SetActive(false);
            }
            historyLogs[staticIndexOfCard].SetActive(true);
            LogBackground.SetActive(true);

            foreach(GameObject i in historyCardList){
        
                i.GetComponent<CardHistory>().myFrame.sortingLayerName = "UI";
                i.GetComponent<CardHistory>().thisCard.sortingLayerName = "UI";
            }
        } 

    }
    public void LogDisabler(int staticIndexOfCard){
        
        historyLogs[staticIndexOfCard].SetActive(false);
        LogBackground.SetActive(false);

        foreach(GameObject i in historyCardList){
        
             i.GetComponent<CardHistory>().myFrame.sortingLayerName = "Board";
             i.GetComponent<CardHistory>().thisCard.sortingLayerName = "Board";
         }
    
    }
    public void LogDisablerAll(){
        
        LogBackground.SetActive(false);
        foreach (GameObject i in historyLogs){
            i.SetActive(false);
        }
        foreach(GameObject i in historyCardList){
            
             i.GetComponent<CardHistory>().myFrame.sortingLayerName = "Board";
             i.GetComponent<CardHistory>().thisCard.sortingLayerName = "Board";
         }
    }
    public void DeleteMyHistoryLog(int staticIndexOfCard){
        for(int i=historyLogs[staticIndexOfCard].transform.childCount-1; i > 0; i--){
            Destroy(historyLogs[staticIndexOfCard].transform.GetChild(i).gameObject);
        }
        
        
    }
    public void MiddlePointSetter(int index, bool choice){

        historyLogs[index].transform.GetChild(0).gameObject.SetActive(choice);
    
    }

}

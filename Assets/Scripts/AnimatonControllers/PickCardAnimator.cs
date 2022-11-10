using System.Collections;
using System.Collections.Generic;
using StormWarfare.Gameplay;
using UnityEngine;

public class PickCardAnimator : MonoBehaviour
{
    [SerializeField] GameObject _gameObject;
    BoardController boardController;

    void Start()
    {
        boardController = _gameObject.GetComponent<BoardController>();
    }

    void PickCard()
    {
        boardController.PickCard();
    }
}

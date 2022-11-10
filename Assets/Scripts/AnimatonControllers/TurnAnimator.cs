using System.Collections;
using System.Collections.Generic;
using StormWarfare.Gameplay;
using Unity.VisualScripting;
using UnityEngine;

public class TurnAnimator : MonoBehaviour
{
    [SerializeField] GameObject _gameObject;
    private BoardController boardController;

    void Start()
    {
        boardController = _gameObject.GetComponent<BoardController>();
    }

    private void EndTurn()
    {
        boardController.EndTurn();
    }
}

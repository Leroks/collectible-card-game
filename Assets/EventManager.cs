using StormWarfare.Card;
using StormWarfare.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void EndTurn(bool isMyTurn);
    public static event EndTurn OnEndTurn;
    public static void EndTurned(bool isMyTurn) => OnEndTurn?.Invoke(isMyTurn);
}

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonBehaviour : MonoBehaviour
{
    public UnityEvent<UnityAction> Actions = new();
    void OnMouseDown() => Actions?.Invoke(null);
}
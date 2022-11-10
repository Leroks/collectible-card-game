using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {

    public GameObject door;
    bool doorOpened = false;

    private void OnMouseDown () {

        if (doorOpened == false)
            StartCoroutine (SmoothMovement.SmoothRotation (door, door.transform.parent.rotation, 2.5f));

        doorOpened = true;
    }
}
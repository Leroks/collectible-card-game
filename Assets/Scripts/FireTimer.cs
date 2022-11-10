using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTimer : MonoBehaviour {

    Vector3 initialPosition = new Vector3 (800.0f, 0.0f, 0.0f);
    Vector3 targetPosition = new Vector3 (-800.0f, 0.0f, 0.0f);
    private float roundDuration = 30.0f;

    public void StartTimer () {
        this.gameObject.transform.position = initialPosition;
        this.gameObject.SetActive (true);
        StartCoroutine (SmoothMovement.SmoothTranslation (this.gameObject, targetPosition, roundDuration));
        StartCoroutine (HideFire ());
    }

    IEnumerator HideFire () {
        yield return new WaitForSeconds (roundDuration + 2.0f);
        this.gameObject.SetActive (false);
    }
}
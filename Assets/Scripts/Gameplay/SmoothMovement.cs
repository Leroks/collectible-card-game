using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SmoothMovement {
    public static IEnumerator SmoothTranslation (GameObject go, Vector3 target, float seconds) {
        float startTime = 0.0f;
        Vector3 startPos = go.transform.localPosition;
        while (startTime < seconds) {
            go.transform.localPosition = Vector3.Lerp (startPos, target, startTime / seconds);
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame ();
        }
        go.transform.localPosition = target;
    }

    public static IEnumerator SmoothTranslationHistory (GameObject go, Vector3 target, float seconds) {
        float startTime = 0.0f;
        Vector3 startPos = go.transform.position;
        while (startTime < seconds) {
            go.transform.position = Vector3.Lerp (startPos, target, startTime / seconds);
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame ();
        }
        go.transform.position = target;
    }

    public static IEnumerator SmoothRotation (GameObject go, Quaternion target, float seconds) {
        float startTime = 0.0f;
        Quaternion currentRot = go.transform.rotation;
        while (startTime < seconds) {
            go.transform.rotation = Quaternion.Lerp (currentRot, target, startTime / seconds);
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame ();
        }
        go.transform.rotation = target;
    }

    public static IEnumerator SmoothTranslation (GameObject go, Vector3 targetPos, Vector3 anchorPos, Vector3 worldUp, float seconds) {
        float startTime = 0.0f;
        Vector3 startPos = go.transform.localPosition;
        while (startTime < seconds) {
            go.transform.localPosition = Vector3.Lerp (startPos, targetPos, startTime / seconds);
            go.transform.LookAt (anchorPos, worldUp);
            var rot = go.transform.localRotation;
            rot.z += 0.13f;
            go.transform.localRotation = rot;
            startTime += Time.deltaTime;
            Debug.Log ("moving");
            yield return new WaitForEndOfFrame ();
        }
        go.transform.LookAt (anchorPos, Vector3.up);
        var tempRot = go.transform.localRotation;
        tempRot.z += 0.13f;
        Debug.Log ("moving");
        go.transform.localRotation = tempRot;
        go.transform.localPosition = targetPos;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTest : MonoBehaviour {

    public Countdown countdown;

    // Start is called before the first frame update
    void Start () {
    }

    // Update is called once per frame
    void Update () {

    }

    void HalfTime () {
        Debug.Log ("half time");
    }

    void LastTen () {
        Debug.Log ("last ten");
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//TODO: coroutine yerine time.delta gibi platform bazli sure sayan bir yapi ile yazilabilir.
public class Countdown : MonoBehaviour
{

    public TMPro.TextMeshPro Seconds0;
    public TMPro.TextMeshPro Seconds1;
    public TMPro.TextMeshPro MilliSeconds0;
    public TMPro.TextMeshPro MilliSeconds1;

    public int Time;

    public delegate void TimeOutDelegate();
    public TimeOutDelegate TimeOut;

    [SerializeField] public GameObject turnTimer;
    [SerializeField] public GameObject endTurnPress;
    [SerializeField] public GameObject enemyTurnPress;

    public void StartCountdown(int countDownTimeSeconds = 0, TimeOutDelegate timeOut = null)
    {
        StopAllCoroutines();
        Time = countDownTimeSeconds;
        this.TimeOut = timeOut;
        MilliSeconds0.text = "0";
        MilliSeconds1.text = "0";
        StartCoroutine(RunCountdown());
    }
    private IEnumerator RunCountdown2()
    {
        int time = 99;
        while (Time < 21)
        {
            yield return new WaitForSeconds(.01f);
            time -= 1;
            MilliSeconds1.text = $"{time / 10}";
            MilliSeconds0.text = $"{time % 10}";
            if (time == 0)
                time = 99;
        }
    }

    private IEnumerator RunCountdown()
    {
        while (Time > 0)
        {
            yield return new WaitForSeconds(1.0f);
            Time -= 1;
            Seconds1.text = $"{Time / 10}";
            Seconds0.text = $"{Time % 10}";
            if (!enemyTurnPress.activeSelf)
            {
                if (Time == 20)
                {
                    turnTimer.SetActive(true);
                    endTurnPress.SetActive(false);
                    StartCoroutine(RunCountdown2());
                }
            }
            if (Time <= 0 && TimeOut != null)
            {
                TimeOut();
                turnTimer.SetActive(false);
            }
        }
    }
    /*    void Update()
    {
        if (_runTimer)
        {
            if (Time > 0)
            {
                Time -= UnityEngine.Time.deltaTime;
                DisplayTime(Time);
            }
            else
            {
                Debug.Log("Time has run out!");
                Time = 0;
                MilliSeconds.text = "00";
                turnTimer.SetActive(false);
                if (TimeOut != null)
                    TimeOut();
                _runTimer = false;
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;
        if (Time <= 20 && !turnTimer.activeInHierarchy)
        {
            turnTimer.SetActive(true);
            endTurnPress.SetActive(false);
            MilliSeconds.text = ((timeToDisplay * 100.0f) % 100.0f).ToInt().ToString();
        }
        Seconds.text = timeToDisplay.ToInt().ToString();
    }*/
}
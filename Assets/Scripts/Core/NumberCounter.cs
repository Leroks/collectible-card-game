using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class NumberCounter : MonoBehaviour
    {
        public int MaxValue;
        public TextMeshPro Text;
        public int CountFPS = 30;
        public float Duration = 1f;
        public string NumberFormat = "N0";
        int _value;
        Coroutine _countingCoroutine;
        public int Value
        {
            get => _value;

            set
            {
                UpdateText(value);
                _value = value;
            }
        }

        void Awake() => Text = GetComponent<TextMeshPro>();

        void UpdateText(int newValue)
        {
            if (_countingCoroutine != null)
                StopCoroutine(_countingCoroutine);
            _countingCoroutine = StartCoroutine(CountText(newValue));
        }

        IEnumerator CountText(int newValue)
        {
            WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
            int previousValue = _value;
            int stepAmount;
            if (newValue - previousValue < 0)
                stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration));
            else
                stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration));

            if (previousValue < newValue)
            {
                while (previousValue < newValue)
                {
                    previousValue += stepAmount;
                    if (previousValue > newValue)
                        previousValue = newValue;
                    Text.SetText(previousValue.ToString(NumberFormat) + "/" + MaxValue);
                    yield return Wait;
                }
            }
            else
            {
                while (previousValue > newValue)
                {
                    previousValue += stepAmount;
                    if (previousValue < newValue)
                        previousValue = newValue;
                    Text.SetText(previousValue.ToString(NumberFormat) + "/" + MaxValue);
                    yield return Wait;
                }
            }
        }
    }
}

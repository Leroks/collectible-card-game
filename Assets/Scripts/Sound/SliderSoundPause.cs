using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderSoundPause : MonoBehaviour
{
    [SerializeField] string volumeParam = "MasterVolume"; 
    [SerializeField] AudioMixer mixer; 
    [SerializeField] Slider slider;
    [SerializeField] float multiplier;  
    int oldPercentValue;
    int newPercentValue;
    int upOrDown = 0; 

    bool hundred = false;

    private void Awake(){

        slider.onValueChanged.AddListener(HandleSlider);
    }

    private void Start(){
        
        slider.value = PlayerPrefs.GetFloat(volumeParam, slider.value);
      
    }

    private void HandleSlider(float value){
        
       
        mixer.SetFloat(volumeParam, Mathf.Log10(value) * multiplier);

        
       
    }

    void OnDisable(){
        PlayerPrefs.SetFloat(volumeParam, slider.value);

        oldPercentValue = (int)((slider.value - 0.0001f) * 100 + 0.01f);
    }

    public void ChangeVolume(){
        mixer.SetFloat(volumeParam, slider.value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class SliderSoundSettings : MonoBehaviour
{
    [SerializeField] string volumeParam = "MasterVolume"; 
    [SerializeField] AudioMixer mixer; 
    [SerializeField] Slider slider;
    [SerializeField] float multiplier;  
    int oldPercentValue;
    int newPercentValue;
    int upOrDown = 0;  

    [SerializeField] Animator animatorLeft;
    [SerializeField] Animator animatorMiddle;
    [SerializeField] Animator animatorRight;

    bool hundred = false;

    private void Awake(){

        slider.onValueChanged.AddListener(HandleSlider);
    }

    private void Start(){
        
        //PlayerPrefs.DeleteAll();

        slider.value = PlayerPrefs.GetFloat(volumeParam, slider.value);
        //slider.value = 100;

        oldPercentValue = (int)((slider.value - 0.0001f) * 100 + 0.01f);

        if(oldPercentValue == 100){
            hundred = true;
            animatorLeft.Play("Counter01");
            animatorRight.Play("Counter90");
            animatorMiddle.Play("Counter90");
        }
        else if( oldPercentValue == 0){
            animatorLeft.Play("Counter10");
            animatorRight.Play("Counter90");
            animatorMiddle.Play("Counter90");
        }
        else{
            int sFig0 = oldPercentValue % 10;
            int sFig1 = oldPercentValue / 10;
            if(sFig1 == 10) sFig1 = 0;

            if(oldPercentValue % 100 != 0){
                animatorLeft.Play("Counter10");
            }
            else{
                animatorLeft.Play("Counter01");
            }

            if(sFig0+1 == 10) sFig0 --; if(sFig1+1 == 10) sFig1 --;

            if(sFig0 != 0) {
                
                animatorRight.Play("Counter" +  sFig0 + (sFig0 + 1));}
            else{animatorRight.Play("Counter10");}
            
            if(sFig1 != 0) {
                animatorMiddle.Play("Counter" +  sFig1 + (sFig1 + 1));
             
            }
            else{animatorMiddle.Play("Counter10");}
            
        }
    }

    private void HandleSlider(float value){
        newPercentValue = (int)((slider.value - 0.0001f) * 100 + 0.01f);
        if(newPercentValue > oldPercentValue){
            upOrDown = 1; 
        }
        else if(newPercentValue < oldPercentValue){
            upOrDown = -1; 
        }
        else{
            upOrDown = 0; 
        }
        mixer.SetFloat(volumeParam, Mathf.Log10(value) * multiplier);

        ChangeNumbers(upOrDown, oldPercentValue, newPercentValue);

        oldPercentValue = newPercentValue;

    }

    void OnDisable(){
        PlayerPrefs.SetFloat(volumeParam, slider.value);

        oldPercentValue = (int)((slider.value - 0.0001f) * 100 + 0.01f);
    }

    void ChangeNumbers(int direction, int old, int now){
        int fig0 = now % 10;
        int fig1 = now / 10;
        if(fig1 == 10) fig1 = 0;

        // 100
        if(now > 99){
                animatorLeft.Play("Counter01");
                animatorRight.Play("Counter90");
                animatorMiddle.Play("Counter90");
                hundred = true;
            }
        else if(hundred){
            animatorLeft.Play("Counter10");
            animatorMiddle.Play("Counter09");
            hundred = false;
        
        }
        // 00
        if(now < 1){
                animatorMiddle.Play("Counter10");
                animatorRight.Play("Counter10");
                animatorMiddle.Play("Counter10");
        }
      
        // others
        if(direction == 1 && now >= 1 && now <= 99){

            if(fig0 != 9){
               
                animatorRight.Play("Counter" + fig0 + (fig0 + 1));
            }
            else{
                animatorRight.Play("Counter90");
                //animatorMiddle.Play("Counter" + fig1 + (fig1 + 1));
            }

            if(fig1 != 9){
                animatorMiddle.Play("Counter" + fig1 + (fig1 + 1));
            }
            else{
                animatorMiddle.Play("Counter90");
            }

            
            
        }
        else if(direction == -1 && now >= 1 && now <= 99){
    
            if(fig0 != 0){
                animatorRight.Play("Counter" +  fig0 + (fig0 - 1));
            }
            else{
                animatorRight.Play("Counter09");
                // if(fig1 != 0)
                // animatorMiddle.Play("Counter" + fig1 + (fig1 - 1));
            }

            if(fig1 != 0){
                animatorMiddle.Play("Counter" + fig1 + (fig1 - 1));
            }
            
        }
    }

    public void ChangeVolume(){
        mixer.SetFloat(volumeParam, slider.value);
    }
}

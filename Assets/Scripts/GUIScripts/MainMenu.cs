using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{

    // [SerializeField]
    // GameObject panelOptions;

    //public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    
    public void LoadScene(int sceneNumber){
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);
    }
    // public void OptionsButton(){
    //     panelOptions.SetActive(true);
    // }
    // public void OptionsButtonBack(){
    //     panelOptions.SetActive(false);
    // }

    //Audio masterMixer;
    AudioMixer musicMixer;

    void Start(){

        //resolutions = Screen.resolutions;

        //resolutionDropdown.ClearOptions();

        // List<string> options = new List<string>();

        // int currentResolutionIndex = 0;

        // for(int i =0; i < resolutions.Length; i++){
        //     string option = resolutions[i].width + " x " + resolutions[i].height;
        //     options.Add(option);

        //     if(resolutions[i].width == Screen.currentResolution.width &&
        //     resolutions[i].height == Screen.currentResolution.height)
        //     {
        //         currentResolutionIndex = i;
        //     }
        // }

        // resolutionDropdown.AddOptions(options);
        // resolutionDropdown.value = currentResolutionIndex;
        // resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex){
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQualitySetting(int quality){
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetScreen(bool isFull){
      
        Screen.fullScreen = isFull;
    }

}

using System.Collections;
using System.Collections.Generic;
using StormWarfare.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    // [SerializeField]
    // GameObject panelMenu;
    [SerializeField]
    GameObject panelPauseMenu;
    [SerializeField] GameObject HistoryContainer;
    private bool esc = true;
    // [SerializeField]
    // GameObject panelOptions;

    //public TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;

    void Start()
    {

        resolutions = Screen.resolutions;

        // resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
            resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // resolutionDropdown.AddOptions(options);
        // resolutionDropdown.value = currentResolutionIndex;
        // resolutionDropdown.RefreshShownValue();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && esc)
        {
            PauseButton();
            esc = false;
            
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && !esc)
        {
            BackButton();
            esc = true;
            
        }
    }

    public void BackButton()
    {
        //panelMenu.SetActive(true);
        panelPauseMenu.SetActive(false);
        HistoryContainer.GetComponent<HistoryCardContainer>().isPause = false;
    }
    public void PauseButton()
    {
        //panelMenu.SetActive(false);
        // for(int i= 0; i< 14; i++){
        //     HistoryContainer.GetComponent<HistoryCardContainer>().LogDisabler(i);
        // }
        panelPauseMenu.SetActive(true);
        HistoryContainer.GetComponent<HistoryCardContainer>().LogDisablerAll();
        HistoryContainer.GetComponent<HistoryCardContainer>().isPause = true;
    }
    // public void OptionsButton(){
    //     panelOptions.SetActive(true);

    // }
    // public void OptionsButtonBack(){
    //     panelOptions.SetActive(false);

    // }

    //TO DO
    public void ConcedeButton(int sceneNumber)
    {
        SceneManager.sceneLoaded -= GameManager.Instance.BoardController.OnSceneLoaded;
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQualitySetting(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetScreen(bool isFull)
    {

        Screen.fullScreen = isFull;
    }
}

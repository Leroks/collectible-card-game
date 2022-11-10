using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public GameObject MainMenu;
    public GameObject Options;
    public float AnimationSecond;
    Coroutine _co;

    [SerializeField] List<GameObject> objectsWithAnimator = new List<GameObject>();
    [SerializeField] GameObject backButton;
    private void Start()
    {
        GoToMainMenu();
    }
    public void GoToMainMenu()
    {
        if (_co != null)
            StopCoroutine(_co);
        _co = StartCoroutine(MenuAnimation(true));
    }

    public void GoToSettingsMenu()
    {
        if (_co != null)
            StopCoroutine(_co);
        _co = StartCoroutine(MenuAnimation(false));
    }
    public IEnumerator MenuAnimation(bool ShowMenu)
    {
        float startTime = -0.5f;
        var targetPosSettings = new Vector3(0, 700, 0);
        var targetPosMenu = new Vector3(0, -700, 0);
        var targetPostSettingsFix = new Vector3(-7.5f, 0, 0);
        var startPosMenu = MainMenu.transform.position;
        var startPosSettings = Options.transform.position;
        if (ShowMenu)
            targetPosMenu = Vector3.zero;
        else
            targetPosSettings = targetPostSettingsFix;

        while (startTime < AnimationSecond)
        {
            MainMenu.transform.position = Vector3.Lerp(startPosMenu, targetPosMenu, startTime / AnimationSecond);
            Options.transform.position = Vector3.Lerp(startPosSettings, targetPosSettings, startTime / AnimationSecond);
            startTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        MainMenu.transform.position = targetPosMenu;
        Options.transform.position = targetPosSettings;
    }

    public void FriendlyPvPButton()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void AnimatorEnable()
    {
        foreach (GameObject m in objectsWithAnimator)
        {
            m.GetComponent<Animator>().enabled = true;
        }
    }

    private void Update()
    {
        if (backButton != null)
        {
            if (MainMenu.transform.position != Vector3.zero)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    backButton.GetComponent<MenuAnimators>().OnMouseDown();
                    GoToMainMenu();
                }
            }
        }
    }
}

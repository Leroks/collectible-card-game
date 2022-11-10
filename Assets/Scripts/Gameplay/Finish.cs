using Assets.Scripts.Core;
using StormWarfare.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro Level;
    [SerializeField] private NumberCounter Rank;

    [SerializeField] SpriteRenderer YouWinLoseDescription;
    [SerializeField] SpriteRenderer YouWinLoseWings;
    [SerializeField] SpriteRenderer CommanderImage;

    public GameObject EthChart;

    public void InitModels(CommanderModel myCommander, EndGameModel endGameModel)
    {
        Level.text = myCommander.Level.ToString();
        Rank.Value = endGameModel.CurrentRank;
        Rank.MaxValue = endGameModel.MaxRank;
        AudioManager.PlaySound(endGameModel.AmIWinner ? "Finish-Win" : "Finish-Lose");
        YouWinLoseDescription.sprite = Resources.Load<Sprite>("Textures/Finish/You" + (endGameModel.AmIWinner ? "Win" : "Lose") + "Label");
        YouWinLoseWings.sprite = Resources.Load<Sprite>("Textures/Finish/You" + (endGameModel.AmIWinner ? "Win" : "Lose") + "Wings");
        CommanderImage.sprite = Resources.Load<Sprite>($"Textures/{myCommander.Texture}");
    }

    private void OnMouseEnter()
    {
        EthChart.SetActive(true);
    }

    private void OnMouseExit()
    {
        EthChart.SetActive(false);
    }

    //todo: menuye yonlendir
    public void Continue()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}

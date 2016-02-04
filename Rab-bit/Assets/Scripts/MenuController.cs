using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void ClickedBack()
    {
        gameManager.SetGameState(gameManager.previousGameState);
    }

    public void ClickedQuit()
    {
        Debug.Log("Clicked Quit button");
        Application.Quit();
    }

    public void ClickedPlay()
    {
        gameManager.SetGameState(GameState.Playing);
    }

    public void ClickedMenu()
    {
        gameManager.SetGameState(GameState.Menu);
    }

    public void ClickedAbout()
    {
        gameManager.SetGameState(GameState.About);
    }

    public void ClickedHighscore()
    {
        gameManager.SetGameState(GameState.Highscore);
    }

    public void ClickedPlaySelectDifficulty()
    {
        gameManager.SetGameState(GameState.DifficultySelection);
    }

    public void ClickedEasy()
    {
        PlayerController.setDifficulty(GameDifficulty.EASY);
        gameManager.SetGameState(GameState.Playing);
    }

    public void ClickedModerate()
    {
        PlayerController.setDifficulty(GameDifficulty.MODERATE);
        gameManager.SetGameState(GameState.Playing);
    }

    public void ClickedHard()
    {
        PlayerController.setDifficulty(GameDifficulty.HARD);
        gameManager.SetGameState(GameState.Playing);
    }

    public void ClickedTutorial1()
    {
        gameManager.SetGameState(GameState.Tutorial1);
    }

    public void ClickedTutorial2()
    {
        gameManager.SetGameState(GameState.Tutorial2);
    }

    public void ClickedTutorial3()
    {
        gameManager.SetGameState(GameState.Tutorial3);
    }

    public void ClickedTutorial4()
    {
        gameManager.SetGameState(GameState.Tutorial4);
    }

    public void ClickedTutorial5()
    {
        gameManager.SetGameState(GameState.Tutorial5);
    }
}

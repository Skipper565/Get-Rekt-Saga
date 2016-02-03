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

    public void ClickedPlay()
    {
        Debug.Log("Clicked Play button");
        gameManager.SetGameState(GameState.Playing);
    }

    public void ClickedMenu()
    {
        Debug.Log("Clicked Menu button");
        gameManager.SetGameState(GameState.Menu);
    }

    public void ClickedAbout()
    {
        Debug.Log("Clicked About button");
        gameManager.SetGameState(GameState.About);
    }

    public void ClickedBack()
    {
        Debug.Log("Clicked Back button");
        gameManager.SetGameState(gameManager.previousGameState);
    }

    public void ClickedHighscore()
    {
        Debug.Log("Clicked Highscore button");
        gameManager.SetGameState(GameState.Highscore);
    }

    public void ClickedQuit()
    {
        Debug.Log("Clicked Quit button");
        Application.Quit();
    }

    public void ClickedPlaySelectDifficulty()
    {
        Debug.Log("Clicked Play button, select difficulty");
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
}

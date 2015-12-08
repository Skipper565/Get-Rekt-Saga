using UnityEngine;
using System.Collections;

public class GameStateController : MonoBehaviour {

    private GameManager gameManager;

    private GameObject gameOverMenu;
    private GameObject mainMenu;

    // Use this for initialization
    void Awake() {
        gameManager = GameManager.Instance;
        gameManager.OnStateChange += ManageStateChange;

        gameOverMenu = UIManager.GameOverMenu;
        mainMenu = UIManager.MainMenu;

        Debug.Log(gameManager.gameState);
        if (gameManager.gameState != GameState.Menu)
        {
            gameManager.SetGameState(GameState.Playing);
        }
        else
        {
            gameManager.SetGameState(GameState.Menu);
        }
    }

    public void ManageStateChange()
    {
        Debug.Log("Changing game state to " + gameManager.gameState);

        switch (gameManager.gameState)
        {
            case GameState.Menu:
                InMenu();
                break;
            case GameState.Playing:
                WhilePlaying();
                break;
            case GameState.Pause:
                WhilePaused();
                break;
            case GameState.GameOver:
                GameOver();
                break;
        }
    }

    private void InMenu()
    {
        //gameOverMenu.enabled = false;
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(true);
        Time.timeScale = 0;
    }

    private void WhilePlaying()
    {
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void WhilePaused()
    {
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(false);
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        gameOverMenu.SetActive(true);
        mainMenu.SetActive(false);
        Time.timeScale = 1;
    }
}

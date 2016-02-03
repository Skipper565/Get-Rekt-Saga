using UnityEngine;
using System.Collections;

public class GameStateController : MonoBehaviour {

    private GameManager gameManager;

    private GameObject gameOverMenu;
    private GameObject mainMenu;
    private GameObject aboutMenu;
    private GameObject hudMenu;
    private GameObject highscoreMenu;
    private GameObject difficultyMenu;
    private GameObject nicknameMenu;

    private Animator nicknameMenuAnimator;

    private PlayerController playerController;

    // Use this for initialization
    void Start() {

        // Load GameManager
        gameManager = GameManager.Instance;
        gameManager.OnStateChange += ManageStateChange;

        // Load UI screens
        gameOverMenu = UIManager.GameOverMenu;
        mainMenu = UIManager.MainMenu;
        aboutMenu = UIManager.AboutMenu;
        hudMenu = UIManager.HudMenu;
        highscoreMenu = UIManager.HighscoreMenu;
        difficultyMenu = UIManager.DifficultyMenu;

        // Load player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Disable all menus; enable them as their events are fired
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        highscoreMenu.SetActive(false);
        difficultyMenu.SetActive(false);

        // Go to menu when the game starts, but don't when it's just restarted
        if (gameManager.gameState == GameState.NewGame)
        {
            gameManager.SetGameState(GameState.Menu);
        }
        // Else the game was restarted and is now in the Playing state
        else
        {
            // Unsubscribe from the event if the game was restarted to prevent duplicate event calls
            gameManager.OnStateChange -= ManageStateChange;

            hudMenu.SetActive(true);
            TriggerHideMenuAnimation();
        }
    }

    public void ManageStateChange()
    {
        Debug.Log("Changing game state to " + gameManager.gameState);

        switch (gameManager.gameState)
        {
            case GameState.Playing:
                WhilePlaying();
                break;
            case GameState.Pause:
                WhilePaused();
                break;
            case GameState.GameOver:
                GameOver();
                break;
            case GameState.Menu:
                InMenu();
                break;
            case GameState.About:
                InMenu();
                break;
            case GameState.Highscore:
                InMenu();
                break;
            case GameState.DifficultySelection:
                InMenu();
                break;
        }
    }

    private void WhilePlaying()
    {
        hudMenu.SetActive(true);

        if (gameManager.previousGameState != GameState.Pause)
        {
            playerController.Restart();
        }
    }

    private void WhilePaused()
    {
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        hudMenu.SetActive(false);
        TriggerShowMenuAnimation();
    }

    private void InMenu()
    {
        hudMenu.SetActive(false);

        TriggerHideMenuAnimation();
        TriggerShowMenuAnimation();

        Time.timeScale = 0;
    }

    private Animator GetMenuAnimator(GameObject menu)
    {
        var panel = menu.transform.FindChild("Panel").gameObject;
        return panel.GetComponent<Animator>();
    }

    private GameObject GetMenuFromState(GameState state)
    {
        GameObject menu = null;

        switch (state)
        {
            case GameState.GameOver:
                menu = gameOverMenu;
                break;
            case GameState.Menu:
                menu = mainMenu;
                break;
            case GameState.About:
                menu = aboutMenu;
                break;
            case GameState.Highscore:
                menu = highscoreMenu;
                break;
            case GameState.DifficultySelection:
                menu = difficultyMenu;
                break;
            default:
                break;
        }

        return menu;
    }

    private void TriggerHideMenuAnimation()
    {
        GameObject menu = GetMenuFromState(gameManager.previousGameState);

        if (menu != null)
        {
            menu.SetActive(true);
            GetMenuAnimator(menu).SetTrigger("hideMenu");
        }
    }

    private void TriggerShowMenuAnimation()
    {
        GameObject menu = GetMenuFromState(gameManager.gameState);

        if (menu != null)
        {
            if (menu.activeSelf)
            {
                GetMenuAnimator(menu).SetTrigger("showMenu");
            }
            else
            {
                menu.SetActive(true);
            }
        }
    }
}

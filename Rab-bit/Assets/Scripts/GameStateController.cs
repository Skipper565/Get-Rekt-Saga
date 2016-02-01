using UnityEngine;
using System.Collections;

public class GameStateController : MonoBehaviour {

    private GameManager gameManager;

    private GameObject gameOverMenu;
    private GameObject mainMenu;
    private GameObject aboutMenu;
    private GameObject hudMenu;

    private Animator gameOverMenuAnimator;
    private Animator mainMenuAnimator;
    private Animator aboutMenuAnimator;

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

        // Load menu animators
        gameOverMenuAnimator = GetMenuAnimator(gameOverMenu);
        mainMenuAnimator = GetMenuAnimator(mainMenu);
        aboutMenuAnimator = GetMenuAnimator(aboutMenu);

        // Load player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        Debug.Log(gameManager.gameState);

        // Disable all menus; enable them as their events are fired
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        gameOverMenu.SetActive(false);

        // Go to menu when the game starts, but don't when it's just restarted
        if (gameManager.gameState == GameState.NewGame)
        {
            gameManager.SetGameState(GameState.Menu);
        }
        else
        {
            // Unsubscribe from the event if the game was restarted to prevent duplicate event calls
            gameManager.OnStateChange -= ManageStateChange;

            // Finish hide menu animations after game reset.
            // NOTE: Flashing hide menu animations after pressing "play" may be caused by this! Not proved.
            if (gameManager.previousGameState == GameState.GameOver)
            {
                hudMenu.SetActive(true);
                gameOverMenu.SetActive(true);
                gameOverMenuAnimator.SetTrigger("hideMenu");
            }
            else if (gameManager.previousGameState == GameState.Menu)
            {
                mainMenu.SetActive(true);
                mainMenuAnimator.SetTrigger("hideMenu");
            }
        }
    }

    void Update()
    {
        // Fix the missing cameras problem
        SetAllMenuCameras();
    }

    public void ManageStateChange()
    {
        Debug.Log("Changing game state to " + gameManager.gameState);

        switch (gameManager.gameState)
        {
            case GameState.About:
                InAboutMenu();
                break;
            case GameState.Menu:
                InMainMenu();
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

    private void InAboutMenu()
    {
        // Always accessed from main menu
        mainMenuAnimator.SetTrigger("hideMenu");

        if (aboutMenu.activeSelf)
        {
            aboutMenuAnimator.SetTrigger("showMenu");
        }
        else
        {
            aboutMenu.SetActive(true);
        }

        Time.timeScale = 0;
    }

    private void InMainMenu()
    {
        if (gameManager.previousGameState == GameState.About)
        {
            aboutMenuAnimator.SetTrigger("hideMenu");
        }
        else if (gameManager.previousGameState == GameState.GameOver)
        {
            hudMenu.SetActive(true);
            gameOverMenuAnimator.SetTrigger("hideMenu");
        }

        if (mainMenu.activeSelf)
        {
            mainMenuAnimator.SetTrigger("showMenu");
        }
        else
        {
            mainMenu.SetActive(true);
        }

        Time.timeScale = 0;
        Debug.Log("MENU");
    }

    private void WhilePlaying()
    {
        Time.timeScale = 1;
        Debug.Log("PLAY");

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

        if (gameOverMenu.activeSelf)
        {
            gameOverMenuAnimator.SetTrigger("showMenu");
        }
        else
        {
            gameOverMenu.SetActive(true);
        }

        Time.timeScale = 1;
        Debug.Log("GAME OVER");
    }

    private void SetMenuCamera(GameObject menu)
    {
        var canvas = menu.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    private void SetAllMenuCameras()
    {
        SetMenuCamera(gameOverMenu);
        SetMenuCamera(mainMenu);
        SetMenuCamera(aboutMenu);
        SetMenuCamera(hudMenu);
    }

    private Animator GetMenuAnimator(GameObject menu)
    {
        var panel = menu.transform.FindChild("Panel").gameObject;
        return panel.GetComponent<Animator>();
    }
}

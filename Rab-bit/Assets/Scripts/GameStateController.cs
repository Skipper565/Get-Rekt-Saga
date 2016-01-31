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
    private GameState oldState;

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
        oldState = gameManager.gameState;

        // Disable all menus; enable them as their events are fired
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        gameOverMenu.SetActive(false);

        // Go to menu when the game starts, but don't when it's just restarted
        if (gameManager.gameState == GameState.NewGame)
        {
            gameManager.SetGameState(GameState.Menu);
        }
        // Unsubscribe from the event if the game was restarted to prevent duplicate event calls
        else
        {
            gameManager.OnStateChange -= ManageStateChange;
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

        Debug.Log("Old: " + oldState + ", new:" + gameManager.gameState);
        if (oldState == gameManager.gameState)
        {
            Debug.Log("GAME STATES ARE EQUAL!!!");
        }

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

        oldState = gameManager.gameState;
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
        if (oldState == GameState.About)
        {
            aboutMenuAnimator.SetTrigger("hideMenu");
        }
        else if (oldState == GameState.GameOver)
        {
            hudMenu.SetActive(true);
            gameOverMenuAnimator.SetTrigger("hideMenu");
        }

        if (mainMenu.activeSelf)
        {
            mainMenuAnimator.SetTrigger("showMenu");
            Debug.Log("yo1");
        }
        else
        {
            mainMenu.SetActive(true);
            Debug.Log("yo2");
        }

        Time.timeScale = 0;
        Debug.Log("MENU");
    }

    private void WhilePlaying()
    {
        if (oldState == GameState.Menu)
        {
            mainMenuAnimator.SetTrigger("hideMenu");
        }
        else if (oldState == GameState.GameOver)
        {
            hudMenu.SetActive(true);
            gameOverMenuAnimator.SetTrigger("hideMenu");
        }

        Time.timeScale = 1;
        Debug.Log("PLAY");

        if (oldState != GameState.Pause)
        {
            //gameManager.OnStateChange -= ManageStateChange;
            playerController.Restart();
        }
    }

    private void WhilePaused()
    {
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        //gameOverMenu.transform.FindChild("Panel").GetComponent<CanvasRenderer>().SetAlpha(100);
        //gameOverMenu.transform.localScale = new Vector2(1, 1);

        //gameOverMenu.SetActive(true);
        //gameOverMenuAnimator.SetTrigger("showMenu");

        //gameOverMenuAnimator.SetTrigger("showMenu");
        //mainMenuAnimator.SetTrigger("hideMenu");
        //aboutMenuAnimator.SetTrigger("hideMenu");

        hudMenu.SetActive(false);

        if (gameOverMenu.activeSelf)
        {
            gameOverMenuAnimator.SetTrigger("showMenu");
            Debug.Log("Game over trigger");
        }
        else
        {
            gameOverMenu.SetActive(true);
            Debug.Log("Game over set true");
        }

        Time.timeScale = 1;
        Debug.Log("GAME OVER");
    }

    public void SetMenuCamera(GameObject menu)
    {
        var canvas = menu.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    public void SetAllMenuCameras()
    {
        SetMenuCamera(gameOverMenu);
        SetMenuCamera(mainMenu);
        SetMenuCamera(aboutMenu);
        SetMenuCamera(hudMenu);
    }

    public Animator GetMenuAnimator(GameObject menu)
    {
        var panel = menu.transform.FindChild("Panel").gameObject;
        return panel.GetComponent<Animator>();
    }
}

using UnityEngine;
using System.Collections;

public class GameStateController : MonoBehaviour {

    private GameManager gameManager;

    private GameObject gameOverMenu;
    private GameObject mainMenu;
    private GameObject aboutMenu;

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

        // Load menu animators
        gameOverMenuAnimator = GetMenuAnimator(gameOverMenu);
        mainMenuAnimator = GetMenuAnimator(mainMenu);
        aboutMenuAnimator = GetMenuAnimator(aboutMenu);

        // Load player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        Debug.Log(gameManager.gameState);
        if (gameManager.gameState != GameState.Menu)
        {
            gameManager.SetGameState(GameState.Playing);
            oldState = GameState.Playing;
        }
        else
        {
            gameManager.SetGameState(GameState.Menu);
            oldState = GameState.Menu;
        }

        aboutMenu.SetActive(false);
        gameOverMenu.SetActive(false);

        //gameOverMenu.transform.FindChild("Panel").GetComponent<CanvasRenderer>().SetAlpha(0);
        //gameOverMenu.transform.localScale = new Vector2(0, 0);
        /*var cg = canvas.GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;*/
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

        oldState = gameManager.gameState;
    }

    private void InAboutMenu()
    {
        aboutMenu.SetActive(true);

        mainMenuAnimator.SetTrigger("hideMenu");
        aboutMenuAnimator.SetTrigger("showMenu");

        //gameOverMenu.SetActive(false);
        //mainMenu.SetActive(true);
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
            gameOverMenuAnimator.SetTrigger("hideMenu");
        }

        mainMenuAnimator.SetTrigger("showMenu");
        Time.timeScale = 0;
    }

    private void WhilePlaying()
    {
        if (oldState == GameState.Menu)
        {
            mainMenuAnimator.SetTrigger("hideMenu");
            playerController.Restart();
        }
        else if (oldState == GameState.GameOver)
        {
            gameOverMenuAnimator.SetTrigger("hideMenu");
            playerController.Restart();
        }

        Time.timeScale = 1;
    }

    private void WhilePaused()
    {
        Time.timeScale = 0;
    }

    private void GameOver()
    {
        //gameOverMenu.transform.FindChild("Panel").GetComponent<CanvasRenderer>().SetAlpha(100);
        //gameOverMenu.transform.localScale = new Vector2(1, 1);
        gameOverMenu.SetActive(true);
        gameOverMenuAnimator.SetTrigger("showMenu");
        Time.timeScale = 0;
    }

    public Animator GetMenuAnimator(GameObject menu)
    {
        var panel = menu.transform.FindChild("Panel").gameObject;
        return panel.GetComponent<Animator>();
    }
}

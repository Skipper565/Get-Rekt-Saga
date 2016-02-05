using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStateController : MonoBehaviour {

    private GameManager gameManager;

    private GameObject gameOverMenu;
    private GameObject mainMenu;
    private GameObject aboutMenu;
    private GameObject hudMenu;
    private GameObject highscoreMenu;
    private GameObject difficultyMenu;
    private GameObject nicknameMenu;
    private GameObject moreMenu;

    private GameObject tutorial1;
    private GameObject tutorial2;
    private GameObject tutorial3;
    private GameObject tutorial4;
    private GameObject tutorial5;

    private PlayerController playerController;
    private bool dontAnimateHideNicknameMenu;

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
        nicknameMenu = UIManager.NicknameMenu;
        moreMenu = UIManager.MoreMenu;

        tutorial1 = UIManager.Tutorial1;
        tutorial2 = UIManager.Tutorial2;
        tutorial3 = UIManager.Tutorial3;
        tutorial4 = UIManager.Tutorial4;
        tutorial5 = UIManager.Tutorial5;

        // Load player controller
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();

        // Disable all menus; enable them as their events are fired
        mainMenu.SetActive(false);
        aboutMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        highscoreMenu.SetActive(false);
        difficultyMenu.SetActive(false);
        nicknameMenu.SetActive(false);
        moreMenu.SetActive(false);

        tutorial1.SetActive(false);
        tutorial2.SetActive(false);
        tutorial3.SetActive(false);
        tutorial4.SetActive(false);
        tutorial5.SetActive(false);

        dontAnimateHideNicknameMenu = true;

        // Go to menu when the game starts, but don't when it's just restarted
        if (gameManager.gameState == GameState.NewGame/* || gameManager.gameState == GameState.Nickname*/)
        {
            //gameManager.SetGameState(GameState.Menu);
            gameManager.SetGameState(GameState.Nickname);
        }
        // Else the game was restarted and is now in the Playing state
        else
        {
            if (gameManager.gameState != GameState.Playing)
            {
                Debug.LogError("Unexpected game state - should be Playing! Current game state: " + gameManager.gameState);
            }
            
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
            case GameState.NewGame:
                gameManager.SetGameState(GameState.Nickname);
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
            case GameState.Nickname:
                InNicknameEdit();
                break;
            case GameState.More:
                InMenu();
                break;
            case GameState.Tutorial1:
                InMenu();
                break;
            case GameState.Tutorial2:
                InMenu();
                break;
            case GameState.Tutorial3:
                InMenu();
                break;
            case GameState.Tutorial4:
                InMenu();
                break;
            case GameState.Tutorial5:
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

    private void InNicknameEdit()
    {
        if (gameManager.previousGameState == GameState.NewGame)
        {
            if (PlayerPrefs.GetString("playerName") != "")
            {
                dontAnimateHideNicknameMenu = true;
                gameManager.SetGameState(GameState.Menu);
                return;
            }
        }

        dontAnimateHideNicknameMenu = false;
        InMenu();

        //var inputField = nicknameMenu.transform.FindChild("Panel").FindChild("InputField").GetComponent<InputField>();
        //inputField.onEndEdit.AddListener();
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
            case GameState.Nickname:
                menu = nicknameMenu;
                break;
            case GameState.More:
                menu = moreMenu;
                break;
            case GameState.Tutorial1:
                menu = tutorial1;
                break;
            case GameState.Tutorial2:
                menu = tutorial2;
                break;
            case GameState.Tutorial3:
                menu = tutorial3;
                break;
            case GameState.Tutorial4:
                menu = tutorial4;
                break;
            case GameState.Tutorial5:
                menu = tutorial5;
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
            if (gameManager.previousGameState == GameState.Nickname && dontAnimateHideNicknameMenu)
            {
                return;
            }

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

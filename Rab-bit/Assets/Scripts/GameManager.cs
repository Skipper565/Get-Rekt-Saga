using UnityEngine;
using System.Collections;

public enum GameState
{
    NewGame,
    Playing,
    Pause,
    GameOver,
    Menu,
    About,
    Highscore,
    DifficultySelection,
    Tutorial1,
    Tutorial2,
    Tutorial3,
    Tutorial4,
    Tutorial5,
}

public delegate void OnStateChangeHandler();

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public event OnStateChangeHandler OnStateChange;
    public GameState gameState { get; private set; }
    public GameState previousGameState { get; private set; }

    // Get only single GameManager instance
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Object.FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                {
                    GameObject manager = new GameObject("_gamemanager");
                    DontDestroyOnLoad(manager);
                    _instance = manager.AddComponent<GameManager>();
                    _instance.gameState = GameState.NewGame;
                    _instance.previousGameState = GameState.NewGame;
                }
            }
            return _instance;
        }
    }

    public void SetGameState(GameState gameState)
    {
        previousGameState = this.gameState;
        this.gameState = gameState;

        if (OnStateChange != null)
        {
            OnStateChange();
        }
    }
}

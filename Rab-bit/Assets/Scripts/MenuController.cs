using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour
{
    private GameManager gameManager;
    private GameObject button;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    /*public void Clicked()
    {
        Debug.Log("Clicked");
        gameManager.SetGameState(GameState.Playing);
    }*/

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
        gameManager.SetGameState(GameState.Menu);
    }
}

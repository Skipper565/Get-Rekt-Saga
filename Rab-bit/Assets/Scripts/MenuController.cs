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

    public void Clicked()
    {
        Debug.Log("Clicked");
        gameManager.SetGameState(GameState.Playing);
    }
}

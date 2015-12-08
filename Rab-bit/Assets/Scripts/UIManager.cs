using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    public static GameObject GameOverMenu;
    public static GameObject MainMenu;

    private void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            GameOverMenu = transform.FindChild("GameOverMenu").gameObject;
            MainMenu = transform.FindChild("MainMenu").gameObject;
        }
    }
}

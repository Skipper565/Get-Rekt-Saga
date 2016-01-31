using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    public static GameObject GameOverMenu;
    public static GameObject MainMenu;
    public static GameObject AboutMenu;
    public static GameObject HudMenu;

    void Awake()
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
            AboutMenu = transform.FindChild("AboutMenu").gameObject;
            HudMenu = transform.FindChild("HUD Canvas").gameObject;
        }
    }
}

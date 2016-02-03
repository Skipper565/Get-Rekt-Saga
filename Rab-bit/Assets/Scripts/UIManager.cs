using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    public static GameObject GameOverMenu;
    public static GameObject MainMenu;
    public static GameObject AboutMenu;
    public static GameObject HudMenu;
    public static GameObject HighscoreMenu;
    public static GameObject DifficultyMenu;
    public static GameObject NicknameMenu;

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
            HighscoreMenu = transform.FindChild("HighScoreMenu").gameObject;
            DifficultyMenu = transform.FindChild("SelectDifficultyMenu").gameObject;
            NicknameMenu = transform.FindChild("NicknameEdit").gameObject;
        }
    }

    void Update()
    {
        // Fix the missing cameras problem
        SetAllMenuCameras();
    }

    private void SetAllMenuCameras()
    {
        SetMenuCamera(GameOverMenu);
        SetMenuCamera(MainMenu);
        SetMenuCamera(AboutMenu);
        SetMenuCamera(HudMenu);
        SetMenuCamera(HighscoreMenu);
        SetMenuCamera(DifficultyMenu);
    }

    private void SetMenuCamera(GameObject menu)
    {
        var canvas = menu.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}

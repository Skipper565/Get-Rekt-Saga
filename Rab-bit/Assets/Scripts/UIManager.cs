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
    public static GameObject Tutorial1;
    public static GameObject Tutorial2;
    public static GameObject Tutorial3;
    public static GameObject Tutorial4;
    public static GameObject Tutorial5;
    public static GameObject MoreMenu;

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
            DifficultyMenu = transform.FindChild("DifficultyMenu").gameObject;
            NicknameMenu = transform.FindChild("NicknameEdit").gameObject;
            MoreMenu = transform.FindChild("MoreMenu").gameObject;

            var tutorial = transform.FindChild("Tutorial");
            Tutorial1 = tutorial.FindChild("Tutorial 1").gameObject;
            Tutorial2 = tutorial.FindChild("Tutorial 2").gameObject;
            Tutorial3 = tutorial.FindChild("Tutorial 3").gameObject;
            Tutorial4 = tutorial.FindChild("Tutorial 4").gameObject;
            Tutorial5 = tutorial.FindChild("Tutorial 5").gameObject;
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
        SetMenuCamera(NicknameMenu);
        SetMenuCamera(MoreMenu);

        SetMenuCamera(Tutorial1);
        SetMenuCamera(Tutorial2);
        SetMenuCamera(Tutorial3);
        SetMenuCamera(Tutorial4);
        SetMenuCamera(Tutorial5);
    }

    private void SetMenuCamera(GameObject menu)
    {
        var canvas = menu.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}

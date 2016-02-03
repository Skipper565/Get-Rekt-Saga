using UnityEngine;
using System.Collections;

public class DifficultySelector : MonoBehaviour {
    bool active = false;
    public MenuController MenuController;
    public GameObject MainMenu;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (active)
        {
            gameObject.GetComponent<Canvas>().sortingOrder = 50;
            //MainMenu.GetComponent<Canvas>().sortingOrder = -1;
        }
        else
        {
            gameObject.GetComponent<Canvas>().sortingOrder = -3;
            MainMenu.GetComponent<Canvas>().sortingOrder = 100;
        }
	}

    public void SelectEasy()
    {
        PlayerController.setDifficulty(GameDifficulty.EASY);
        active = false;
        MenuController.ClickedPlay();
        gameObject.GetComponent<Canvas>().sortingOrder = -3;
        gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
    }

    public void SelectNormal()
    {
        PlayerController.setDifficulty(GameDifficulty.MODERATE);
        active = false;
        MenuController.ClickedPlay();
        gameObject.GetComponent<Canvas>().sortingOrder = -3;
        gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
    }

    public void SelectHard()
    {
        PlayerController.setDifficulty(GameDifficulty.HARD);
        active = false;
        gameObject.GetComponent<Canvas>().sortingOrder = -3;
        gameObject.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
    }

    public void Display()
    {
        active = true;
        MenuController.ClickedPlay();
        gameObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
    }
}

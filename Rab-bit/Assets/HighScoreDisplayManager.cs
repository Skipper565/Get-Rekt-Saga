using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HighScoreDisplayManager : MonoBehaviour {

    public Text score1;
    public Text score2;
    public Text score3;
    public Text score4;
    public Text score5;

    public Text name1;
    public Text name2;
    public Text name3;
    public Text name4;
    public Text name5;

    private IList<Tuple<float, string>> localScoreboard;
    private IList<Tuple<float, string>> globalScoreboard;
    private Text[] scores;
    private Text[] names;

    public static int displayDifficulty = 1;
    public static bool runOnce = false;
    public static bool globalDisplay = false;

	// Use this for initialization
    void Awake()
    {
        
        runOnce = false;
        //IList<Tuple<float, string>> scoreboard = PlayerController.getLocalHighScore(displayDifficulty);
    }

	void Start () {
        runOnce = false;
        //IList<Tuple<float, string>> scoreboard = PlayerController.getLocalHighScore(displayDifficulty);
	}
	
	// Update is called once per frame
	void Update () {
        if (!runOnce)
        {
            runOnce = true;
            localScoreboard = PlayerController.getLocalHighScore(displayDifficulty);
        }
        

        Text[] scores = { score1, score2, score3, score4, score5 };
        Text[] names = { name1, name2, name3, name4, name5 };

        if (globalDisplay)
        {
            for (int i = 0; i < globalScoreboard.Count; i++)
            {
                names[i].text = globalScoreboard[i].Second;
                scores[i].text = "" + (int)globalScoreboard[i].First;
            }
        }
        else
        {
            for (int i = 0; i < localScoreboard.Count; i++)
            {
                names[i].text = localScoreboard[i].Second;
                scores[i].text = "" + (int)localScoreboard[i].First;
            }
        }        
	}

    private void loadScores()
    {
        if (globalDisplay)
        {
            globalScoreboard = PlayerController.getGlobalHighScore(displayDifficulty);
        }
        else
        {
            localScoreboard = PlayerController.getLocalHighScore(displayDifficulty);
        }
    }

    public void setDisplayEasyDifficulty()
    {
        displayDifficulty = 0;
        loadScores();
    }

    public void setDisplayNormalDifficulty()
    {
        displayDifficulty = 1;
        loadScores();
    }

    public void setDisplayHardDifficulty()
    {
        displayDifficulty = 2;
        loadScores();
    }

    public void setGlobalDisplay()
    {
        globalDisplay = true;
        loadScores();
    }

    public void setLocalDisplay()
    {
        globalDisplay = false;
        loadScores();
    }
}

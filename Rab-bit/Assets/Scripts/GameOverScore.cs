using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverScore : MonoBehaviour {

    public Text scoreText;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        scoreText.text = "" + PlayerPrefs.GetInt("lastScore");
	}

    /// <summary>
    /// Returns true if new highscore was achieved during last game
    /// </summary>
    /// <param name="autoReset">set true to reset new highscore monitor !if set to false USE ResetNewHighScore() explicitly</param>
    /// <returns></returns>
    public static bool IsNewHighScore(bool autoReset)
    {
        if (PlayerPrefs.GetInt("newHighScore") == 1)
        {
            if (autoReset) ResetNewHighScore();
            return true;
        }
        else return false;
    }

    public static void ResetNewHighScore()
    {
        PlayerPrefs.SetInt("newHighScore", 0);
    }
}

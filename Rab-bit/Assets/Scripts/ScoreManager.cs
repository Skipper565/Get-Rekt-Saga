using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {

    public Text scoreText;
    public float distanceModifier;

    public static float distance;
    public static float score;

    public static int scoreFromPowerUps;

	// Use this for initialization
	void Start () {
        scoreText.text = "0";

	    distance = 0;
	    score = 0;
        scoreFromPowerUps = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    distance = Camera.main.transform.position[0] * distanceModifier;

        if (PowerUpScore.collected)
        {
            scoreFromPowerUps += PowerUpScore.value;
            PowerUpScore.collected = false;
        }


        score = (distance + scoreFromPowerUps) * PlayerController.scoreCoeficient;

        scoreText.text = "" + (int)score;
	}
}

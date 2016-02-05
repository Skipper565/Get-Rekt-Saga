using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Assets.Scripts;
using UnityEngine.UI;

public enum GameDifficulty
{
    EASY, MODERATE, HARD
}

//We are sad because we are working with .NET 3.5 so we have to find wheel one more time :(
//Obsolete with .NET 4
public class Tuple<T1, T2>
{
    public T1 First { get; private set; }
    public T2 Second { get; private set; }
    internal Tuple(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }
}

public static class Tuple
{
    public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
    {
        var tuple = new Tuple<T1, T2>(first, second);
        return tuple;
    }
}

public class DifficultyScore
{
    public List<Tuple<float, string>> score = new List<Tuple<float, string>>();
}

public class PlayerController : MonoBehaviour {
    
    // MOVEMENT
    public float fallSpeedLimit;
    public float collisionTolerance;
    public float diveLength;
	public float jumpVelocity;
    public int jumpCountLimit;
	public float horizontalVelocity;
    public float waveHopVelocity;

    public static int jumpCount = 0;

    // GAME CONTROL
    GameManager gameManager;
    public bool pauseOnCollide;

    // POWERUPS
    public static bool swapGravity = false;
    public PowerUpJump powerUpJump;

    // DIFFICULTY
    public static GameDifficulty gameDif = GameDifficulty.HARD;
    public static float scoreCoeficient = 1;

    //PLAYER CONTROL
    private KeyCode keyW = KeyCode.W;
    private KeyCode keyUp = KeyCode.UpArrow;
    private KeyCode keyS = KeyCode.S;
    private KeyCode keyDown = KeyCode.DownArrow;
    private KeyCode restartKey;
    private KeyCode quitKey;

    //EFFECTS

    public GameObject waterDrops;
    public GameObject bloodDrops;

    //AUDIO
    private AudioSource[] jumpSounds;
    private AudioSource ploppySound;
    private AudioSource gameOverSound;
	private AudioSource powerUpSound;

	//ANIMATION
	private GameObject jumpHudOne;
	private GameObject jumpHudTwo;
	private GameObject jumpHudThree;
	private GameObject jumpHudFour;
	private GameObject diveAnim;
	private GameObject jumpGlowAnim;
	private GameObject scoreGlowAnim;


    //OTHER
    private Rigidbody2D rb;
    private DateTime deathTime;
    private float screenSplit;
	private int bars; // number of jump hud bars to be shown
    private Vector2 previousVelocity = new Vector2(0,0);

    public static string nickName = "default";
    public GameObject guts;
    public int gutsSpeed;

    public static int numberOfTopScores = 5;

    public static string urlGet = "http://grs.pe.hu/app/loadScore.php?difficulty=";
    public static string urlSet = "http://grs.pe.hu/app/saveScore.php?";

    private static DifficultyScore[] localHighScore = { new DifficultyScore(), new DifficultyScore(), new DifficultyScore(), new DifficultyScore() };
    private static DifficultyScore[] globalHighScore = { new DifficultyScore(), new DifficultyScore(), new DifficultyScore(), new DifficultyScore() };

    //public static float[] localHighScore = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
    //public static float[] globalHighScore = {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};

    void Awake()
    {
        //
        //getGlobalHighScores();
        //PlayerPrefs.DeleteAll();

        for (int i = 0; i < Enum.GetNames(typeof(GameDifficulty)).Length; i++)
        {
            float tempScore;
            string tempNick;

            string debugLog = "LOCAL " + ((GameDifficulty)i).ToString();

            localHighScore[i].score = new List<Tuple<float, string>>();

            for (int scorePos = 0; scorePos < numberOfTopScores; scorePos++)
            {
                tempScore = PlayerPrefs.GetFloat("scoreF_" + ((GameDifficulty)i).ToString() + "_" + scorePos);
                tempNick = PlayerPrefs.GetString("scoreS_" + ((GameDifficulty)i).ToString() + "_" + scorePos);

                localHighScore[i].score.Add(new Tuple<float, string>(tempScore, tempNick));

                debugLog += " " + tempNick + "_" + tempScore + " " ;
            }

#if UNITY_EDITOR
            //Debug.ClearDeveloperConsole();

            //Debug.Log("On start:" + localHighScore[1].score.Count);
            Debug.Log(debugLog);

            //debugLog = "GLOBAL " + ((GameDifficulty)i).ToString();

            //for (int scorePos = 0; scorePos < numberOfTopScores; scorePos++)
            //{
            //    tempScore = globalHighScore[i].score[scorePos].First;
            //    tempNick = globalHighScore[i].score[scorePos].Second;

            //    debugLog += " " + tempNick + "_" + tempScore + " ";
            //}

            //Debug.Log(debugLog);
#endif
        }
    }

    // Use this for initialization
    void Start()
    {
        gameManager = GameManager.Instance;
        bloodDrops.SetActive(false);
        waterDrops.SetActive(false);

		diveAnim = GameObject.Find("diveAnim");
		diveAnim.SetActive(false);

		jumpHudOne = GameObject.Find("One");
		jumpHudTwo = GameObject.Find("Two");
		jumpHudThree = GameObject.Find("Three");
		jumpHudFour = GameObject.Find("Four");

		jumpGlowAnim = GameObject.Find("glowingWheelJumps");
		scoreGlowAnim = GameObject.Find("glowingWheelScore");

		jumpGlowAnim.SetActive(false);
		scoreGlowAnim.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(horizontalVelocity, 0));
		rb.freezeRotation = true;
		
		jumpSounds = new AudioSource[5];
        var audioSources = GetComponents<AudioSource>();
        for (int i = 0; i < 5; i++)
        {
            jumpSounds[i] = audioSources[i];
        }

        ploppySound = audioSources[5];
        gameOverSound = audioSources[6];
		powerUpSound = audioSources[7];

        screenSplit = Camera.main.pixelWidth/2;

#if UNITY_ANDROID
        restartKey = KeyCode.Escape;
        quitKey = KeyCode.Space;
#else
        restartKey = KeyCode.Space;
        quitKey = KeyCode.Escape;
#endif
    }

    void FixedUpdate()
    {
        previousVelocity = rb.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        // deprecated
//#if UNITY_EDITOR
//        if (Input.GetKeyDown(KeyCode.J))
//        {
//            setDifficulty(GameDifficulty.EASY);
//        }
//        else if (Input.GetKeyDown(KeyCode.K))
//        {
//            setDifficulty(GameDifficulty.MODERATE);
//        }
//        else if (Input.GetKeyDown(KeyCode.L))
//        {
//            setDifficulty(GameDifficulty.HARD);
//        }
//#endif

        if (gameManager.gameState != GameState.GameOver)
        {
            //moveForward
            if (rb.velocity.x != horizontalVelocity)
            {
                rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y);
            }

            if (swapGravity)
            {
                rb.gravityScale *= -1;
                rb.velocity = new Vector2(rb.velocity.x, 0);

                if (rb.gravityScale > 0)
                {
                    transform.up = Vector2.up;
                    keyW = KeyCode.W;
                    keyUp = KeyCode.UpArrow;
                    keyS = KeyCode.S;
                    keyDown = KeyCode.DownArrow;
                }
                else
                {
                    // Inverse keys (W, S) and transform vector 'up'
                    transform.up = Vector2.down;
                    keyW = KeyCode.S;
                    keyUp = KeyCode.DownArrow;
                    keyS = KeyCode.W;
                    keyDown = KeyCode.UpArrow;
                }

                swapGravity = false;
            }

            //handle user touch input
            if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                if (Input.touches[0].position.x < screenSplit)
                {
                    if (rb.gravityScale > 0)
                    {
                        moveUp();
						// for transitions between animation states
						if (GetComponent<Animator>().GetBool("InJumpOne") == false && GetComponent<Animator>().GetBool("InJumpTwo") == false) {
							GetComponent<Animator>().SetBool("InJumpOne", true);
						}
						else if (GetComponent<Animator>().GetBool("InJumpOne") == true && GetComponent<Animator>().GetBool("InJumpTwo") == false) {
							GetComponent<Animator>().SetBool("InJumpOne", false);
							GetComponent<Animator>().SetBool("InJumpTwo", true);
						}
						else if (GetComponent<Animator>().GetBool("InJumpOne") == false && GetComponent<Animator>().GetBool("InJumpTwo") == true) {
							GetComponent<Animator>().SetBool("InJumpOne", true);
							GetComponent<Animator>().SetBool("InJumpTwo", false);
						}
                    }
                    else 
                    {
                        moveDown();
                    }
                }
                else
                {
                    if (rb.gravityScale > 0)
                    {
                        moveDown();
                    }
                    else
                    {
                        moveUp();
						// for transitions between animation states
						if (GetComponent<Animator>().GetBool("InJumpOne") == false && GetComponent<Animator>().GetBool("InJumpTwo") == false) {
							GetComponent<Animator>().SetBool("InJumpOne", true);
						}
						else if (GetComponent<Animator>().GetBool("InJumpOne") == true && GetComponent<Animator>().GetBool("InJumpTwo") == false) {
							GetComponent<Animator>().SetBool("InJumpOne", false);
							GetComponent<Animator>().SetBool("InJumpTwo", true);
						}
						else if (GetComponent<Animator>().GetBool("InJumpOne") == false && GetComponent<Animator>().GetBool("InJumpTwo") == true) {
							GetComponent<Animator>().SetBool("InJumpOne", true);
							GetComponent<Animator>().SetBool("InJumpTwo", false);
						}
                    }
                }
            }

            //handle user key input
            if (!(Input.GetKey(keyW) && Input.GetKey(keyS)) || !(Input.GetKey(keyUp) && Input.GetKey(keyDown)))
            {
                if (Input.GetKeyDown(keyW) || Input.GetKeyDown(keyUp))
                {
                    moveUp();
					// for transitions between animation states
					if (GetComponent<Animator>().GetBool("InJumpOne") == false && GetComponent<Animator>().GetBool("InJumpTwo") == false) {
						GetComponent<Animator>().SetBool("InJumpOne", true);
					}
					else if (GetComponent<Animator>().GetBool("InJumpOne") == true && GetComponent<Animator>().GetBool("InJumpTwo") == false) {
						GetComponent<Animator>().SetBool("InJumpOne", false);
						GetComponent<Animator>().SetBool("InJumpTwo", true);
					}
					else if (GetComponent<Animator>().GetBool("InJumpOne") == false && GetComponent<Animator>().GetBool("InJumpTwo") == true) {
						GetComponent<Animator>().SetBool("InJumpOne", true);
						GetComponent<Animator>().SetBool("InJumpTwo", false);
					}
                }
                else if (Input.GetKeyDown(keyS) || Input.GetKeyDown(keyDown))
                {
                    moveDown();
                }
            }

            //limit fall speed
            if (rb.velocity.y < fallSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, fallSpeedLimit);
            }
        }
        // Disabled for now because of the menu
        /*else if ((Input.touchCount > 0 || Input.anyKeyDown) && DateTime.Now.Subtract(deathTime).TotalSeconds > 0.5)
        {
            Restart();
        }*/

        // Space restarts the game
        if (Input.GetKeyDown(restartKey))
        {
            //Restart();
            gameManager.SetGameState(GameState.Playing);
        }
        // Escape quits the game
        else if (Input.GetKeyDown(quitKey))
        {
            // Application.Quit() is ignored in the editor or the web player.
            Application.Quit();

            // Force the editor to quit the game
            //UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    void moveUp()
    {
        // ignore jump if limit was exceeded
        if (jumpCount < jumpCountLimit || (PowerUpJump.collected && jumpCount < jumpCountLimit + 1))
        {
            if (jumpCount == jumpCountLimit)
            {
				jumpGlowAnim.SetActive(false); // disable glowing animation of jump hud
                PowerUpJump.collected = false;
            }

            // before jump - especially for jumping in air - we set current y velocity to zero, so every jump has same height when force is applied
            rb.velocity = new Vector2(rb.velocity.x, 0);

            // Inverse jump velocity if the gravity is inversed as well
            var jumpVelocityGravityDirection = rb.gravityScale > 0 ? jumpVelocity : -jumpVelocity;

            rb.AddForce(new Vector2(0, jumpVelocityGravityDirection));

            jumpSounds[UnityEngine.Random.Range(0, 4)].Play();

            jumpCount++;

			bars = jumpCountLimit-jumpCount;
			if (PowerUpJump.collected)
				bars += 1;
			switch (bars) {
			case 0:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 1:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 2:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 3:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 4:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				break;
			default:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			}
            //Debug.Log("JumpCount: " + jumpCount);
        }          
    }

    void moveDown()
    {
        //Debug.Log("Gravity direction: " + (Vector3.up == transform.up ? "Down" : "Up"));
        var distanceToCollider = Physics2D.Raycast(transform.position, -transform.up).distance;
        var targetCollider = Physics2D.Raycast(transform.position, -transform.up).collider;

        jumpSounds[UnityEngine.Random.Range(0, 4)].Play();

        // If distance to collider is smaller than our usual dive, teleport to the collider, no further.
        if (distanceToCollider <= diveLength && targetCollider.tag.Contains("PowerUp"))
        {
            // Uncomment this for determining distance of the player and the collider. Set value collisionTolerance accordingly.
            Debug.Log("Move down: Collider: " + targetCollider.name + " Distance: " + distanceToCollider 
                + " In the gravity direction: " + (rb.gravityScale > 0 ? "Down" : "Up"));

            if (distanceToCollider <= collisionTolerance)
            {
                // Do nothing - that prevents "jumping" into the floor.
            }
            else
            {
                // Else move player to the collider
                transform.Translate(Vector3.down * distanceToCollider);
            }
        }
        else
        {
            if (targetCollider.tag == "JumpPowerUp")
            {
                powerUpJump.Collect();
            }
            
            // when diving, player´s position changes discretely (but visually with continuous motion animation)
            transform.Translate(Vector3.down * diveLength);
			diveAnim.SetActive(false);
			diveAnim.SetActive(true);
        }

        // set vertical velocity to zero so player slowly starts to fall after dive action
        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        // Pause game on collision with tagged object
        if (coll.gameObject.tag == "NotToTouch" && pauseOnCollide)
		{
            bloodDrops.SetActive(true);
            bloodDrops.GetComponent<Animator>().Play("waterDrop");
			jumpGlowAnim.SetActive(false);
			scoreGlowAnim.SetActive(false);

            GameObject.Find("squish").transform.Translate(new Vector3(0,0,90));
            
            //bloodDrops.GetComponent<Animator>().Play("waterDrops");

            if (gameManager.gameState != GameState.GameOver)
            {
                gameManager.SetGameState(GameState.GameOver);
                if (!waterDrops.activeSelf)
                {
                    gameOverSound.PlayOneShot(gameOverSound.clip);
                }
            }
            
            deathTime = DateTime.Now;

			GetComponent<Animator>().SetBool("IsCrashed",true);
			GetComponent<Animator>().SetBool("InJumpOne",false);
			GetComponent<Animator>().SetBool("InJumpTwo",false);

            //Handle and save score
            nickName = PlayerPrefs.GetString("playerName");
            if (nickName == "")
            {
                nickName = "Bunny";
            }

            Splat();

            saveScore();
        }

        //Debug.Log("Collision with " + coll.gameObject.tag + " " + coll.gameObject.ToString());

        // Reset jump counter if colliding with floor
        if ((coll.gameObject.tag == "BarrierBottom" && rb.gravityScale > 0)
            || (coll.gameObject.tag == "BarrierTop" && rb.gravityScale <= 0)
            )
        {
            jumpCount = 0;
			GetComponent<Animator>().SetBool("InJumpOne",false);
			GetComponent<Animator>().SetBool("InJumpTwo",false);

			bars = jumpCountLimit-jumpCount;
			if (PowerUpJump.collected)
				bars += 1;
			switch (bars) {
			case 0:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 1:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 2:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 3:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 4:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				break;
			default:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			}

            //if (coll.gameObject.name.StartsWith("SeaWaves"))
            //{
            //    rb.AddForce(new Vector2(0, waveHopVelocity));

            //    //TODO add wave splash sound 
            //    waterDrops.SetActive(true);
            //    waterDrops.GetComponent<Animator>().Play("waterDrop");

            //}
        }

    }

	void OnTriggerEnter2D(Collider2D coll)
	{
		// if player touches water play splash animation
		if (coll.gameObject.tag == "Splash")
		{
			waterDrops.SetActive(true);
			waterDrops.GetComponent<Animator>().Play("waterDrop");
            ploppySound.PlayOneShot(ploppySound.clip);
		}

		// if player gets powerup play animation and sound
		if (coll.gameObject.tag == "JumpPowerUp" || coll.gameObject.tag == "ScorePowerUp")
		{
			powerUpSound.PlayOneShot(powerUpSound.clip);
		}

		// refresh jump hud when hitting jump powerup
		if (coll.gameObject.tag == "JumpPowerUp")
		{
			
			jumpGlowAnim.SetActive(false); // ensuring animation object is inactive so it can be activated below and therefore animation is palyed
			jumpGlowAnim.SetActive(true);

			bars = jumpCountLimit-jumpCount+1;
			switch (bars) {
			case 0:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 1:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 2:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 3:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			case 4:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0.6f);
				break;
			default:
				jumpHudOne.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudTwo.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudThree.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				jumpHudFour.GetComponent<Image>().color = new Color(1f,1f,1f,0f);
				break;
			}
		}
		// refresh jump hud when hitting jump powerup
		if (coll.gameObject.tag == "ScorePowerUp")
		{
			scoreGlowAnim.SetActive(false);
			scoreGlowAnim.SetActive(true);
		}
	}

    void LateUpdate()
    {
        bloodDrops.transform.position = gameObject.transform.position;
		waterDrops.transform.position = new Vector3(gameObject.transform.position.x,-4.3f,gameObject.transform.position.z);
    }

    private void saveScore()
    {
        int originalTopHighScores = localHighScore.GetHashCode();

        PlayerPrefs.SetInt("lastScore", (int)ScoreManager.score);

        localHighScore[(int)gameDif].score.Add(new Tuple<float, string>(ScoreManager.score, nickName));
        localHighScore[(int)gameDif].score.Sort(
            delegate(Tuple<float, string> x, Tuple<float, string> y)
            {
                return x.First.CompareTo(y.First);
            });
        localHighScore[(int)gameDif].score.Reverse();
        localHighScore[(int)gameDif].score.RemoveAt(numberOfTopScores);

        for (int i = 0; i < numberOfTopScores; i++)
        {
            PlayerPrefs.SetFloat("scoreF_" + gameDif.ToString() + "_" + i, localHighScore[(int)gameDif].score[i].First);
            PlayerPrefs.SetString("scoreS_" + gameDif.ToString() + "_" + i, localHighScore[(int)gameDif].score[i].Second);
        }

        if (originalTopHighScores != localHighScore.GetHashCode())
        {
            PlayerPrefs.SetInt("newHighScore", 1);
        }

        WWW www = new WWW(urlSet + "score=" + (int)ScoreManager.score + "&nickname=" + nickName + "&difficulty=" + (int)gameDif);
    }

    public void Restart()
    {
        //gameOverMenu.enabled = false;
        jumpCount = 0;
        Application.LoadLevel(0);
    }

    public static void setDifficulty(GameDifficulty gd)
    {
        PlayerController.gameDif = gd;

        switch (gd)
        {
            case GameDifficulty.EASY:
                Time.timeScale = 0.5f;
                scoreCoeficient = 0.5f;
                break;
            case GameDifficulty.MODERATE:
                Time.timeScale = 0.75f;
                scoreCoeficient = 0.75f;
                break;
            case GameDifficulty.HARD:
                Time.timeScale = 1;
                scoreCoeficient = 1f;
                break;
            default:
                break;
        }
    }

    public static IList<Tuple<float, string>> getLocalHighScore(int i)
    {
        return localHighScore[i].score.AsReadOnly();
    }

    /// <summary>
    /// submethod of getGlobalHighScores, loads only specified difficulty and returns it
    /// </summary>
    /// <param name="i">difficulty to be loaded</param>
    /// <returns></returns>
    public static IList<Tuple<float, string>> getGlobalHighScore(int i)
    {
        WWW www = new WWW(urlGet + i);
        globalHighScore[i].score = new List<Tuple<float, string>>();

        while (!www.isDone)
        {
            WaitForOneSec();
        }

        string content = www.text;

        //Debug.Log(content);

        string[] scoresTemp = content.Split('?');

        foreach (var item in scoresTemp)
        {
            string[] scoreTemp = item.Split(',');
            if (scoreTemp[0] != "")
            {
                globalHighScore[i].score.Add(new Tuple<float, string>(float.Parse(scoreTemp[0]), scoreTemp[1]));
            }
        }

        while (globalHighScore[i].score.Count < numberOfTopScores)
        {
            globalHighScore[i].score.Add(new Tuple<float, string>(0, ""));
        }

        return globalHighScore[i].score.AsReadOnly();
    }

    /// <summary>
    /// Use this to load globalHighScore variable
    /// </summary>
    public static void getGlobalHighScores()
    {
        for (int i = 0; i < Enum.GetNames(typeof(GameDifficulty)).Length; i++)
        {
            getGlobalHighScore(i);
        }
    }

    public static IEnumerator WaitForOneSec()
    {
        yield return new WaitForSeconds(1);
    }

    public void Splat()
    {
        System.Random rnd = new System.Random();
        int randNum;

        foreach (Transform child in guts.transform)
        {
            randNum = rnd.Next(360);

            Vector2 newpos = new Vector2(1.2f * (float)Math.Cos(randNum),1.2f * (float)Math.Sin(randNum));
            Vector2 velocity = newpos + previousVelocity * 2f;

            Rigidbody2D childRB = child.gameObject.GetComponent<Rigidbody2D>();

            childRB.AddForce(velocity * gutsSpeed);
            child.position = transform.position + new Vector3(newpos.x, newpos.y, 0);
            
        }
    }

    public void ResetSplat()
    {

    }
}

using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    
    public float fallSpeedLimit;

	public float diveLength;
	public float jumpVelocity;
    public int jumpCountLimit;
	public float horizontalVelocity;

    public bool pauseOnCollide;

    private AudioSource audio;

    private Rigidbody2D rb;

    public static bool swapGravity = false;
    public static bool paused = false;
    private KeyCode keyW = KeyCode.W;
    private KeyCode keyUp = KeyCode.UpArrow;
    private KeyCode keyS = KeyCode.S;
    private KeyCode keyDown = KeyCode.DownArrow;

    public float yScreenCoo;
    private int jumpCount = 0;

    // Use this for initialization
    void Start()
    {
		rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(horizontalVelocity, 0));
		rb.freezeRotation = true;

        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused)
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

            //handle user input
            if (!(Input.GetKey(keyW) && Input.GetKey(keyS)) || !(Input.GetKey(keyUp) && Input.GetKey(keyDown)))
            {
                if (Input.GetKeyDown(keyW) || Input.GetKeyDown(keyUp))
                {
                    // ignore jump if limit was exceeded
                    if (jumpCount < jumpCountLimit)
                    {
                        // before jump - especially for jumping in air - we set current y velocity to zero, so every jump has same height when force is applied
                        rb.velocity = new Vector2(rb.velocity.x, 0);

                        // Inverse jump velocity if the gravity is inversed as well
                        var jumpVelocityGravityDirection = rb.gravityScale > 0 ? jumpVelocity : -jumpVelocity;

                        rb.AddForce(new Vector2(0, jumpVelocityGravityDirection));
                        audio.Play();

                        jumpCount++;
                    }                    
                }
                else if (Input.GetKeyDown(keyS) || Input.GetKeyDown(keyDown))
                {
                    //Debug.Log("Gravity direction: " + (Vector3.up == transform.up ? "Down" : "Up"));
                    var distanceToCollider = Physics2D.Raycast(transform.position, -transform.up).distance;

                    // If distance to collider is smaller than our usual dive, teleport to the collider, no further.
                    if (distanceToCollider <= diveLength)
                    {
                        //Debug.Log("Collider: " + Physics2D.Raycast(transform.position, -transform.up).collider.name 
                        //    + " Distance: " + distanceToCollider + " In the gravity direction: " + (rb.gravityScale > 0 ? "Down" : "Up"));

                        transform.Translate(Vector3.down * distanceToCollider);    // vector.down
                    }
                    else
                    {
                        // when diving, player´s position changes discretely (but visually with continuous motion animation)
                        transform.Translate(Vector3.down * diveLength);
                    }

                    // set vertical velocity to zero so player slowly starts to fall after dive action
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
            }

            //limit fall speed
            if (rb.velocity.y < fallSpeedLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, fallSpeedLimit);
            }
        }

        // Space restarts the game
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = false;
            Application.LoadLevel(0);
        }
        // Escape quits the game
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Application.Quit() is ignored in the editor or the web player.
            Application.Quit();

            // Force the editor to quit the game
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        // Pause game on collision with tagged object
        if (coll.gameObject.tag == "NotToTouch" && pauseOnCollide)
        {
            paused = true;
        }

        Debug.Log(jumpCount);
        Debug.Log(coll.gameObject.tag + " " + coll.gameObject.transform.position.y + " " + yScreenCoo + " " + rb.gravityScale);

        // Reset jump counter if colliding with floor
        if ((coll.gameObject.tag == "BarrierBottom" && coll.gameObject.transform.position.y == yScreenCoo && rb.gravityScale > 0)
            || (coll.gameObject.tag == "BarrierTop" && coll.gameObject.transform.position.y == -yScreenCoo && rb.gravityScale <= 0)
            )
        {
            jumpCount = 0;
        }
    }
}

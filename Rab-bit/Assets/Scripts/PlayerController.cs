using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    
    public float fallSpeedLimit;

	public float diveLength;
	public float jumpVelocity;
	public float horizontalVelocity;

    private AudioSource audio;

    private Rigidbody2D rb;

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
        //moveForward
        if (rb.velocity.x != horizontalVelocity)
        {
            rb.velocity = new Vector2(horizontalVelocity, rb.velocity.y); 
        }

        //handle user input
        if (!(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)))
        {
			if(Input.GetKeyDown(KeyCode.W))
			{
				rb.velocity = new Vector2(rb.velocity.x, 0);  // before jump - especially for jumping in air - we set current y velocity to zero, so every jump has same height when force is applied
				rb.AddForce(new Vector2(0,jumpVelocity));
                audio.Play();
			}
			else if(Input.GetKeyDown(KeyCode.S))
			{
                var distanceToCollider = Physics2D.Raycast(transform.position, Vector2.down).distance;

                // If distance to collider is smaller than our usual dive, teleport to the collider, no further.
                if (distanceToCollider <= diveLength)
                {
                    //Debug.Log("Collider: " + Physics2D.Raycast(transform.position, Vector2.down).collider.name 
                    //    + " Distance: " + distanceToCollider);

                    transform.Translate(Vector2.down * distanceToCollider);
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                }
                else
                {
                    //Debug.Log("No obstacle!");

                    // when diving, player´s position changes discretely (but visually with continuous motion animation)
                    transform.Translate(Vector2.down * diveLength);

                    // set vertical velocity to zero so player slowly starts to fall after dive action
                    rb.velocity = new Vector2(rb.velocity.x, 0); 
                } 
			}
            // Space restarts the game
            else if (Input.GetKeyDown(KeyCode.Space))
            {
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

        //limit fall speed
        if (rb.velocity.y < fallSpeedLimit)
        {
            rb.velocity = new Vector2(rb.velocity.x, fallSpeedLimit);
        }

    }
}

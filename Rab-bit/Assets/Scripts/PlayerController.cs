using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    
    public float fallSpeedLimit;

	public float diveLength;
	public float jumpVelocity;
	public float horizontalVelocity;

    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
		rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(horizontalVelocity, 0));
		rb.freezeRotation = true;
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
			}
			else if(Input.GetKeyDown(KeyCode.S))
			{
				transform.Translate(Vector2.down * diveLength); // when diving, player´s position changes discretely (but visually with continuous motion animation)
				rb.velocity = new Vector2(rb.velocity.x, 0); // set vertical velocity to zero so player slowly starts to fall after dive action
			}

        }

        //limit fall speed
        if (rb.velocity.y < fallSpeedLimit)
        {
            rb.velocity = new Vector2(rb.velocity.x, fallSpeedLimit);
        }

    }
}

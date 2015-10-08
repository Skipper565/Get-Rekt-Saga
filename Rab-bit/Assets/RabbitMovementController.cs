using UnityEngine;
using System.Collections;

public class RabbitMovementController : MonoBehaviour {
    //increase of jump height
    public float upModifier;
    //limit of jump height
    public float upLimit;
    public float upMultiplier;
    //minimal jump
    public float minimalUp;

    public float downModifier;
    public float fallLimit;

    public float gravityAttenuation;
    public float jumpPenalzationModifier;

    public float startupSpeed;
    public float accelerationForward;

    private float verticalForce = 0;
    private Rigidbody2D rb;
    private float minimalHorizontalVelocity;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        minimalHorizontalVelocity = startupSpeed;
        rb.AddForce(new Vector2(startupSpeed, 0));
    }

    // Update is called once per frame
    void Update()
    {
        //moveForward
        if (rb.velocity.x < minimalHorizontalVelocity)
        {
            rb.velocity = new Vector2(minimalHorizontalVelocity, rb.velocity.y);
        }

        //slowly increase game speed
        minimalHorizontalVelocity += accelerationForward;

        //weaken gravity
        rb.AddForce(new Vector2(0, gravityAttenuation));

        //handle user input
        if (!(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)))
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                verticalForce = minimalUp;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                verticalForce += upModifier;
                if (verticalForce > upLimit)
                {
                    verticalForce = upLimit;
                }

                //increase horizontal speed as a penalization
                minimalHorizontalVelocity += accelerationForward * jumpPenalzationModifier;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                transform.Translate(Vector2.down * downModifier);
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
            else
            {
                //jump if there is any force gathered
                if (verticalForce != 0 && rb.velocity.y < 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(new Vector2(0, verticalForce * upMultiplier));
                    verticalForce = 0;
                }
            }
        }

        //limit fall speed
        if (rb.velocity.y < fallLimit)
        {
            rb.velocity = new Vector2(rb.velocity.x, fallLimit);
        }
    }
}

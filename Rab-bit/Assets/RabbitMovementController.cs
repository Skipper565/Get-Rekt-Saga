using UnityEngine;
using System.Collections;

public class RabbitMovementController : MonoBehaviour {

    public float upAndDownModifier;
    public float upAndDownLimit;

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

        minimalHorizontalVelocity += accelerationForward;

        //weaken gravity
        rb.AddForce(new Vector2(0, gravityAttenuation));

        //handle user input
        if (!(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)))
        {
            if (Input.GetKey(KeyCode.W))
            {
                verticalForce += upAndDownModifier;
                if (verticalForce > upAndDownLimit)
                {
                    verticalForce = upAndDownLimit;
                }

                //increase horizontal speed as a penalization
                minimalHorizontalVelocity += accelerationForward * jumpPenalzationModifier;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                verticalForce -= upAndDownModifier;
                if (verticalForce < (-0.7f) * upAndDownLimit)
                {
                    verticalForce = (-0.7f) * upAndDownLimit;
                }
            }
            else
            {
                rb.AddForce(new Vector2(0, verticalForce));
                verticalForce = 0;
            }
        }
    }
}

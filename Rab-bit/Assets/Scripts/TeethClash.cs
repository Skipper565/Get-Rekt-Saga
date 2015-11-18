using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;

public class TeethClash : MonoBehaviour
{
    public Transform otherTooth;
    private Transform PlayerTransform;

    private float speed;
    private float delay;
    private bool collided;

    private Transform upperTooth;
    private Transform lowerTooth;
    private Bounds upperBounds;
    private Bounds lowerBounds;
    private int upperToothLowestY;
    private int lowerToothHighestY;
    private float sizeThisTooth;
    private float sizeOtherTooth;


    // Use this for initialization
    void Start()
    {
        PlayerTransform = GameObject.Find("PlayerSprite").transform;

        // This tooth is upper, the other one lower
        if (transform.position.y > otherTooth.position.y)
        {
            upperTooth = transform;
            lowerTooth = otherTooth.transform;

            upperBounds = upperTooth.GetComponent<Renderer>().bounds;
            lowerBounds = lowerTooth.GetComponent<Renderer>().bounds;

            sizeThisTooth = upperBounds.min.y;
            sizeOtherTooth = lowerBounds.max.y;
        }
        // This tooth is lower, the other one upper
        else
        {
            upperTooth = otherTooth.transform;
            lowerTooth = transform;

            upperBounds = upperTooth.GetComponent<Renderer>().bounds;
            lowerBounds = lowerTooth.GetComponent<Renderer>().bounds;

            sizeThisTooth = lowerBounds.max.y;
            sizeOtherTooth = upperBounds.min.y;
        }

        speed = 5f;
        delay = 1f;
        collided = false;
    }

    void Update()
    {
        // If they collided, return immediately
        if (collided)
        {
            return;
        }

        // If they collide (unable to find out so without RigidBody or RayCast), stop moving
        upperBounds = upperTooth.GetComponent<Renderer>().bounds;
        lowerBounds = lowerTooth.GetComponent<Renderer>().bounds;

        if (upperBounds.min.y <= lowerBounds.max.y)
        {
            collided = true;
            return;
        }

        // If the tooth is behind the player, move
        if (PlayerTransform.transform.position.x - delay >= transform.position.x)
        {
            MoveTooth();
        }
    }

    void MoveTooth()
    {
        var towardThisY = new Vector3(transform.position.x, transform.position.y - sizeThisTooth, transform.position.z);
        var towardOtherY = new Vector3(otherTooth.position.x, otherTooth.position.y - sizeOtherTooth, otherTooth.position.z);

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, towardThisY, step);
        otherTooth.position = Vector3.MoveTowards(otherTooth.position, towardOtherY, step);
    }

    public void ResetCollisionAnimation()
    {
        Start();
    }
}

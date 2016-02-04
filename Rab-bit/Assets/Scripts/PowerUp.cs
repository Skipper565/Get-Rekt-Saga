using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

    private float recycleOffset = 20.0f;
    
	protected virtual void Start()
    {
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	protected virtual void Update()
    {
	    if (gameObject.activeSelf)
	    {
            if (transform.localPosition.x + recycleOffset < Camera.main.transform.position[0])
            {
                // Position of powerUp behind the camera, deactivate it
                gameObject.SetActive(false);
                return;
            }
        }
    }

    public virtual void SpawnIfAvailable(Bounds bounds)
    {
        if (gameObject.activeSelf)
        {
            return;
        }

        var position = FindSpawnPosition(bounds);

        // Can happen if no suitable position is found within some iterations
        if (position != Vector2.zero)
        {
            transform.localPosition = position;
            gameObject.SetActive(true);
        }
    }

    /*void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Power up collected, prepare to get rekt!");
        gameObject.SetActive(false);

        collected = true;

        //PlayerController.swapGravity = true;
        //PlayerController.jumpCount = 0;
    }*/

    public Vector2 FindSpawnPosition(Bounds bounds)
    {
        const int maxIterations = 1000;
        int iterationCounter = 0;
        float x, y;
        bool collided = false;
        float radius = 0.5f;
        var player = GameObject.Find("Player").transform;

        do
        {
            x = Random.Range(bounds.min.x, bounds.max.x);
            y = Random.Range(bounds.min.y, bounds.max.y);

            collided = Physics2D.OverlapCircle(new Vector2(x, y), radius);

            // Do not spawn it behind player (and certain offset in front of him)
            if (player.position.x + 2 > x)
            {
                continue;
            }

            iterationCounter++;

        } while (collided && iterationCounter < maxIterations);

        if (collided)
        {
            return Vector2.zero;
        }

        return new Vector2(x, y);
    }
}

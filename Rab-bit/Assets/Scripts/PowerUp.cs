using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

    public Vector2 offset;
    public Vector2 rotationVelocity;
    public float recycleOffset;
    public float spawnChance;
    
	void Start () {
        // You use this method to either activate or deactivate an entire game object, not just a single component. 
        // Also, when deactivating a game object all its child game objects will be deactivated as well. 
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.localPosition.x + recycleOffset < Camera.main.transform.position[0])
        {
            // Position of powerUp behind the camera, deactivate it
            gameObject.SetActive(false);
            return;
        }
        transform.Rotate(rotationVelocity * Time.deltaTime);
    }

    public void SpawnIfAvailable(Vector2 position)
    {
        if (gameObject.activeSelf || spawnChance <= Random.Range(0f, 100f))
        {
            return;
        }

        transform.localPosition = position + offset;
        gameObject.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Power up collected! Prepare to be rekt.");
        gameObject.SetActive(false);

        PlayerController.swapGravity = true;
    }

    /*private void GameOver()
    {
        gameObject.SetActive(false);
    }*/
}

using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

    public GameObject player;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x + 10, transform.position.y, transform.position.z);
    }
}

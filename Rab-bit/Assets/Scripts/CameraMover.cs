using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour {

    public GameObject player;
	public float relativePlayerPosition; // 0 is most left screen position 1 is most right screen position
	private float xScreenCoo; // screen width in world coordinates (= not in pixels)

    // Use this for initialization
    void Start()
    {

		int xScreenPixel = Camera.main.pixelWidth; // get camera x pixel count
		xScreenCoo = Camera.main.ScreenToWorldPoint(new Vector2(xScreenPixel,0))[0]*2f; // get world coodrinate screen size using xScreenPixelWidth
//		Debug.Log(Camera.main.ScreenToWorldPoint(new Vector2(xScreenPixel,0)));
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = new Vector3(player.transform.position.x + xScreenCoo/2f - xScreenCoo*relativePlayerPosition , transform.position.y, transform.position.z);
    }
}

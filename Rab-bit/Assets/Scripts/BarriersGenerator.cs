using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Generates Pavilon walls (top, bottom) and creates two types of obstacles

// Warining - this solution has several limitations:
// cant handle very high speeds of players x movement
// prefabs of Pavilon walls have to be slightly longer than camera view along x axis

public class BarriersGenerator : MonoBehaviour {

	public GameObject obstacleOne;
	public GameObject obstacleTwo;
	public Queue<GameObject> obstacleList;
	public GameObject barrier;
	public float barrierYOffset; // 0 = no gap between barriers, 1 = whole y screen size is gap betwwen barriers
	public int xGenerationOffset; // area along x axis. when right border of camera is detected inside of this area, next barrier ahead is generated. When x generation offset is loo low and x speed too high, there is high risk of not detecting anything at all!
	private Vector3 barrierDimensions;
	private float lastOffset;
	private float xScreenCoo;
	private float yScreenCoo;
	private GameObject NowOnScreenTop; // reference to top barrier which is currently on screen
	private GameObject NowOnScreenBottom; // reference to bottom barrier which is currently on screen
	private GameObject nextBarrierTop;
	private GameObject nextBarrierBottom;
	private bool wasTriggered; // used in Update function when new barrier ahead should be instantiated
    public PowerUp powerUp;


	// Use this for initialization
	void Start () {

		wasTriggered = false; // helps recognise wheter next barriers ahead had been already generated or not (third if in update function)

		int xScreenPixel = Camera.main.pixelWidth; // get camera x pixel count
		int yScreenPixel = Camera.main.pixelHeight; // get camera y pixel count

		xScreenCoo = Camera.main.ScreenToWorldPoint(new Vector2(xScreenPixel,yScreenPixel))[0]; //pixels to world coodrinates
		yScreenCoo = Camera.main.ScreenToWorldPoint(new Vector2(xScreenPixel,yScreenPixel))[1];

		barrierDimensions = barrier.transform.localScale/2f;

		NowOnScreenTop = (GameObject) Instantiate(barrier, new Vector2(0, yScreenCoo*barrierYOffset + barrierDimensions[1]), Quaternion.identity); // create instance of barrier prefab and translate vertical according to barrier y offset parameter
		NowOnScreenTop.tag = "BarrierTop";
		NowOnScreenBottom = (GameObject) Instantiate(barrier, new Vector2(0, -yScreenCoo*barrierYOffset - barrierDimensions[1]), Quaternion.identity);
		NowOnScreenBottom.tag = "BarrierBottom";

		obstacleList = new Queue<GameObject>();
		obstacleList.Enqueue(new GameObject());
		obstacleList.Enqueue(new GameObject());

        //powerUp = new PowerUp();

//		Debug.Log(YScreenCoo*barrierOffset + barrierDimensions[1]);
//		Debug.Log(YScreenCoo);
//		Debug.Log(YScreenCoo*barrierOffset);
//		Debug.Log(barrierDimensions[1]);
//
//		Destroy(NowOnScreenTop, 4f);

	}
	
	// Update is called once per frame
	void Update () {
		if(NowOnScreenTop != null && NowOnScreenBottom != null)
		{
			// if vertical position of barrier was changed, change gap between barriers according to it
			if(barrierYOffset != lastOffset) 
			{
				foreach(GameObject b in GameObject.FindGameObjectsWithTag("BarrierTop"))
				{
					b.transform.position = new Vector3(b.transform.position.x, yScreenCoo*barrierYOffset + barrierDimensions[1],b.transform.position.z);
				}
				foreach(GameObject b in GameObject.FindGameObjectsWithTag("BarrierBottom"))
				{
					b.transform.position = new Vector3(b.transform.position.x, -yScreenCoo*barrierYOffset - barrierDimensions[1],b.transform.position.z);
				}
//				NowOnScreenTop.transform.position = new Vector3(0, yScreenCoo*barrierYOffset + barrierDimensions[1],0);
//				NowOnScreenBottom.transform.position = new Vector3(0, -yScreenCoo*barrierYOffset - barrierDimensions[1],0);
				lastOffset = barrierYOffset;
			}

			// if right camera border is almost at the end of the horizont of current barriers, generate next barriers ahead
			if(NowOnScreenTop.transform.position[0] + barrierDimensions[0] >= Camera.main.transform.position[0] + xScreenCoo && NowOnScreenTop.transform.localPosition[0] + barrierDimensions[0] - xGenerationOffset < Camera.main.transform.position[0] + xScreenCoo && !wasTriggered)
			{

				nextBarrierTop = (GameObject) Instantiate(barrier, new Vector2(NowOnScreenTop.transform.position[0] + barrierDimensions[0]*2f, yScreenCoo*barrierYOffset + barrierDimensions[1]), Quaternion.identity);
				nextBarrierTop.tag = "BarrierTop";
				nextBarrierBottom = (GameObject) Instantiate(barrier, new Vector2(NowOnScreenTop.transform.position[0] + barrierDimensions[0]*2f, -yScreenCoo*barrierYOffset - barrierDimensions[1]), Quaternion.identity);
				nextBarrierBottom.tag = "BarrierBottom";

                // Generate powerUp
                powerUp.SpawnIfAvailable(new Vector2(NowOnScreenTop.transform.position[0] + barrierDimensions[0] * 2f, yScreenCoo - 8));

				// generating obstacles here:
				obstacleList.Enqueue((GameObject) Instantiate(obstacleOne, new Vector2(NowOnScreenTop.transform.position[0] + barrierDimensions[0]*2f, 0), Quaternion.identity));
				obstacleList.Enqueue((GameObject) Instantiate(obstacleTwo, new Vector2(NowOnScreenTop.transform.position[0] + barrierDimensions[0], -2.5f), Quaternion.identity));

				wasTriggered = true;
			}

			// if the rightmost instantiated barrier´s right border passes leftmost border of the camera it Kills itsef and reference is switched to the next barrier
			if(NowOnScreenTop.transform.localPosition[0] + barrierDimensions[0] < Camera.main.transform.localPosition[0] - xScreenCoo)
			{
				// remove barriers behind camera
				Destroy(NowOnScreenTop);
				NowOnScreenTop = nextBarrierTop;
				Destroy(NowOnScreenBottom);
				NowOnScreenBottom = nextBarrierBottom;


				// remove obstacles behind camera
				if (obstacleList.Count != 0)
					Destroy(obstacleList.Dequeue());
				if (obstacleList.Count != 0)
					Destroy(obstacleList.Dequeue());

				wasTriggered = false;
			}
		}

	}
}

using UnityEngine;
using System.Collections;

public class jumpHud : MonoBehaviour {

	public int hudBars;
	// Use this for initialization
	void Start () {
		this.hudBars = 4;
		SetJumpHudBarsNumber(hudBars);
	}

	public void SetJumpHudBarsNumber(int i)
	{
		switch (i) {
		case 0:
			GameObject.Find("One").SetActive(false);
			GameObject.Find("Two").SetActive(false);
			GameObject.Find("Three").SetActive(false);
			GameObject.Find("Four").SetActive(false);
			break;
		case 1:
			GameObject.Find("One").SetActive(true);
			GameObject.Find("Two").SetActive(false);
			GameObject.Find("Three").SetActive(false);
			GameObject.Find("Four").SetActive(false);
			break;
		case 2:
			GameObject.Find("One").SetActive(true);
			GameObject.Find("Two").SetActive(true);
			GameObject.Find("Three").SetActive(false);
			GameObject.Find("Four").SetActive(false);
			break;
		case 3:
			GameObject.Find("One").SetActive(true);
			GameObject.Find("Two").SetActive(true);
			GameObject.Find("Three").SetActive(true);
			GameObject.Find("Four").SetActive(false);
			break;
		case 4:
			GameObject.Find("One").SetActive(true);
			GameObject.Find("Two").SetActive(true);
			GameObject.Find("Three").SetActive(true);
			GameObject.Find("Four").SetActive(true);
			break;
		}

	}
}

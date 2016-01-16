using UnityEngine;
using System.Collections;

public class SeawaveMovement : MonoBehaviour
{
    public float amplitude;
    public float frequency;
    public float angle;
    public float height;
    
	
	// Update is called once per frame
	void Update () {
        angle += frequency * Time.deltaTime;
        if (angle > 360) angle -= 360;

	    var newPosition = transform.localPosition;
        newPosition.y = amplitude * Mathf.Sin(angle * (Mathf.PI / 180)) + height;
	    transform.localPosition = newPosition;
	}
}

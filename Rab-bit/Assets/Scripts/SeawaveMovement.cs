using UnityEngine;
using System.Collections;

public class SeawaveMovement : MonoBehaviour
{
    public float amplitude;
    public float frequency;
    public float angle;
    
	
	// Update is called once per frame
	void Update () {
        angle += frequency * Time.deltaTime;
	    var newPosition = transform.localPosition;
        newPosition.x = amplitude * Mathf.Sin(angle * (Mathf.PI / 180))*0.1f;
		transform.localPosition = new Vector3(transform.localPosition.x+newPosition.x,transform.localPosition.y, transform.localPosition.z);
	}
}

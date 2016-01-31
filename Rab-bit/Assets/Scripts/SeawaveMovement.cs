using UnityEngine;
using System.Collections;

public class SeawaveMovement : MonoBehaviour
{
    public float amplitude;
    public float frequency;
    public float angle;

    private float previousTime;
    public float deltaTime { get; private set; }

    void Awake()
    {
        previousTime = Time.realtimeSinceStartup;
    }

    void Update() {
        // Cannot use Time.deltaTime: when timeScale = 0 -> deltaTime = 0, animation is broken.
        float realTime = Time.realtimeSinceStartup;
        deltaTime = realTime - previousTime;
        previousTime = realTime;

        angle += frequency * deltaTime;

        var newPosition = transform.localPosition;
        newPosition.x = amplitude * Mathf.Sin(angle * (Mathf.PI / 180)) * 0.1f;

        transform.localPosition = new Vector3(
            transform.localPosition.x + newPosition.x,
            transform.localPosition.y, 
            transform.localPosition.z);
	}
}

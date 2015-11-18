using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TeethBarrierGenerator : MonoBehaviour
{
    public List<GameObject> listOfTeethBarriers;

    // Now valid only for teeth; when adding another pavilon/obstacle with different length, adjust algorithm
    private float barrierLength;
    private GameObject firstBarrier;
    private GameObject lastBarrier;
    private Queue<GameObject> queueOfBarriers;
    private Dictionary<GameObject, InitialPosition> initialPosition;

    private float worldCoordX;

    // Area along x axis. When right border of camera is detected inside this area, next barrier ahead is generated.
    // When x generation offset is loo low and x speed too high, there is high risk of not detecting anything at all!
    public int xGenerationOffset;

    // Use this for initialization
    void Start ()
    {
        initialPosition = new Dictionary<GameObject, InitialPosition>();
        
        // Load all barriers and their initial position
        foreach (var barrier in listOfTeethBarriers)
        {
            barrier.SetActive(false);
            LoadInitialPosition(barrier);
        }
        firstBarrier = listOfTeethBarriers.First();

        // --- Any other pavilon barriers to be loaded here ---

        int screenPixelWidth = Camera.main.pixelWidth;
        int screenPixelHeight = Camera.main.pixelHeight;

        // World coordinates
        worldCoordX = Camera.main.ScreenToWorldPoint(new Vector2(screenPixelWidth, screenPixelHeight))[0];

        // Set first barrier and its length (only valid for teeth)
        firstBarrier.SetActive(true);
        queueOfBarriers = new Queue<GameObject>();
        queueOfBarriers.Enqueue(firstBarrier);

        var widestChild = firstBarrier.transform.Find("TeethGumTop").transform.Find("teethGumBottom");
        barrierLength = widestChild.GetComponent<Renderer>().bounds.max.x
                      - widestChild.GetComponent<Renderer>().bounds.min.x;

        // First barrier must be shifted a bit to the left
        var startingOffsetOfAxisX = -3f;

        firstBarrier.transform.Translate(new Vector2(Camera.main.transform.position[0] + startingOffsetOfAxisX, Camera.main.transform.position[1]));
        lastBarrier = firstBarrier;

        // Set second barrier just to be sure; generate next barriers with Update()
        GenerateBarrier();
    }

    // Update is called once per frame
    void Update () {
        xGenerationOffset = 5;

        if ((lastBarrier.transform.position.x >= Camera.main.transform.position[0] + worldCoordX)
            && (lastBarrier.transform.localPosition.x - xGenerationOffset < Camera.main.transform.position[0] + worldCoordX))
        {
            GenerateBarrier();
        }

        if (firstBarrier.transform.position.x < Camera.main.transform.position[0] - (worldCoordX * 2.5))
        {
            DeactivateBarrier();
        }
    }

    private float getNextPositionX()
    {
        return lastBarrier.transform.position.x + barrierLength;
    }

    private float getNextPositionY()
    {
        return lastBarrier.transform.position.y;
    }

    private void GenerateBarrier()
    {
        GameObject randomBarrier;
        const int MaxIterations = 100;
        int iterationCounter = 0;

        // Do until we find barrier that is not active
        do
        {
            randomBarrier = PickRandomBarrier();
            iterationCounter++;

        } while (randomBarrier.activeSelf && iterationCounter < MaxIterations);

        Debug.Log("Generated barrier " + randomBarrier.name);
        randomBarrier.SetActive(true);
        randomBarrier.transform.position = new Vector2(getNextPositionX(), getNextPositionY());
        ApplyInitialPosition(randomBarrier);

        lastBarrier = randomBarrier;
        queueOfBarriers.Enqueue(lastBarrier);
    }

    private GameObject PickRandomBarrier()
    {
        int listLength = listOfTeethBarriers.Count;

        // Random.Range(min - inclusive, max - exclusive)
        int randomNumber = Random.Range(0, listLength);
        return listOfTeethBarriers[randomNumber];
    }

    private void DeactivateBarrier()
    {
        firstBarrier.SetActive(false);
        queueOfBarriers.Dequeue();
        firstBarrier = queueOfBarriers.Peek();
    }

    private void LoadInitialPosition(GameObject barrier)
    {
        var bottomTeethObject = barrier.transform.Find("TeethBottom");
        var bottomTeethDict = new Dictionary<Transform, Vector3>();
        var topTeethObject = barrier.transform.Find("TeethTop");
        var topTeethDict = new Dictionary<Transform, Vector3>();

        // Save local position of each bottom tooth
        foreach (Transform tooth in bottomTeethObject)
        {
            bottomTeethDict.Add(tooth, tooth.localPosition);
        }

        // Save local position of each top tooth
        foreach (Transform tooth in topTeethObject)
        {
            topTeethDict.Add(tooth, tooth.localPosition);
        }

        var bothTeeth = new InitialPosition(bottomTeethDict, topTeethDict);
        initialPosition.Add(barrier, bothTeeth);
    }

    private void ApplyInitialPosition(GameObject barrier)
    {
        var bottomTeethObject = barrier.transform.Find("TeethBottom");
        var topTeethObject = barrier.transform.Find("TeethTop");
        var bothTeeth = initialPosition[barrier];

        // Update position of each bottom tooth
        foreach (Transform tooth in bottomTeethObject)
        {
            tooth.localPosition = bothTeeth.BottomTeeth[tooth];
        }

        // Update position of each top tooth
        foreach (Transform tooth in topTeethObject)
        {
            tooth.localPosition = bothTeeth.TopTeeth[tooth];
            TeethClash teethClashScript = tooth.GetComponent<TeethClash>();

            // Some teeth may have the script attached, some may not (static and/or bottom teeth)
            if (teethClashScript != null)
            {
                teethClashScript.ResetCollisionAnimation();
            }
            else
            {
                Debug.Log("No teeth animating script on " + tooth.name, tooth);
            }
        }
    }

    private class InitialPosition
    {
        public InitialPosition(Dictionary<Transform, Vector3> bottomTeeth, Dictionary<Transform, Vector3> topTeeth)
        {
            this.BottomTeeth = bottomTeeth;
            this.TopTeeth = topTeeth;
        }

        public Dictionary<Transform, Vector3> BottomTeeth { get; private set; }
        public Dictionary<Transform, Vector3> TopTeeth { get; private set; }
    }
}

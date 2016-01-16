using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class SeaPavilon : Pavilon
{
    List<GameObject> SeaBarriers { get; set; }
    private Dictionary<GameObject, SeaInitialPosition> initialPosition;

    public SeaPavilon(List<GameObject> seaBarriers)
    {
        this.SeaBarriers = seaBarriers;
        initialPosition = new Dictionary<GameObject, SeaInitialPosition>();
    }

    public override void LoadInitialPosition(GameObject barrier)
    {
        // SeaWaves and boats grouped under "SeaWaves" and "Boats". Iterates all their elements.
        var seaWavesObject = barrier.transform.Find("SeaWaves");
        var seaWavesDict = new Dictionary<Transform, Vector3>();
        var boatsObject = barrier.transform.Find("Boats");
        var boatsDict = new Dictionary<Transform, Vector3>();

        // Save local position of each sea wave
        foreach (Transform seaWave in seaWavesObject)
        {
            seaWavesDict.Add(seaWave, seaWave.localPosition);
        }

        // Save local position of each boat
        foreach (Transform boat in boatsObject)
        {
            boatsDict.Add(boat, boat.localPosition);
        }

        var seaInitialPosition = new SeaInitialPosition(seaWavesDict, boatsDict);
        this.initialPosition.Add(barrier, seaInitialPosition);
    }

    public override void ApplyInitialPosition(GameObject barrier)
    {
        var seaWavesObject = barrier.transform.Find("SeaWaves");
        var boatsObject = barrier.transform.Find("Boats");
        var seaInitialPosition = this.initialPosition[barrier];

        // Update position of each sea wave
        foreach (Transform seaWave in seaWavesObject)
        {
            seaWave.localPosition = seaInitialPosition.SeaWaves[seaWave];
        }

        // Update position of each boat
        foreach (Transform boat in boatsObject)
        {
            boat.localPosition = seaInitialPosition.Boats[boat];

            // Relict from copy-paste of Teeth pavilon; might be useful in future when editing this class
            /*TeethClash teethClashScript = boat.GetComponent<TeethClash>();

            // Some teeth may have the script attached, some may not (static and/or bottom teeth)
            if (teethClashScript != null)
            {
                teethClashScript.ResetCollisionAnimation();
            }
            else
            {
                //Debug.Log("No teeth animating script on " + tooth.name, tooth);
            }*/
        }
    }

    public override float GetBarrierLength()
    {
        GameObject barrier = SeaBarriers.First();

        if (barrier == null)
        {
            throw new ArgumentException("Null object in listOfSeaItems");
        }

        // Widest child of the barrier - select manually
        var widestChild = barrier.transform.Find("Top").transform.Find("woodenWall");
        var length = widestChild.GetComponent<Renderer>().bounds.max.x
                      - widestChild.GetComponent<Renderer>().bounds.min.x;

        return length;
    }

    public override string GetPavilonName()
    {
        return "Sea pavilon";
    }

    public override List<GameObject> GetBarriers()
    {
        return SeaBarriers;
    }

    /// <summary>
    /// Get initial position of all items within each prefab for animation reset.
    /// Required for all animated items!
    /// </summary>
    private class SeaInitialPosition
    {
        public SeaInitialPosition(Dictionary<Transform, Vector3> seaWaves, Dictionary<Transform, Vector3> boats)
        {
            this.SeaWaves = seaWaves;
            this.Boats = boats;
        }

        public Dictionary<Transform, Vector3> SeaWaves { get; private set; }
        public Dictionary<Transform, Vector3> Boats { get; private set; }
    }
}

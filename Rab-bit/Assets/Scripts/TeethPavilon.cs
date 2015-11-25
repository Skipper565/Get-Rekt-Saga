using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class TeethPavilon : Pavilon
{
    List<GameObject> TeethBarriers { get; set; }
    private Dictionary<GameObject, TeethInitialPosition> initialPosition;

    public TeethPavilon(List<GameObject> teethBarriers)
    {
        this.TeethBarriers = teethBarriers;
        initialPosition = new Dictionary<GameObject, TeethInitialPosition>();
    }

    public override void LoadInitialPosition(GameObject barrier)
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

        var bothTeeth = new TeethInitialPosition(bottomTeethDict, topTeethDict);
        this.initialPosition.Add(barrier, bothTeeth);
    }

    public override void ApplyInitialPosition(GameObject barrier)
    {
        var bottomTeethObject = barrier.transform.Find("TeethBottom");
        var topTeethObject = barrier.transform.Find("TeethTop");
        var bothTeeth = this.initialPosition[barrier];

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
                //Debug.Log("No teeth animating script on " + tooth.name, tooth);
            }
        }
    }

    public override float GetBarrierLength()
    {
        GameObject barrier = TeethBarriers.First();

        if (barrier == null)
        {
            throw new ArgumentException("Null onject in listofTeeth");
        }

        var widestChild = barrier.transform.Find("TeethGumTop").transform.Find("teethGumBottom");
        var length = widestChild.GetComponent<Renderer>().bounds.max.x
                      - widestChild.GetComponent<Renderer>().bounds.min.x;

        return length;
    }

    public override string GetPavilonName()
    {
        return "Teeth pavilon";
    }

    public override List<GameObject> GetBarriers()
    {
        return TeethBarriers;
    }

    /// <summary>
    /// Get initial position of all items within each prefab for animation reset
    /// </summary>
    private class TeethInitialPosition
    {
        public TeethInitialPosition(Dictionary<Transform, Vector3> bottomTeeth, Dictionary<Transform, Vector3> topTeeth)
        {
            this.BottomTeeth = bottomTeeth;
            this.TopTeeth = topTeeth;
        }

        public Dictionary<Transform, Vector3> BottomTeeth { get; private set; }
        public Dictionary<Transform, Vector3> TopTeeth { get; private set; }
    }
}

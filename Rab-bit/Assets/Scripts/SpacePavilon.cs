using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;

public class SpacePavilon : Pavilon
{
    List<GameObject> SpaceBarriers { get; set; }
    private Dictionary<GameObject, SpaceInitialPosition> initialPosition;

    public SpacePavilon(List<GameObject> spaceBarriers)
    {
        this.SpaceBarriers = spaceBarriers;
        initialPosition = new Dictionary<GameObject, SpaceInitialPosition>();
    }

    public override void LoadInitialPosition(GameObject barrier)
    {
        // In space, no one can hear you scream. Also, pretty much everything is animated, so add everything.
        var spaceDict = new Dictionary<Transform, Vector3>();

        // Save local position of each space thing
        foreach (Transform spaceSwag in barrier.transform)
        {
            spaceDict.Add(spaceSwag, spaceSwag.localPosition);
        }

        var spaceInitialPosition = new SpaceInitialPosition(spaceDict);
        this.initialPosition.Add(barrier, spaceInitialPosition);
    }

    public override void ApplyInitialPosition(GameObject barrier)
    {
        foreach (Transform spaceThing in barrier.transform)
        {
            var spaceInitialPosition = this.initialPosition[barrier];
            spaceThing.localPosition = spaceInitialPosition.SpaceThings[spaceThing];
        }
    }

    public override float GetBarrierLength()
    {
        GameObject barrier = SpaceBarriers.First();

        if (barrier == null)
        {
            throw new ArgumentException("Null object in listOfSpaceItems");
        }

        // Widest child of the barrier - select manually
        var widestChild = barrier.transform.Find("SpaceGround");
        var length = widestChild.GetComponent<Renderer>().bounds.max.x
                      - widestChild.GetComponent<Renderer>().bounds.min.x;

        return length;
    }

    public override string GetPavilonName()
    {
        return "Space pavilon";
    }

    public override List<GameObject> GetBarriers()
    {
        return SpaceBarriers;
    }

    /// <summary>
    /// Get initial position of all items within each prefab for animation reset.
    /// Required for all animated items!
    /// </summary>
    private class SpaceInitialPosition
    {
        public SpaceInitialPosition(Dictionary<Transform, Vector3> spaceThings)
        {
            this.SpaceThings = spaceThings;
        }

        public Dictionary<Transform, Vector3> SpaceThings { get; private set; }
    }
}

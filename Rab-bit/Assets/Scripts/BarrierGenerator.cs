﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Random = UnityEngine.Random;

public class BarrierGenerator : MonoBehaviour
{
    // How many obstacles of each barriers should be shown before switching to another barriers
    public int obstacleRepetition;
    private int currentObstacleRepetition;

    public PowerUpJump powerUpJump;
    public PowerUpScore powerUpScore;

    // List of barriers; each contain barriers of exactly one type
    // Add new list for each new pavilon
    public List<GameObject> listOfTeethBarriers;
    public List<GameObject> listOfSeaBarriers;
    public List<GameObject> listOfSpaceBarriers;
    // TODO: Add other list

    private Pavilon currentPavilon;
    TeethPavilon teethPavilon;
    SeaPavilon seaPavilon;
    SpacePavilon spacePavilon;
    List<Pavilon> listOfAllPavilons; 
    // TODO: Add other pavilons

    // Barriers
    private float barrierLengthInCurrentPavilon;
    private GameObject firstBarrier;
    private GameObject lastBarrier;
    private Queue<GameObject> queueOfBarriers;

    // World coordinate X of camera coordinate
    private float worldCoordX;

    // Area along x axis. When right border of camera is detected inside this area, next barrier ahead is generated.
    // When x generation offset is loo low and x speed too high, there is high risk of not detecting anything at all!
    private int xGenerationOffset;

    // Use this for initialization
    void Start()
    {
        teethPavilon = new TeethPavilon(listOfTeethBarriers);
        seaPavilon = new SeaPavilon(listOfSeaBarriers);
        spacePavilon = new SpacePavilon(listOfSpaceBarriers);
        // TODO: Initialize other pavilons by their type (PavilonType : Pavilon)

        // Load all barriers and their initial position
        // TODO: Add other pavilons to the list of all pavilons
        listOfAllPavilons = new List<Pavilon>() { teethPavilon, seaPavilon, spacePavilon };

        foreach (var pavilon in listOfAllPavilons)
        {
            foreach (var barrier in pavilon.GetBarriers())
            {
                barrier.SetActive(false);
                pavilon.LoadInitialPosition(barrier);
            }
        }
        
        /* Choose current pavilon and the first barrier.
        We always want to start with the first barrier of the teeth pavilon 
        which has good starting conditions (some space at the beginning), but 
        first we must choose any barrier prior to the first one to act as a "scenery". */
        currentPavilon = teethPavilon;
        //firstBarrier = currentPavilon.GetBarriers().First();
        firstBarrier = PickRandomBarrier(currentPavilon.GetBarriers().Skip(1).ToList());

        // Get screen width and height + world coordinates of axis x
        int screenPixelWidth = Camera.main.pixelWidth;
        int screenPixelHeight = Camera.main.pixelHeight;
        worldCoordX = Camera.main.ScreenToWorldPoint(new Vector2(screenPixelWidth, screenPixelHeight))[0];

        // Set first barrier and its length
        firstBarrier.SetActive(true);
        queueOfBarriers = new Queue<GameObject>();
        queueOfBarriers.Enqueue(firstBarrier);
        barrierLengthInCurrentPavilon = currentPavilon.GetBarrierLength();

        // First barrier must be shifted a bit to the left; translate the first barrier to suitable position
        firstBarrier.transform.Translate(new Vector2(Camera.main.transform.position[0] - firstBarrier.transform.position.x - 20f,
                                                     Camera.main.transform.position[1]));
        lastBarrier = firstBarrier;

        // Set second barrier just to be sure; generate next barriers with Update()
        GenerateBarrier(currentPavilon.GetBarriers().First());
        GenerateBarrier();

        // Two barriers generated so far
        currentObstacleRepetition = 3;
        xGenerationOffset = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if ((lastBarrier.transform.position.x >= Camera.main.transform.position[0] + worldCoordX)
            && (lastBarrier.transform.localPosition.x - xGenerationOffset < Camera.main.transform.position[0] + worldCoordX))
        {
            GenerateBarrier();
            currentObstacleRepetition++;
        }

        if (firstBarrier.transform.position.x < Camera.main.transform.position[0] - (worldCoordX * 2.5))
        {
            DeactivateBarrier();
        }

        // Pick new barriers if 
        if (currentObstacleRepetition >= obstacleRepetition)
        {
            ChangePavilon();
            //Debug.Log("Current repetition: " + currentObstacleRepetition + ", obstacle repetition: " + obstacleRepetition);
            currentObstacleRepetition = 0;
        }
    }

    private float getNextPositionX()
    {
        //Debug.Log("Next position X: " + (lastBarrier.transform.position.x + barrierLengthInCurrentPavilon));
        //Debug.Log("Barrier length: " + barrierLengthInCurrentPavilon);

        return lastBarrier.transform.position.x + barrierLengthInCurrentPavilon;
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
            randomBarrier = PickRandomBarrier(currentPavilon.GetBarriers());
            iterationCounter++;

        } while (randomBarrier.activeSelf && iterationCounter < MaxIterations);

        var nextPosition = new Vector2(getNextPositionX(), getNextPositionY());

        Debug.Log("Generated barrier " + randomBarrier.name);
        randomBarrier.SetActive(true);
        randomBarrier.transform.position = nextPosition;
        currentPavilon.ApplyInitialPosition(randomBarrier);

        lastBarrier = randomBarrier;
        queueOfBarriers.Enqueue(lastBarrier);

        var powerUp = PickRandomPowerUp(new List<PowerUp>() {powerUpJump, powerUpScore});
        powerUp.SpawnIfAvailable(GetBounds(randomBarrier));
    }

    private void GenerateBarrier(GameObject barrier)
    {
        if (barrier.activeSelf)
        {
            Debug.LogError("Attempted to generate barrier that is already active. Generating random barrier instead.");
            GenerateBarrier();
            return;
        }

        Debug.Log("Generated barrier " + barrier.name);
        barrier.SetActive(true);
        barrier.transform.position = new Vector2(getNextPositionX(), getNextPositionY());
        currentPavilon.ApplyInitialPosition(barrier);

        lastBarrier = barrier;
        queueOfBarriers.Enqueue(lastBarrier);
    }

    private GameObject PickRandomBarrier(List<GameObject> barriers)
    {
        int listLength = barriers.Count;
        if (listLength == 0)
        {
            Debug.LogError("Empty list of barriers in this pavilon. Check that you assigned them to the list.");
            throw new ArgumentException("Empty list of barriers in this pavilon. Check that you assigned them to the list.");
        }

        // Random.Range(min - inclusive, max - exclusive)
        int randomNumber = Random.Range(0, listLength);
        return barriers[randomNumber];
    }

    private Pavilon PickRandomPavilon(List<Pavilon> listOfPavilons)
    {
        int listLength = listOfPavilons.Count;

        // Random.Range(min - inclusive, max - exclusive)
        int randomNumber = Random.Range(0, listLength);
        return listOfPavilons[randomNumber];
    }

    private void ChangePavilon()
    {
        Pavilon randomPavilon;

        // Pick new one until different barriers is picked
        do
        {
            randomPavilon = PickRandomPavilon(listOfAllPavilons);
        } while (randomPavilon == currentPavilon);

        currentPavilon = randomPavilon;
        Debug.Log("Current pavilon: " + currentPavilon.GetPavilonName());
    } 

    private void DeactivateBarrier()
    {
        firstBarrier.SetActive(false);
        queueOfBarriers.Dequeue();
        firstBarrier = queueOfBarriers.Peek();
    }

    private Bounds GetBounds(GameObject barrier)
    {
        var bounds = GetMaxBoundsOfPrefab(barrier);

        // Spawn power up only in the upper half of the barrier
        if (currentPavilon == seaPavilon)
        {
            var center = new Vector3(bounds.center.x, (bounds.center.y + bounds.max.y)/2, bounds.center.z);
            var size = new Vector3(bounds.size.x, bounds.size.y/2, bounds.size.z);
            Bounds b = new Bounds(center, size);
            return b;
        }
        else
        {
            return bounds;
        }
    }

    private Bounds GetMaxBoundsOfPrefab(GameObject barrier)
    {
        var bounds = new Bounds(barrier.transform.position, Vector3.zero);

        var boundingChildren = barrier.GetComponentsInChildren<Renderer>()
            .Where(x => x.gameObject.tag == "BarrierBottom" || x.gameObject.tag == "BarrierTop")
            .ToList();

        foreach (var child in boundingChildren)
        {
            bounds.Encapsulate(child.bounds);
        }

        return bounds;
    }

    private PowerUp PickRandomPowerUp(List<PowerUp> listOfPowerUps)
    {
        int randomNumber = Random.Range(0, listOfPowerUps.Count);
        return listOfPowerUps[randomNumber];
    }
}

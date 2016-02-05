using UnityEngine;

public class PowerUpScore : PowerUp
{
    public float spawnChance;
    public int powerUpValue;
    public static int value;
    public static bool collected;

    protected override void Start()
    {
        if (powerUpValue == 0)
        {
            value = 20;
        }
        else
        {
            value = powerUpValue;
        }

        collected = false;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SpawnIfAvailable(Bounds bounds)
    {
        if (/*collected || */spawnChance <= Random.Range(0f, 100f))
        {
            return;
        }

        base.SpawnIfAvailable(bounds);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Collect();
    }

    public void Collect()
    {
        Debug.Log("Score power up collected, prepare to get rekt!");
        gameObject.SetActive(false);

        collected = true;
    }
}

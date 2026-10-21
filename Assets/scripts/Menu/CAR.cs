using UnityEngine;

public class CAR : MonoBehaviour
{
    [Header("Cars")]
    public GameObject[] Cars;

    [Header("Spawn Points")]
    // 0,1 = Left Side
    // 2,3 = Right Side
    public Transform[] SpawnPoints;

    [Header("Spawn")]
    [Range(0, 2)]
    public int MaxCarsPerWave = 2;

    [Header("Speed")]
    public float MinSpeed = 10f;
    public float MaxSpeed = 18f;

    [Header("Spawn Check")]
    public float CheckRadius = 4f;

    int aliveCars = 0;

    void Start()
    {
        SpawnWave();
    }

    void SpawnWave()
    {
        int count = Random.Range(1, MaxCarsPerWave + 1);

        aliveCars = 0;

        for (int i = 0; i < count; i++)
            SpawnOneCar();
    }

    void SpawnOneCar()
    {
        for (int i = 0; i < 10; i++)
        {
            Transform spawn = SpawnPoints[Random.Range(0, SpawnPoints.Length)];

            Collider[] hits = Physics.OverlapSphere(spawn.position, CheckRadius);

            bool occupied = false;

            foreach (Collider c in hits)
            {
                if (c.CompareTag("Car"))
                {
                    occupied = true;
                    break;
                }
            }

            if (occupied)
                continue;

            GameObject car = Instantiate(
                Cars[Random.Range(0, Cars.Length)],
                spawn.position,
                spawn.rotation);

            car.tag = "Car";

            CarMovement move = car.GetComponent<CarMovement>();

            if (move != null)
            {
                move.Speed = Random.Range(MinSpeed, MaxSpeed);
                move.Manager = this;
            }

            aliveCars++;
            return;
        }
    }

    public void CarDestroyed()
    {
        aliveCars--;

        if (aliveCars <= 0)
        {
            SpawnWave();
        }
    }
}
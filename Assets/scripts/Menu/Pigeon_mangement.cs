using UnityEngine;

public class Pigeon_mangement : MonoBehaviour
{
    public GameObject[] PigeonSpawnPoint, pigeongroups;
    [Header("DelayTime")]
    public float DelayToSpawn;
    [Range(0f,10f)]
    public float StartDelay;
    private int RandowmSpawnPigeon, RandowmSpawnPoint;
    void Start()
    {
        InvokeRepeating(nameof(BirdSpawner), StartDelay, DelayToSpawn);
    }
    private void BirdSpawner()
    {
        RandowmSpawnPigeon = Random.Range(0, pigeongroups.Length);
        RandowmSpawnPoint = Random.Range(0, PigeonSpawnPoint.Length);
        GameObject BirdGroup = Instantiate(pigeongroups[RandowmSpawnPigeon],
            PigeonSpawnPoint[RandowmSpawnPoint].transform.position,
             PigeonSpawnPoint[RandowmSpawnPoint].transform.rotation);
        

    }
}

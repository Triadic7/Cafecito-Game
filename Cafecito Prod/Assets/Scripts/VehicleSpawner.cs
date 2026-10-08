using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> vehicles;

    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private float spawnInterval = 6f;

    /// <summary>
    /// How long it takes before a vehicle despawns.
    /// </summary>
    [SerializeField]
    private float despawnTime = 7f;

    private bool canSpawnVehicles = true;

    private void Start()
    {
        canSpawnVehicles = true;
        StartCoroutine(SpawnVehiclesRepeatedly());
    }

    private IEnumerator SpawnVehiclesRepeatedly()
    {
        while (canSpawnVehicles)
        {
            SpawnVehicle();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Spawns a vehicle.
    /// </summary>
    private void SpawnVehicle()
    {
        // Pick random vehicle.
        GameObject vehiclePrefab = vehicles[Random.Range(0, vehicles.Count)];

        // Pick between left or right.
        int direction = Random.Range(0, 2);
        Transform spawn = spawnPoints[direction];

        // Make copy.
        GameObject vehicleInstance = Instantiate(vehiclePrefab, spawn.position, Quaternion.identity);

        // Make vehicle move.
        StartCoroutine(vehicleInstance.GetComponent<Vehicle>().DriveForSeconds(despawnTime, direction == 0 ? true : false));
    }

    
}

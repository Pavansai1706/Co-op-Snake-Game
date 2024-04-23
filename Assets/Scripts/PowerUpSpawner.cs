using UnityEngine;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] powerUpPrefabs; // Array of power-up prefabs to spawn
    [SerializeField] private float spawnInterval = 5f; // Interval between spawns (5 seconds)
    [SerializeField] private GameObject gridArea; // Reference to the GridArea GameObject

    private void Start()
    {
        // Delay the initial spawning by waiting for 5 seconds
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(5f); // Wait for 5 seconds

        // Start the coroutine for spawning power-ups
        StartCoroutine(SpawnPowerUpsRoutine());
    }

    private IEnumerator SpawnPowerUpsRoutine()
    {
        while (true)
        {
            // Wait for the specified spawn interval
            yield return new WaitForSeconds(spawnInterval);

            // Spawn a power-up
            SpawnPowerUp();
        }
    }

    private void SpawnPowerUp()
    {
        if (powerUpPrefabs.Length == 0)
        {
            Debug.LogWarning("No power-up prefabs assigned.");
            return;
        }

        if (gridArea == null)
        {
            Debug.LogWarning("GridArea GameObject is not assigned.");
            return;
        }

        // Get the boundaries of the grid area
        Bounds gridBounds = gridArea.GetComponent<Collider2D>().bounds;

        // Generate a random position within the grid area boundaries
        Vector3 spawnPosition = new Vector3(Random.Range(gridBounds.min.x, gridBounds.max.x), Random.Range(gridBounds.min.y, gridBounds.max.y), 0f);

        // Randomly select a power-up prefab from the array
        GameObject powerUpPrefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];

        // Instantiate the selected power-up prefab at the random position
        GameObject spawnedPowerUp = Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);

        // Parent the spawned power-up to the PowerUpSpawner object to keep it in the scene
        spawnedPowerUp.transform.SetParent(transform);
    }
}

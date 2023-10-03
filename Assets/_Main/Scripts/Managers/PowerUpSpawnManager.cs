using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class PowerUpSpawnManager : NetworkBehaviour
{
    [Header("Spawner Settings")] 
    [SerializeField] private NetworkObject[] powerUpPrefabs;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private bool autoStart = true;
    [SerializeField] private float timeForSpawn = 500f;
    [SerializeField] private float currTime = 0f;

    [Header("Despawn Settings")] 
    [SerializeField] private float despawnTime = 30f;

    private void Start()
    {
        if (IsServer && autoStart)
            StartSpawning();
    }

    private void Update()
    {
        currTime += Time.deltaTime;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnPowerUps());
    }

    private IEnumerator SpawnPowerUps()
    {
        while (currTime < timeForSpawn)  // TODO: CAMBIAR ESTO URGENTE A ALGUN CHEQUEO DE SI EL TIMER DE JUEGO SIGUE CORRIENDO)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (transform.childCount == 0)
            {
                SpawnRandomPowerUp();
            }
        }
    }
    
    private void SpawnRandomPowerUp()
    {
        if (powerUpPrefabs.Length == 0) return;

        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        NetworkObject powerUpPrefab = powerUpPrefabs[randomIndex];
        
        NetworkObject spawnedPowerUp = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
        spawnedPowerUp.Spawn();
        
        StartCoroutine(DespawnAfterTime(spawnedPowerUp));
    }

    private IEnumerator DespawnAfterTime(NetworkObject spawnedPowerUp)
    {
        yield return new WaitForSeconds(despawnTime);

        if (spawnedPowerUp == default) 
            yield break;
        
        spawnedPowerUp.Despawn();
        Destroy(spawnedPowerUp.gameObject);
    }
}

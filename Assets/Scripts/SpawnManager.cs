using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Enemy information
    public GameObject enemyPrefab;
    public Vector3Int randomPosBounds;
    public Vector3 positionOffset; 

    // Wave information
    public int numberOfEnemies;
    private int waveNumber = 1;

    // Rest point
    public GameObject restPointPrefab;
    public int maxRestPoint = 1;

    // Start is called before the first frame update
    void Start()
    {
        CreateEnemyWaves(waveNumber);
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfEnemies == 0)
        {
            CreateEnemyWaves(++waveNumber);
        }
        
    }

    // Generate a position at square on the board
    Vector3Int GenerateRandomPosition()
    {
        int randomPosX = Random.Range(-randomPosBounds.x, randomPosBounds.x);
        int randomPosZ = Random.Range(-randomPosBounds.z, randomPosBounds.z);

        return new Vector3Int(randomPosX, randomPosBounds.y, randomPosZ);
    }

    // Create 
    void CreateEnemyWaves(int enemyNumber)
    {
        Vector3 randPos; 

        for(int i = 0; i < enemyNumber; i++)
        {
            randPos = GenerateRandomPosition() + positionOffset;
            Instantiate(enemyPrefab, randPos, enemyPrefab.transform.rotation);
        }

        numberOfEnemies = enemyNumber;

        Instantiate(restPointPrefab, GenerateRandomPosition(), restPointPrefab.transform.rotation);
    }
}

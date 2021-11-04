using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidPlayer : MonoBehaviour
{
    private SpawnManager spawnManager;
    private PlayerController playerController;

    public GameObject currentEnemyHit
    {
        get;
        private set;
    }

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        playerController = FindObjectOfType<PlayerController>();
    }

    // What the heck are we hitting?
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentEnemyHit = collision.gameObject;
            spawnManager.numberOfEnemies--;
            playerController.AttackEnemy();
        }
        else if(collision.gameObject.CompareTag("Rest Point"))
        {
            Destroy(collision.gameObject);
            playerController.ResetEnergy();
        }
    }
}

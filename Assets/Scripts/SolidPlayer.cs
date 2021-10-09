using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidPlayer : MonoBehaviour
{
    private SpawnManager spawnManager;
    private PlayerController playerController;
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
            Debug.Log("Enemy hit");
            collision.gameObject.GetComponent<Animator>().SetBool("Death_b", true);
            //Destroy(collision.gameObject);
            spawnManager.numberOfEnemies--;
        }
        else if(collision.gameObject.CompareTag("Rest Point"))
        {
            Debug.Log("Refreshing energy");
            Destroy(collision.gameObject);
            playerController.ResetEnergy();
        }
    }
}

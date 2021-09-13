using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 movementLimits;
    public SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerMovement();
        CheckBoundaries();
    }
    
    // Handle the players lateral movements
    void HandlePlayerMovement()
    {
        // Player movement is done on a square by square movement
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position += Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += Vector3.back;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += Vector3.left;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += Vector3.right;
        }
    }
    void CheckBoundaries()
    {
        // If the player ever gets past the boundaries return it by one block
        if(transform.position.x > movementLimits.x)
        {
            transform.position += Vector3.left;
        }
        else if(transform.position.x < -movementLimits.x)
        {
            transform.position += Vector3.right;
        }
        if(transform.position.z > movementLimits.z)
        {
            transform.position += Vector3.back;
        }
        else if(transform.position.z < -movementLimits.z)
        {
            transform.position += Vector3.forward;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit");
            Destroy(collision.gameObject);
            spawnManager.numberOfEnemies--;
        }
    }
}

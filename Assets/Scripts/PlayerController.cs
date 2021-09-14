using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 movementLimits;
    public SpawnManager spawnManager;

    // Stat reenergy
    private readonly float maxEnergy = 20;
    // Player stats
    public float energy = 50;

    // Used to handle different movement types
    private KeyCode currentKey;
    private readonly float timeInterval = 0.1f;
    private float timeCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();
        currentKey = KeyCode.None;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        CheckBoundaries();
        CheckStats();
    }
    
    // Handle the player movement
    void HandleMovement()
    {
        // Handle the keyboard input
        if (currentKey == KeyCode.None)
        {
            if (Input.GetKeyDown(KeyCode.W)) currentKey = KeyCode.W;
            else if (Input.GetKeyDown(KeyCode.S)) currentKey = KeyCode.S;
            else if (Input.GetKeyDown(KeyCode.A)) currentKey = KeyCode.A;
            else if (Input.GetKeyDown(KeyCode.D)) currentKey = KeyCode.D;
        }
        else
        {
            if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) ||
               Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                currentKey = KeyCode.None;
            }

            // Handle if the a certain time frame has passed to move
            if(timeCounter >= timeInterval)
            {
                switch (currentKey)
                {
                    case KeyCode.W:
                        transform.position += Vector3.forward;
                        energy--;
                        break;
                    case KeyCode.S:
                        transform.position += Vector3.back;
                        energy--;
                        break;
                    case KeyCode.A:
                        transform.position += Vector3.left;
                        energy--;
                        break;
                    case KeyCode.D:
                        transform.position += Vector3.right;
                        energy--;
                        break;
                    default:
                        break;
                }
                // Reset the counter
                timeCounter = 0;
            }
            else
            {
                timeCounter += Time.deltaTime;
            }
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

    void CheckStats()
    {
        if(energy <= 0)
        {
            Debug.Log("Game over");
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
        else if(collision.gameObject.CompareTag("Rest Point"))
        {
            Debug.Log("Refreshing energy");
            Destroy(collision.gameObject);
            energy = maxEnergy;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 movementLimits;
    public SpawnManager spawnManager;
    // Note: The player prefab will be moved after pressing enter
    private Vector3 originalLocation;

    // Player movement tracking
    public Queue<Quaternion> playerRot;
    private bool isMoving = false;

    // Stat reenergy
    public float maxEnergy;
    // Player stats
    public float energy;

    // Used to handle different movement types
    private KeyCode currentKey;
    private readonly float timeInterval = 0.1f;
    private float timeCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerRot = new Queue<Quaternion>();
        spawnManager = FindObjectOfType<SpawnManager>();
        currentKey = KeyCode.None;
        originalLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerRot.Count != 0 && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Time to move");
            transform.position = originalLocation;
        }

        HandleMovement();
        CheckBoundaries();
        CheckStats();
    }
    void Traverse()
    {
        foreach(var rot in playerRot)
        {
        }
    }

    void HandleMovement()
    {
        // Handle the key presses when they're set
        if (Input.GetKeyDown(KeyCode.W)) SetKeyDown(KeyCode.W);
        else if (Input.GetKeyDown(KeyCode.S)) SetKeyDown(KeyCode.S);
        else if (Input.GetKeyDown(KeyCode.A)) SetKeyDown(KeyCode.A);
        else if (Input.GetKeyDown(KeyCode.D)) SetKeyDown(KeyCode.D);

        // If the player has stopped moving don't track movement
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) ||
           Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            isMoving = false;
        }

        // If there's still movement and the counter > time interval
        if (isMoving && timeCounter >= timeInterval)
        {
            switch (currentKey)
            {
                case KeyCode.W:
                    QueueMovement(new Vector3(0, 0, 0));
                    break;
                case KeyCode.S:
                    QueueMovement(new Vector3(0, 180, 0));
                    break;
                case KeyCode.A:
                    QueueMovement(new Vector3(0, 270, 0));
                    break;
                case KeyCode.D:
                    QueueMovement(new Vector3(0, 90, 0));
                    break;
                default:
                    break;
            }
            // Reset the counter
            timeCounter = 0;
        }

        // If we're still moving, track the input counter else void it
        if (isMoving) timeCounter += Time.deltaTime;
        else timeCounter = 0;
    }

    // Just a setter function for the current key
    private void SetKeyDown(KeyCode k)
    {
        currentKey = k;
        isMoving = true;
    }

    // Queue up the rotation of the players movement and move the player
    private void QueueMovement(Vector3 rot)
    {
        transform.rotation = Quaternion.Euler(rot);
        playerRot.Enqueue(transform.rotation);

        transform.position += transform.forward;
        energy--;
    }

    // Check if the player is attempting to go beyond the x and z axis
    private void CheckBoundaries()
    {
        Vector3 playerPos = transform.position;
        
        if((playerPos.x > movementLimits.x || playerPos.x < -movementLimits.x) ||
           (playerPos.z > movementLimits.z || playerPos.z < -movementLimits.z))
        {
            transform.position -= transform.forward;
        }
    }


    // Check if the player has no energy
    void CheckStats()
    {
        if(energy <= 0)
        {
            Debug.Log("Game over");
        }
    }

    // What the heck are we hitting?
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

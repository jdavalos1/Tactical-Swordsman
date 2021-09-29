using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 movementLimits;
    private SpawnManager spawnManager;
    // Note: The player prefab will be moved after pressing enter
    [SerializeField] GameObject transparentPlayer;
    [SerializeField] GameObject solidPlayer;
    [SerializeField] FollowPlayer followPlayerScript;

    // Player movement tracking
    private bool playerIsMoving = false;
    private Queue<Quaternion> playerRot;

    // Moving solid information
    [SerializeField] float solidPlayerMoveSpeed = 5;
    private float journeyLength;
    private float startTime;
    private bool solidIsMoving = false;
    private Vector3 originalPosition;
    private Vector3 nextPosition;

    // Stat reenergy
    [SerializeField] float maxEnergy;
    // Player stats
    private float energy = 100;

    // Used to handle different movement types
    private KeyCode currentKey;
    private readonly float timeInterval = 0.1f;
    private float timeCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        followPlayerScript = Camera.main.GetComponent<FollowPlayer>(); 
        playerRot = new Queue<Quaternion>();
        spawnManager = FindObjectOfType<SpawnManager>();
        currentKey = KeyCode.None;
    }

    // Update is called once per frame
    void Update()
    {
        // If we hit space then start moving
        if (Input.GetKeyDown(KeyCode.Space))
        {
            followPlayerScript.player = solidPlayer;
            solidIsMoving = true;
            StartCoroutine(Traverse());
        }

        // Is the translucent allowed to move or the solid?
        if (!solidIsMoving)
        {
            HandleTransparentShow();
            HandleMovement();
            CheckBoundaries();
            CheckStats();
        }
    }

    IEnumerator Traverse()
    {
        solidPlayer.GetComponent<Animator>().SetBool("Run_b", true);
        // Iterate through the queue
        while(playerRot.Count > 0)
        {
            // Rotate the solid player first
            solidPlayer.transform.rotation = playerRot.Dequeue();
            // Obtain the next position and the length needed
            nextPosition = solidPlayer.transform.position + solidPlayer.transform.forward;
            journeyLength = Vector3.Distance(solidPlayer.transform.position, nextPosition);
            startTime = Time.time;
            // The starting position obtained
            originalPosition = solidPlayer.transform.position;
            // let the object move forward
            yield return StartCoroutine(MoveForward());
        }
        solidIsMoving = false;
        followPlayerScript.player = transparentPlayer;
        solidPlayer.GetComponent<Animator>().SetBool("Run_b", false);
    }

    IEnumerator MoveForward()
    {
        float dist = Vector3.Distance(solidPlayer.transform.position, nextPosition);
        // While the object can still traverse 
        while ( dist > 0)
        {
            // Get the distance covered from the beginning of the movement
            float distCovered = (Time.time - startTime) * solidPlayerMoveSpeed;
            // Get the current fraction of the journey
            float fractionOfJourney = distCovered / journeyLength;
            // Lerp over time and wait 0.005 secs before lerping again
            solidPlayer.transform.position = Vector3.Lerp(originalPosition, nextPosition, fractionOfJourney);
            yield return new WaitForSeconds(0.005f);
            dist = Vector3.Distance(solidPlayer.transform.position, nextPosition);
            Debug.Log($"Solid Player Position: {solidPlayer.transform.position}");
            Debug.Log($"Next Position: {nextPosition}");
            Debug.Log($"Distance Covered: {dist}");
        }
    }

    // Handle whether or not the transparent version should be seen
    void HandleTransparentShow()
    {
        // Get the distance between the transparent and solid player
        float dist = Vector3.Distance(transparentPlayer.transform.position, solidPlayer.transform.position);

        // Set the player active if the distance is 0
        transparentPlayer.SetActive(false);
        if (dist > 0) transparentPlayer.SetActive(true);
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
            playerIsMoving = false;
        }

        // If there's still movement and the counter > time interval
        if (playerIsMoving && timeCounter >= timeInterval)
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
        if (playerIsMoving) timeCounter += Time.deltaTime;
        else timeCounter = 0;
    }

    // Just a setter function for the current key
    private void SetKeyDown(KeyCode k)
    {
        currentKey = k;
        playerIsMoving = true;
    }

    // Queue up the rotation of the players movement and move the player
    private void QueueMovement(Vector3 rot)
    {
        transparentPlayer.transform.rotation = Quaternion.Euler(rot);
        playerRot.Enqueue(transparentPlayer.transform.rotation);

        transparentPlayer.transform.position += transparentPlayer.transform.forward;
        energy--;
    }


    // Check if the player is attempting to go beyond the x and z axis
    private void CheckBoundaries()
    {
        Vector3 playerPos = transparentPlayer.transform.position;
        
        if((playerPos.x > movementLimits.x || playerPos.x < -movementLimits.x) ||
           (playerPos.z > movementLimits.z || playerPos.z < -movementLimits.z))
        {
            playerRot.Dequeue();
            energy++;
            transparentPlayer.transform.position -= transparentPlayer.transform.forward;
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

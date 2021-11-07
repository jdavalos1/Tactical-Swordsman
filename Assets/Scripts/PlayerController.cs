using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    // Game Manager to see if we can do anything
    GameManager gameManager;
    // Sound manager for the sfx
    SoundManager soundManager;
    // Limits in the x y and z direction for player movement
    public Vector3 movementLimits;

    // Note: The player prefab will be moved after pressing space 
    [SerializeField] GameObject transparentPlayerPrefab;
    [SerializeField] GameObject solidPlayer;
    private FollowPlayer followPlayerScript;
    private GameObject transparentPlayer;

    // Player movement tracking
    private bool playerIsMoving = false;
    private Queue<Quaternion> playerRot;

    // Moving solid information
    [SerializeField] float solidPlayerMoveSpeed;
    private float originalPlayerMoveSpeed;
    private float journeyLength;
    private float startTime;
    private bool solidIsMoving = false;
    private Vector3 originalPosition;
    private Animator solidPlayerAnimator;
    private Coroutine traversalCoroutine;

    // Player stats
    private float currentEnergy;
    private float movementTraversed;
    private int killCount;

    // Player maximums
    [SerializeField] float maximumEnergy;
    [SerializeField] int allowedMovement;

    // Used to handle different movement types
    private KeyCode currentKey;
    private readonly float timeInterval = 0.1f;
    private float timeCounter = 0;

    // UI Attributes
    [SerializeField] TextMeshProUGUI killsCountText;
    [SerializeField] Slider energySlider;

    // Start is called before the first frame update
    void Start()
    {
        // Set up stats
        movementTraversed = 0;
        currentKey = KeyCode.None;
        currentEnergy = maximumEnergy;

        // Set up movement vars
        playerRot = new Queue<Quaternion>();
        solidPlayerAnimator = solidPlayer.GetComponent<Animator>();
        followPlayerScript = Camera.main.GetComponent<FollowPlayer>();
        originalPlayerMoveSpeed = solidPlayerMoveSpeed;

        // Set up managers
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();

        // Set up UI
        energySlider.maxValue = maximumEnergy;
        killsCountText.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.isGameOver)
        {
            HandleSolidMovement();
            CheckStats();
            // Is the translucent allowed to move or the solid?
            if (!solidIsMoving)
            {
                HandleMovement();
                CheckBoundaries();
                HandleEscapeKey();
                HandleSingleReturnPress();
            }
        }
    }


    /// <summary>
    /// Traverse based on the rotations handled
    /// </summary>
    /// <returns></returns>
    IEnumerator Traverse()
    {
        solidPlayerAnimator.SetBool("Run_b", true);
        // Play a randomly looped running sound
        string runningSound = $"Running_1";
        soundManager.Play(runningSound);
        // Iterate through the queue
        while(playerRot.Count > 0)
        {
            // Rotate the solid player first
            solidPlayer.transform.rotation = playerRot.Dequeue();
            // Obtain the next position and the length needed
            Vector3 nextPosition = solidPlayer.transform.position + solidPlayer.transform.forward;
            journeyLength = Vector3.Distance(solidPlayer.transform.position, nextPosition);
            startTime = Time.time;
            // The starting position obtained
            originalPosition = solidPlayer.transform.position;
            // let the object move forward
            yield return StartCoroutine(MoveForward(nextPosition));
        }
        movementTraversed = 0;
        solidIsMoving = false;
        followPlayerScript.player = transparentPlayer;
        soundManager.Stop(runningSound);
        solidPlayerAnimator.SetBool("Run_b", false);
    }

    /// <summary>
    /// Moves a unit vector forward
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveForward(Vector3 nextPosition)
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
            yield return null; 
            dist = Vector3.Distance(solidPlayer.transform.position, nextPosition);
        }
        currentEnergy--;
        energySlider.value = currentEnergy;
    }

    /// <summary>
    /// Handles if the return key has been pressed
    /// to dequeue the last position
    /// </summary>
    void HandleSingleReturnPress()
    {
        if (Input.GetKeyDown(KeyCode.B) && playerRot.Count > 0)
        {
            soundManager.Play("LowClick");
            // First remove the last movement and find out the last
            // rotation to rotate the player to the needed position
            var listVer = playerRot.ToList();
            listVer.RemoveAt(listVer.Count - 1);

            // If the player is not at solid location change the location
            // else delete the transparent and start them at solid
            if (playerRot.Count > 1)
            {
                transparentPlayer.transform.position -= transparentPlayer.transform.forward;
                var secLastRot = listVer[listVer.Count - 1];
                transparentPlayer.transform.rotation = secLastRot;
            }
            else
            {
                followPlayerScript.player = solidPlayer;
                Destroy(transparentPlayer);
            }

            movementTraversed--;
            currentKey = 0;
            playerRot = new Queue<Quaternion>(listVer);
        }
    }

    void HandleSolidMovement()
    {
        // If we hit space then start moving
        if (Input.GetKeyDown(KeyCode.Space))
        {
            followPlayerScript.player = solidPlayer;
            solidIsMoving = true;
            currentKey = KeyCode.None;
            if (transparentPlayer != null)
            {
                Destroy(transparentPlayer);
            }

            traversalCoroutine = StartCoroutine(Traverse());
        }
    }

    /// <summary>
    /// Removes the transparent and returns to the solid
    /// </summary>
    void HandleEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followPlayerScript.player = solidPlayer;
            currentKey = KeyCode.None;
            movementTraversed = 0;
            playerRot.Clear();
            Destroy(transparentPlayer);
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
        soundManager.Play("HighClick");
        if(playerRot.Count == 0 && transparentPlayer == null)
        {
            transparentPlayer = Instantiate(transparentPlayerPrefab, solidPlayer.transform.position, solidPlayer.transform.rotation);
            followPlayerScript.player = transparentPlayer;
        }

        if (movementTraversed < allowedMovement)
        {
            movementTraversed++;
            transparentPlayer.transform.rotation = Quaternion.Euler(rot);
            playerRot.Enqueue(transparentPlayer.transform.rotation);
            transparentPlayer.transform.position += transparentPlayer.transform.forward;
        }
    }


    // Check if the player is attempting to go beyond the x and z axis
    private void CheckBoundaries()
    {
        if (transparentPlayer == null) return;
        Vector3 playerPos = transparentPlayer.transform.position;
        
        if((playerPos.x > movementLimits.x || playerPos.x < -movementLimits.x) ||
           (playerPos.z > movementLimits.z || playerPos.z < -movementLimits.z))
        {
            if(playerRot.Count > 0) playerRot.Dequeue();
            movementTraversed--;
            transparentPlayer.transform.position -= transparentPlayer.transform.forward;
        }
    }

    // Check if the player has no energy
    void CheckStats()
    {
        // Should only happen once when the energy has gone to 0
        if (currentEnergy <= 0 && !gameManager.isGameOver)
        {
            gameManager.isGameOver = true;
            solidPlayerAnimator.SetBool("Death_b", true);
            solidPlayerAnimator.SetBool("Run_b", false);
            playerRot.Clear();
            solidIsMoving = false;
            if (traversalCoroutine != null) StopCoroutine(traversalCoroutine);
        }
    }

    // Just used to not expose the max energy and energy fields
    public void ResetEnergy()
    {
        currentEnergy = maximumEnergy;
        energySlider.value = currentEnergy;
    }

    public void AttackEnemy()
    {
        killCount++;
        killsCountText.text = killCount.ToString();
        solidPlayerMoveSpeed = 0;
        solidPlayerAnimator.SetBool("Attack_b", true);
        solidPlayerAnimator.SetBool("Run_b", false);
    }
    
    public void ContinueRun()
    {
        solidPlayerMoveSpeed = originalPlayerMoveSpeed;
        startTime = Time.time;
        solidPlayerAnimator.SetBool("Attack_b", false);
        solidPlayerAnimator.SetBool("Run_b", true);
        soundManager.Play("Running_1");
    }
}

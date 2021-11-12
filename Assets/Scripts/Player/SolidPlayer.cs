using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidPlayer : MonoBehaviour
{
    private PlayerController playerController;
    private SoundManager soundManager;

    public GameObject currentEnemyHit
    {
        get;
        private set;
    }

    // Start is called before the first frame update
    void Start()
    {
        soundManager = FindObjectOfType<SoundManager>();
        playerController = FindObjectOfType<PlayerController>();
    }

    // What the heck are we hitting?
    void OnCollisionEnter(Collision collision)
    {
        soundManager.Stop("Running_1");
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentEnemyHit = collision.gameObject;
            playerController.AttackEnemy();
        }
        else if(collision.gameObject.CompareTag("Rest Point"))
        {
            soundManager.Play("Glass_Shattering");
            soundManager.Play("Healing");
            GetComponent<ParticleSystem>().Play();
            Destroy(collision.gameObject);
            playerController.ResetEnergy();
        }
    }
}

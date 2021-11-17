using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public ParticleSystem explosionParticles;

    // Start is called before the first frame update
    void Start()
    {
        explosionParticles.Play();
        StartCoroutine(ShowEnemy());
    }

    IEnumerator ShowEnemy()
    {
        yield return new WaitForSeconds(0.1f);
        enemyPrefab.SetActive(true);
    }
}

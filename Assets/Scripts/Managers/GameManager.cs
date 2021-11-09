using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameOver;
    [SerializeField] GameObject introUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject endGameUI;
    private SoundManager soundManager;

    // Start is called before the first frame update
    void Awake()
    {
        isGameOver = true;
        soundManager = FindObjectOfType<SoundManager>();
    }

    void Start()
    {
        soundManager.Play("BGM");
    }

    public void StartGame()
    {
        isGameOver = false;
        introUI.SetActive(false);
        inGameUI.SetActive(true);
    }

    public void ShowGameoverUI()
    {
        inGameUI.SetActive(false);
        endGameUI.SetActive(true);
        isGameOver = false;
        endGameUI.GetComponent<Animator>().SetBool("Endgame_b", true);
    }
}

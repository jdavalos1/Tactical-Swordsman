using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameOver;
    [SerializeField][Min(0)] int reviveTax;

    [SerializeField] GameObject introUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject endGameUI;

    private SoundManager soundManager;
    [SerializeField] GameObject solidPlayer;

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

    public void Revive()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        if (pc.killCount >= reviveTax)
        {
            pc.killCount -= reviveTax;
            pc.ResetEnergy();
            inGameUI.SetActive(true);
            endGameUI.SetActive(false);
            endGameUI.GetComponent<Animator>().SetBool("Endgame_b", false);
            solidPlayer.GetComponent<Animator>().SetBool("Recover_b", true);
            solidPlayer.GetComponent<Animator>().SetBool("Death_b", false);
        }
    }
}

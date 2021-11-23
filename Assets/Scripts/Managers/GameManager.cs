using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameOver;
    public bool isPaused;
    [SerializeField] [Min(0)] int reviveTax;

    [SerializeField] GameObject introUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject endGameUI;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject tutorialUI;

    // Buttons to display after returning to tutorial
    [SerializeField] GameObject inGameMenuButtons;

    [SerializeField] GameObject titleUI;
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

    public void BeginIntro()
    {
        introUI.GetComponent<Animator>().SetBool("Begin_b", true);
    }

    /// <summary>
    /// Pause the game and show the menu
    /// </summary>
    public void PauseGame()
    {
        // The tutorial will handle its own esc key press not the pause
        if (tutorialUI.activeSelf) return;
        if (!(endGameUI.activeSelf || introUI.activeSelf))
        {
            if (!isPaused)
            {
                isGameOver = true;
                pauseMenu.SetActive(true);
                inGameUI.SetActive(false);
                Time.timeScale = 0.0f;
                isPaused = true;
            }
            else
            {
                isPaused = false;
                isGameOver = false;
                pauseMenu.SetActive(false);
                inGameUI.SetActive(true);
                Time.timeScale = 1.0f;
            }
        }
    }

    public void ShowGameoverUI()
    {
        inGameUI.SetActive(false);
        endGameUI.SetActive(true);
        isGameOver = false;
        endGameUI.GetComponent<Animator>().SetBool("Endgame_b", true);
    }

    public void ShowTutorialInTitle()
    {
        titleUI.SetActive(false);
        tutorialUI.SetActive(true);
        tutorialUI.GetComponent<HideTutorial>().returnMenu = titleUI;
    }

    public void ShowTutorialInGame()
    {
        inGameMenuButtons.SetActive(false);
        tutorialUI.SetActive(true);
        tutorialUI.GetComponent<HideTutorial>().returnMenu = inGameMenuButtons;
    }

    public void Revive()
    {
        PlayerController pc = FindObjectOfType<PlayerController>();

        if (pc.killCount >= reviveTax)
        {
            isGameOver = false;
            pc.ReduceScore(reviveTax);
            pc.ResetEnergy();
            inGameUI.SetActive(true);
            endGameUI.SetActive(false);
            endGameUI.GetComponent<Animator>().SetBool("Endgame_b", false);
            solidPlayer.GetComponent<Animator>().SetBool("Recover_b", true);
            solidPlayer.GetComponent<Animator>().SetBool("Death_b", false);
        }
    }

    public void ShowHeroIntroduction()
    {
        titleUI.SetActive(false);
        introUI.GetComponent<Animator>().SetBool("Begin_b", false);
    }

    public void StartGame()
    {
        isGameOver = false;
        introUI.SetActive(false);
        inGameUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

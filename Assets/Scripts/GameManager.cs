using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameOver;
    [SerializeField] GameObject introUI;
    [SerializeField] GameObject inGameUI;
    // Start is called before the first frame update
    void Start()
    {
        isGameOver = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        Debug.Log("Pressed");
        isGameOver = false;
        introUI.SetActive(false);
        inGameUI.SetActive(true);
    }
}

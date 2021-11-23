using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTutorial : MonoBehaviour
{
    [SerializeField] GameObject tutorialUI;
    [HideInInspector] public GameObject returnMenu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && tutorialUI.activeSelf)
        {
            tutorialUI.SetActive(false);
            returnMenu.SetActive(true);
        }
    }
}
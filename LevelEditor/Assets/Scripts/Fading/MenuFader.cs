﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFader : MonoBehaviour
{
    public Animator animator;
    public Button mainMenu;
    public Button importMenu;
    public int mainMenuLevel; // Level that we need to load 
    public int importMenuLevel;
    private int levelToLoad;


    public void FadeToLevel(int levelIndex) // Fading with the fadeOut animation
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete() // Load level after fade
    {
        SceneManager.LoadScene(levelToLoad);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (mainMenu.IsActive())
        {
            levelToLoad = mainMenuLevel;
            FadeToLevel(mainMenuLevel);
        }
        else if (importMenu.IsActive()) 
        {
            levelToLoad = importMenuLevel;
            FadeToLevel(importMenuLevel);
        }
    }
}

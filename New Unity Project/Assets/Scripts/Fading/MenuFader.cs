using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFader : MonoBehaviour
{
    public Animator animator;
    private int levelToLoad;

    public void QuitMenu()
    {
        Debug.Log("QUIT!!");
        Application.Quit();
    }

    public void FadeToLevel(int levelIndex) // Fading with the fadeOut animation
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete() // Load level after fade
    {
        SceneManager.LoadScene(levelToLoad);
    }
}

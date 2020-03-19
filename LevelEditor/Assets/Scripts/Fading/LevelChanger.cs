using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChanger : MonoBehaviour
{

    public Animator animator; 
    public int levelToLoad; // Level that we need to load 

    [SerializeField]
    private string test = "fiyezkjl3";
    public LevelGeneration lg;

    public static int size;

    [SerializeField]
    public Text text;


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
        if (levelToLoad == 0)
        {
            size = int.Parse(text.text);
            Debug.Log(text.text);
        }
       
        if (levelToLoad == 1)
        {
            Debug.Log(lg);
            //lg.SetContainsRivers(false);
            //lg.SetSize(size, size);
            Debug.Log("oui");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FadeToLevel(levelToLoad);
        }
    }
}

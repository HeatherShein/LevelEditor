using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChanger : MonoBehaviour
{

    public Animator animator; 
    public int levelToLoad; // Level that we need to load 

    // Parameters of the level
    private int mapWidthInTiles, mapDepthInTiles;
    private bool containsTrees, containsRivers;
    private int numberOfRivers;
    private float riverThreshold, waterThreshold, grassThreshold, mountainThreshold, snowThreshold;
    private int treeSize;
    private float desert, savane, tundra, boreal, tropical;
    private float mapScale;
    private float mountainHeight;
    private Color riverColor, waterColor, grassColor, mountainColor, snowColor;

    // Generator 
    public LevelGeneration levelGeneration;
    public RiverGeneration riverGeneration;
    public TreeGeneration treeGeneration;
    public TileGeneration tileGeneration;


    public void FadeToLevel(int levelIndex) // Fading with the fadeOut animation
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete() // Load level after fade
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void createMap()
    {

    }

    public void setParameters() 
    {
        // Level
        levelGeneration.setTileSize(mapWidthInTiles, mapDepthInTiles);
        levelGeneration.setContainsRivers(containsRivers);
        levelGeneration.setContainsTrees(containsTrees);

        // River
        riverGeneration.setNbRivers(numberOfRivers);
        riverGeneration.setRiverColor(riverColor);
        riverGeneration.setHeightThreshold(riverThreshold);

        // Tree
        tileGeneration.setMapScale(mapScale);


    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}

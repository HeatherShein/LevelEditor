using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

[System.Serializable]
public class LevelFader : MonoBehaviour
{

    public Animator animator; 
    public int levelToLoad; // Level that we need to load 

    // Parameters of the level
    //General 
    private static int XTen, XNumber, YTen, YNumber;
    private static bool containsTrees, containsRivers, containsRocks;

    // Environment
    private static float waterThreshold, grassThreshold, mountainThreshold, snowThreshold;
    private static float mountainHeight, mapScale;
    private static Color waterColor, grassColor, mountainColor, snowColor;

    // River and Forest
    public static int riverTen, riverNumber, treeSize;
    private static float treeTropical, treeDesert, treeSavane, treeTundra, treeBoreal, riverTropical, riverDesert, riverSavane, riverTundra, riverBoreal, riverThreshold;
    private static Color riverColor;


    // Input of the menu
    // General 
    public Dropdown XTenDD, XNumberDD, YTenDD, YNumberDD;
    public Toggle containsRiversT, containsTreesT, containsRocksT;

    // Environment
    public Slider mapScaleS, mountainHeightS, waterThresholdS, grassThresholdS, mountainThresholdS, snowThresholdS;
    public ColorPicker waterColorCP, grassColorCP, mountainColorCP, snowColorCP;

    // River and Forest
    public ColorPicker riverColorCP;
    public Dropdown riverTenDD, riverNumberDD, treeSizeDD;
    public Slider treeTropicalS, treeDesertS, treeSavaneS, treeTundraS, treeBorealS, riverTropicalS, riverDesertS, riverSavaneS, riverTundraS, riverBorealS, riverThresholdS;
    

    // Generator 
    public LevelGeneration levelGeneration;
    public RiverGeneration riverGeneration;
    public TreeGeneration treeGeneration;
    public TileGeneration tileGeneration;


    public void FadeToLevel(int levelIndex) // Fading with the fadeOut animation
    {
        CreateMap();
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete() // Load level after fade
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void CreateMap()
    {
        
            XTen = (int)Mathf.Pow(10f, XTenDD.value);
            YTen = (int)Mathf.Pow(10f, YTenDD.value);
            XNumber = XNumberDD.value;
            YNumber = YNumberDD.value;
            containsRivers = containsRiversT.isOn;
            containsRocks = containsRocksT.isOn;
            containsTrees = containsTreesT.isOn;

            // Environment

            mapScale = mapScaleS.value;
            mountainHeight = mountainHeightS.value;
            waterThreshold = waterThresholdS.value;
            grassThreshold = grassThresholdS.value;
            mountainThreshold = mountainThresholdS.value;
            snowThreshold = snowThresholdS.value;
            waterColor = waterColorCP.GetColor();
            grassColor = grassColorCP.GetColor();
            mountainColor = mountainColorCP.GetColor();
            snowColor = snowColorCP.GetColor();

            // River and Forest 

            riverColor = riverColorCP.GetColor();
            riverTen = (int)Mathf.Pow(10f, riverTenDD.value);
            riverNumber = riverNumberDD.value;
            treeTropical = treeTropicalS.value;
            treeDesert = treeDesertS.value;
            treeTundra = treeTundraS.value;
            treeBoreal = treeBorealS.value;
            treeSavane = treeSavaneS.value;
            riverTropical = riverTropicalS.value;
            riverDesert = riverDesertS.value;
            riverSavane = riverSavaneS.value;
            riverTundra = riverTundraS.value;
            riverBoreal = riverBorealS.value;
            riverThreshold = riverThresholdS.value;

            switch (treeSizeDD.value) {
                case 0:
                    treeSize = 1;
                    break;
                case 1:
                    treeSize = 2;
                    break;
                case 2:
                    treeSize = 3;
                    break;
                default:
                    treeSize = 1;
                    break;
            }



    }

    public void SetParameters() 
    {
        
        // Level
        levelGeneration.setTileSize(XTen*XNumber, YTen*YNumber);
        levelGeneration.setContainsRivers(containsRivers);
        levelGeneration.setContainsTrees(containsTrees);
        levelGeneration.setContainsRocks(containsRocks);

        // River
        riverGeneration.setNbRivers(riverTen*riverNumber);
        riverGeneration.setRiverColor(riverColor);
        riverGeneration.setHeightThreshold(riverThreshold);

        // Tree
        treeGeneration.SetMapScale(treeSize);

        // Tile
        tileGeneration.setMapScale(mapScale);
        tileGeneration.setWater(waterColor, waterThreshold);
        tileGeneration.setGrass(grassColor, grassThreshold);
        tileGeneration.setMountain(mountainColor, mountainThreshold);
        tileGeneration.setSnow(snowColor, snowThreshold);
        

    }

    // Start is called before the first frame update
    void Start()
    {
        if (levelToLoad == 1)
        {
            SetParameters();
            levelGeneration.GenerateMap();
        }

    }

    // Update is called once per frame
    void Update()
    {
        //CreateMap();
        Debug.Log(containsRivers);
        //Debug.Log(XTen);
    }
}

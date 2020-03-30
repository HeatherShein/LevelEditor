using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public static int riverTen, riverNumber;
    public static Vector3 treeSize, rockSize;
    private static float treeTropical, treeDesert, treeSavane, treeTundra, treeBoreal, rockTropical, rockDesert, rockSavane, rockTundra, rockBoreal, riverThreshold;
    private static Color riverColor;
    private static float[] treeNeighbor = new float[5];
    private static float[] rockNeighbor = new float[5];


    // Input of the menu
    // General 
    public Dropdown XTenDD, XNumberDD, YTenDD, YNumberDD;
    public Toggle containsRiversT, containsTreesT, containsRocksT;

    // Environment
    public Slider mapScaleS, mountainHeightS, waterThresholdS, grassThresholdS, mountainThresholdS, snowThresholdS;
    public ColorPicker waterColorCP, grassColorCP, mountainColorCP, snowColorCP;

    // River and Forest
    public ColorPicker riverColorCP;
    public Dropdown riverTenDD, riverNumberDD, treeSizeDD, rockSizeDD;
    public Slider treeTropicalS, treeDesertS, treeSavaneS, treeTundraS, treeBorealS, rockTropicalS, rockDesertS, rockSavaneS, rockTundraS, rockBorealS, riverThresholdS;

    // Generator 
    public LevelGeneration levelGeneration;
    public RiverGeneration riverGeneration;
    public TreeGeneration treeGeneration;
    public RockGeneration rockGeneration;
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
            XNumber = XNumberDD.value + 1;
            YNumber = YNumberDD.value + 1;
            containsRivers = containsRiversT.isOn;
            containsRocks = containsRocksT.isOn;
            containsTrees = containsTreesT.isOn;

            // Environment

            mapScale = mapScaleS.value;
            mountainHeight = mountainHeightS.value * 50;
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
            treeTropical = Mathf.Ceil(treeTropicalS.value * 10);
            treeDesert = Mathf.Ceil(treeDesertS.value * 10);
            treeTundra = Mathf.Ceil(treeTundraS.value * 10);
            treeBoreal = Mathf.Ceil(treeBorealS.value * 10);
            treeSavane = Mathf.Ceil(treeSavaneS.value * 10);
            rockTropical = Mathf.Ceil(rockTropicalS.value * 10);
            rockDesert = Mathf.Ceil(rockDesertS.value * 10);
            rockSavane = Mathf.Ceil(rockSavaneS.value * 10);
            rockTundra = Mathf.Ceil(rockTundraS.value);
            rockBoreal = Mathf.Ceil(rockBorealS.value * 10);
            riverThreshold = riverThresholdS.value;

            switch (treeSizeDD.value) {
                case 0:
                    treeSize = new Vector3(0.01f,0.01f,0.01f);
                    break;
                case 1:
                    treeSize = new Vector3(0.05f, 0.05f, 0.05f);
                    break;
                case 2:
                    treeSize = new Vector3(0.1f, 0.1f, 0.1f);
                    break;
                default:
                    treeSize = new Vector3(0.01f, 0.01f, 0.01f);
                    break;
            }

            switch (rockSizeDD.value)
            {
                case 0:
                    rockSize = new Vector3(0.02f, 0.02f, 0.02f);
                    break;
                case 1:
                    rockSize = new Vector3(0.05f, 0.05f, 0.05f);
                    break;
                case 2:
                    rockSize = new Vector3(0.1f, 0.1f, 0.1f);
                    break;
                default:
                    rockSize = new Vector3(0.02f, 0.02f, 0.02f);
                    break;
            }      

        treeNeighbor[0] = treeDesert;
        treeNeighbor[1] = treeSavane;
        treeNeighbor[2] = treeTundra;
        treeNeighbor[3] = treeBoreal;
        treeNeighbor[4] = treeTropical;
        rockNeighbor[0] = rockDesert;
        rockNeighbor[1] = rockSavane;
        rockNeighbor[2] = rockTundra;
        rockNeighbor[3] = rockBoreal;
        rockNeighbor[4] = rockTropical;
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
        treeGeneration.SetLocalScale(treeSize);
        treeGeneration.SetNeighborRadius(treeNeighbor);

        // Rock
        rockGeneration.SetLocalScale(rockSize);
        rockGeneration.SetNeighborRadius(rockNeighbor);

        // Tile
        tileGeneration.setMapScale(mapScale);
        tileGeneration.setWater(waterColor, waterThreshold);
        tileGeneration.setGrass(grassColor, grassThreshold);
        tileGeneration.setMountain(mountainColor, mountainThreshold);
        tileGeneration.setSnow(snowColor, snowThreshold);
        tileGeneration.setHeight(mountainHeight);

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

    public void QuitMenu()
    {
        Debug.Log("QUIT!!");
        Application.Quit();
    }
}

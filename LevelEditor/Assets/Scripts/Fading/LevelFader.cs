﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class LevelFader : MonoBehaviour
{

    public Animator animator; 
    public int levelToLoad; // Level that we need to load 

    // Parameters of the level
    private static int mapWidthInTiles, mapDepthInTiles;
    private static int numberOfRivers;

    private static float riverThreshold, waterThreshold, grassThreshold, mountainThreshold, snowThreshold;
    private static float desert, savane, tundra, boreal, tropical;
    private static float mapScale, treeSize;
    private static float mountainHeight;

    private static bool containsTrees, containsRivers;

    private static Color riverColor, waterColor, grassColor, mountainColor, snowColor;

    // Input of the menu
    public InputField mapWidthInTilesIF, mapDepthInTilesIF, numberOfRiversIF, treeSizeIF, waterThresholdIF, grassThresholdIF, mountainThresholdIF, snowThresholdIF;
    public Toggle containsRiversT, containsTreesT;
    public Slider riverThresholdS, desertS, savaneS, tundraS, borealS, tropicalS;
    public Scrollbar mapScaleSB, mountainHeightSB;
    public ColorPicker riverColorFCP, waterColorFCP, grassColorFCP, mountainColorFCP, snowColorFCP;

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
        try
        {
            // int 
            mapDepthInTiles = int.Parse(mapDepthInTilesIF.text);
            mapWidthInTiles = int.Parse(mapWidthInTilesIF.text);
            numberOfRivers = int.Parse(numberOfRiversIF.text);
            treeSize = int.Parse(treeSizeIF.text);

            // float
            riverThreshold = riverThresholdS.value;
            waterThreshold = float.Parse(waterThresholdIF.text);
            grassThreshold = float.Parse(grassThresholdIF.text);
            mountainThreshold = float.Parse(mountainThresholdIF.text);
            snowThreshold = float.Parse(snowThresholdIF.text);
            desert = desertS.value;
            savane = savaneS.value;
            tundra = tundraS.value;
            boreal = borealS.value;
            tropical = tropicalS.value;
            mapScale = mapScaleSB.value;
            mountainHeight = mountainHeightSB.value;

            // bool
            containsRivers = containsRiversT.isOn;
            containsTrees = containsTreesT.isOn;

            // Color
            riverColor = riverColorFCP.GetColor();
            waterColor = waterColorFCP.GetColor();
            grassColor = grassColorFCP.GetColor();
            mountainColor = mountainColorFCP.GetColor();
            snowColor = snowColorFCP.GetColor();
        } catch (System.Exception e)
        {
            Debug.Log(e);
            Debug.Log("Something is empy");
            SceneManager.LoadScene(1);
        }
        
    }

    public void SetParameters() 
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
        }

    }

    // Update is called once per frame
    void Update()
    {
    }
}

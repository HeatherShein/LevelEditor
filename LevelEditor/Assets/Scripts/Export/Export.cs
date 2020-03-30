using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Export : MonoBehaviour
{
    public void SaveFiles()
    {
        // Copy needed files into another folder
        // Scene
        AssetDatabase.CopyAsset("Assets/Scenes", "Assets/SavedScene/Scenes");
        // Imports
        AssetDatabase.CopyAsset("Assets/Imports/RockPackage", "Assets/SavedScene/Imports/Rocks");
        AssetDatabase.CopyAsset("Assets/Imports/StandardAssets/Environment/SpeedTree", "Assets/SavedScene/Imports/Trees");
        // Prefabs
        AssetDatabase.CopyAsset("Assets/Prefabs", "Assets/SavedScene/Prefabs");
        // Scripts
        AssetDatabase.CopyAsset("Assets/Scripts", "Assets/SavedScene/Scripts");
        // Export them
        AssetDatabase.ExportPackage("Assets/SavedScene/", "H:/SavedScenes/Saved");
    }
}

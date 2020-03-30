using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class Export : MonoBehaviour
{

    public void SaveFiles()
    {

        string[] assetPathNames = new string[5] { "Assets/Scenes/Level.unity", "Assets/Scenes/Level.unity.meta", "Assets/Prefabs", "Assets/Scripts", "Assets/Imports/ColorPicker"};
        //string[] assetPathNamesWithImports = new string[4] { "Assets/Scenes", "Assets/Imports", "Assets/Prefabs", "Assets/Scripts" };

        /*
        System.IO.Directory.CreateDirectory("Assets/SavedScene");
        System.IO.Directory.CreateDirectory("H:/Documents/Unity Projects/Saves");

        // Copy needed files into another folder
        // Scene
        if (AssetDatabase.CopyAsset("Assets/Scenes", "Assets/SavedScene/Scenes"))
        {
            Debug.Log("Scene copied");
        }
        
        // Imports
        AssetDatabase.CopyAsset("Assets/Imports/RockPackage", "Assets/SavedScene/Imports/Rocks");
        AssetDatabase.CopyAsset("Assets/Imports/StandardAssets/Environment/SpeedTree", "Assets/SavedScene/Imports/Trees");
        // Prefabs
        AssetDatabase.CopyAsset("Assets/Prefabs", "Assets/SavedScene/Prefabs");
        // Scripts
        AssetDatabase.CopyAsset("Assets/Scripts", "Assets/SavedScene/Scripts");*/

        // Export them
        AssetDatabase.ExportPackage(assetPathNames, "H:/Documents/Unity Projects/Saves/Save.unitypackage", ExportPackageOptions.Recurse);
    }
}

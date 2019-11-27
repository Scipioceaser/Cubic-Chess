using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetManager
{

    [MenuItem("Assets/Build AssetBundles")]
    static void CreateModelAssets()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Assets/Build AssetBundles (For build)")]
    static void ExportResource()
    {
        string folderName = "AssetBundles";
        string filePath = Path.Combine(Application.streamingAssetsPath, folderName);

        //Build for Windows platform
        BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        //Uncomment to build for other platforms
        //BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.iOS);
        //BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.Android);
        //BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.WebGL);
        //BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);

        //Refresh the Project folder
        AssetDatabase.Refresh();
    }
}

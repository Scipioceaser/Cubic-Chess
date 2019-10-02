using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Unit : MonoBehaviour
{   
    public void SetModelFromAssets(GameObject objectToAddModel, string modelAssetBundle, string modelName)
    {
       string p = Path.Combine(Application.dataPath, "AssetBundles");
       var modelBundle = AssetBundle.LoadFromFile(Path.Combine(p, modelAssetBundle));

        if (modelBundle == null)
        {
            Debug.LogWarning("Failed to load " + modelAssetBundle);
            return;
        }
        
        GameObject prefab = modelBundle.LoadAsset<GameObject>(modelName);
        objectToAddModel.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        objectToAddModel.GetComponent<MeshRenderer>().sharedMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
    }
}

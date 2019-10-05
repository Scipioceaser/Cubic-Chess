﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Unit : MonoBehaviour
{
    [HideInInspector]
    public Node currentNode;

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
        modelBundle.Unload(false);
    }
    
    //TODO: Add particle effect when node mesh lands at destination. Some shake would be good too.
    public IEnumerator Move(Vector3 startPos, Vector3 endPos, float timeValue)
    {
        float r = 1.0f / timeValue;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * r;
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0.0f, 1.0f, t));
            
            yield return null;
        }
    }

    public virtual List<Vector3> GetValidMovePositions(Vector3 position)
    {
        // Don't copy this bit
        return null;
    }
}

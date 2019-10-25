﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Unit : MonoBehaviour
{
    [HideInInspector]
    public Node currentNode;
    [HideInInspector]
    public bool moving;
    [HideInInspector]
    public int moveIndex = 0;
    [HideInInspector]
    public List<Vector3> positions;
    //[HideInInspector]
    public Direction spawnDir;
    [HideInInspector]
    public Vector3 unAdjustedPosition;
    [HideInInspector]
    public Map map;
    [HideInInspector]
    public Vector3 moveDirection;

    private MeshRenderer meshrender;
    private MeshFilter meshfilter;
    private Color outlineColorDefault = Color.black;

    public virtual void Awake()
    {
        meshrender = GetComponent<MeshRenderer>();
        meshfilter = GetComponent<MeshFilter>();

        map = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
    }

    public void SetModelFromAssets(GameObject objectToAddModel, string modelAssetBundle, string modelName, string shaderName = "Standard")
    {
       string p = Path.Combine(Application.dataPath, "AssetBundles");
       var modelBundle = AssetBundle.LoadFromFile(Path.Combine(p, modelAssetBundle));

       if (modelBundle == null)
       {
           Debug.LogWarning("Failed to load " + modelAssetBundle);
           return;
       }

        Material mat = new Material(Shader.Find("Outline"));

        if (shaderName == "Outline")
        {
            mat.SetFloat("_OutlineWidth", 1.035f);
            mat.SetColor("_OutlineColor", Color.blue);
        }
        
       GameObject prefab = modelBundle.LoadAsset<GameObject>(modelName);
       objectToAddModel.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
       objectToAddModel.GetComponent<MeshRenderer>().sharedMaterial = mat;
       //objectToAddModel.GetComponent<MeshRenderer>().sharedMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
       modelBundle.Unload(false);
    }

    public void SetOutlineWidthAndColor(float width = 1.01f)
    {
        meshrender.sharedMaterial.SetFloat("_OutlineWidth", width);
    }

    public void SetOutlineWidthAndColor(Color color, float width = 1.01f)
    {
        meshrender.sharedMaterial.SetFloat("_OutlineWidth", width);
        meshrender.sharedMaterial.SetColor("_OutlineColor", color);
    }
    
    public bool IsLineStraight(float A1, float A2, float B1, float B2)
    {
        return Mathf.Abs(A2 - A1) == Mathf.Abs(B2 - B1);
    }
    
    public float LineDelta(float A, float B)
    {
        return Mathf.Abs(B - A);
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

    //TODO: Add check so that units who can move long distances need to be at edge to go to the sides
    public virtual List<Vector3> GetValidMovePositions(Vector3 position, int team = 1)
    {
        // Don't copy this bit
        return null;
    }

    public void SetPositions(List<Vector3> movePositions)
    {
        positions = movePositions;
    }

    public static Vector3 UnitDirectionToVectorDirection(Direction dir, bool multiplyGlobalScale = false)
    {
        Vector3 d = Vector3.zero;

        switch (dir)
        {
            case Direction.FORWARD:
                d = Vector3.forward;
                break;
            case Direction.BACK:
                d = Vector3.back;
                break;
            case Direction.LEFT:
                d = Vector3.left;
                break;
            case Direction.RIGHT:
                d = Vector3.right;
                break;
            case Direction.UP:
                d = Vector3.up;
                break;
            case Direction.DOWN:
                d = Vector3.down;
                break;
            default:
                break;
        }

        if (multiplyGlobalScale)
            d *= Globals.scale;

        return d;
    }

    public void AlignUnit(Vector3 destination)
    {
        Vector3 d = (destination - UnitSpawnPoint.GetNearestNode(destination, 1, true).position);
        //transform.LookAt(transform.localPosition + d);
        transform.rotation = Quaternion.FromToRotation(Vector3.up, d);
    }

    //TODO: Add loop to move along all points
    public virtual void MoveAlongPath(Vector3 destination = new Vector3()) { }
}

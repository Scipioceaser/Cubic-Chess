using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Fix issue where moving map generator creates weird grid assignment.

public class Generate : MonoBehaviour
{
    [Range(1, 50)]
    public int sizeX = 1;
    [Range(1, 50)]
    public int sizeY = 1;
    [Range(1, 50)]
    public int sizeZ = 1;

    [Range(1, 10)]
    public int nodeScale = 1;
    [HideInInspector]
    public GameObject[] grid;
    
    private int gLength;
    private int gWidth;
    private int gHeight;

    // Start is called before the first frame update
    void Awake()
    {
        CreateNodes();
    }

    private void CreateNodes()
    {
        int l, w, h;

        l = sizeZ + 2;
        w  = sizeX + 2;
        h = sizeY + 2;
        
        if (grid.Length != 0)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                Vector3 pos = grid[i].transform.position;
                Node n;

                if (pos.y != 0 && pos.y != (h - 1) && pos.x != 0 && pos.x != (w - 1) && pos.z != 0 && pos.z != (l - 1))
                {
                    n = grid[i].AddComponent<Node>();
                    n.Init(nodeScale, transform.position + new Vector3(pos.x, pos.y, pos.z));
                }
                else
                {
                    n = grid[i].AddComponent<NodeMesh>();
                    n.Init(nodeScale, transform.position + new Vector3(pos.x, pos.y, pos.z));
                }
            }
        }
    }

    public void CreateMap()
    {
        int length, width, height;

        length = gLength = sizeZ + 2;
        width = gWidth = sizeX + 2;
        height = gHeight = sizeY + 2;

        grid = new GameObject[width * height * length];
        
        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameObject nodeObject = new GameObject("Node");

                    nodeObject.transform.parent = transform;
                    nodeObject.transform.position = new Vector3(x, y, z) + transform.position;
                    grid[i] = nodeObject;
                    i++;
                }
            }
        }

        if (Application.isPlaying)
            CreateNodes();
    }

    public void DestroyMap()
    {
       
        for (int i = 0; i < grid.Length; i++)
        {
            DestroyImmediate(grid[i]);
        }

        grid = new GameObject[0];
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        
        GameObject[] gridOld = grid;

        if (gridOld == grid)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                Vector3 p = grid[i].transform.position;

                if (p.y != 0 && p.y != (gHeight - 1) &&
                    p.x != 0 && p.x != (gWidth - 1) &&
                    p.z != 0 && p.z != (gLength - 1))
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawSphere(grid[i].transform.position, (float)nodeScale / 10f);
            }
        }
        else
        {
            gridOld = grid;
        }
    }
}
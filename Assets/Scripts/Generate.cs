using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Node[] grid;

    // Start is called before the first frame update
    void Awake()
    {
    }

    public void CreateMap()
    {
        int length = sizeZ+2;
        int width = sizeX+2;
        int height = sizeY+2;

        grid = new Node[width * height * length];

        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < length; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    GameObject nodeObject = new GameObject("Node");
                    Node n;

                    if (y != 0 && y != (height - 1) && x != 0 && x != (width - 1) && z != 0 && z != (length - 1))
                    {
                        n = nodeObject.AddComponent<Node>();
                        n.Init(nodeScale, transform.position + new Vector3(x, y, z));
                        grid[i] = n;
                    }
                    else
                    {
                        n = nodeObject.AddComponent<NodeMesh>();
                        n.Init(nodeScale, transform.position + new Vector3(x, y, z));
                        grid[i] = n;
                    }
                    i++;
                }
            }
        }
    }

    public void DestroyMap()
    {
       
        for (int i = 0; i < grid.Length; i++)
        {
            DestroyImmediate(grid[i].gameObject);
        }

        grid = new Node[0];
    }
}
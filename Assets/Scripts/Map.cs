using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    NONE=0,
    INTERIOR_EMPTY,
    EXTERIOR_EMPTY,
}

public class Map : MonoBehaviour
{
    [Range(1, 50)]
    public int boardSize = 1;
    [Range(1, 50)]
    public int boardHeight = 1;
    [Range(0.1f, 10f)]
    public float nodeDropSpeed = 0.5f;
    [Range(1f, 10f)]
    public float nodePlacementSpeed = 1f;
    [Range(1, 10)]
    public int nodeScale = 1;
    public bool dropWithAnimation = true;
    
    public Color colorEven = Color.white;
    public Color colorOdd = Color.black;

    [HideInInspector]
    public GameObject[] grid;

    // 0 = No map, 1 = Interior map, 2 = exterior map
    [SerializeField]//[HideInInspector]
    public MapType mapType = MapType.NONE;

    [HideInInspector]
    public Node[,,] nodes;

    private void Awake()
    {
        if (mapType == MapType.INTERIOR_EMPTY)
        {
            StartCoroutine(CreateNodesInterior());
        }
        else if (mapType == MapType.EXTERIOR_EMPTY)
        {
            StartCoroutine(CreateNodesExterior());
        }
    }

    // Have to add these in for the gizmos not do draw in a weird way after re-entering scene mode
    [SerializeField][HideInInspector]
    private int gizmoGridSize = 1;
    [SerializeField][HideInInspector]
    private int gizmoGridHeight = 1;

    #region Node and Position functions
    public Vector3 WorldPosFromNodePos(Vector3 nodePosition)
    {
        return (nodePosition + transform.position);
    }

    public Node NodeFromNodeVector(Vector3 nodePosition)
    {
        return nodes[(int)nodePosition.x, (int)nodePosition.y, (int)nodePosition.z];
    }

    public Node NodeFromWorldPoints(int x, int y, int z)
    {
        return nodes[x, y, z];
    }
    #endregion

    public IEnumerator CreateNodesExterior()
    {
        WaitForSeconds wait = new WaitForSeconds((nodePlacementSpeed / 100f));

        Material even = new Material(Shader.Find("Standard"));
        Material odd = new Material(Shader.Find("Standard"));
        even.color = colorEven;
        odd.color = colorOdd;

        nodes = new Node[(boardSize + 2), (boardHeight + 2), (boardSize + 2)];
        
        if (grid.Length != 0)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                Vector3 pos = grid[i].transform.position;

                if (pos.y == 0 || pos.y == (boardHeight + 1) || pos.x == 0 || pos.x == (boardSize + 1) || pos.z == 0 || pos.z == (boardSize + 1))
                {
                    Node n;

                    n = grid[i].AddComponent<Node>();
                    n.Init(nodeScale, pos);

                    nodes[(int)pos.x, (int)pos.y, (int)pos.z] = n;
                }
                else
                {
                    NodeMesh n;
                    n = grid[i].AddComponent<NodeMesh>();

                    if (pos.y % 2 == 0)
                    {
                        if (pos.x % 2 == 0 && pos.z % 2 == 0 || pos.x % 2 != 0 && pos.z % 2 != 0)
                        {
                            n.SetColor(even);
                        }
                        else
                        {
                            n.SetColor(odd);
                        }
                    }
                    else
                    {
                        if (pos.x % 2 == 0 && pos.z % 2 == 0 || pos.x % 2 != 0 && pos.z % 2 != 0)
                        {
                            n.SetColor(odd);
                        }
                        else
                        {
                            n.SetColor(even);
                        }
                    }
                    
                    n.Init(nodeScale, pos);

                    nodes[(int)pos.x, (int)pos.y, (int)pos.z] = n;

                    if (dropWithAnimation)
                        StartCoroutine(n.Move(transform.position + new Vector3(pos.x, pos.y, pos.z) +
                            new Vector3(0, (boardHeight + 25), 0), transform.position + new Vector3(pos.x, pos.y, pos.z), nodeDropSpeed));

                    yield return wait;
                }
            }
        }
    }

    public IEnumerator CreateNodesInterior()
    {
        WaitForSeconds wait = new WaitForSeconds((nodePlacementSpeed / 100f));
        Material even = new Material(Shader.Find("Standard"));
        Material odd = new Material(Shader.Find("Standard"));
        even.color = colorEven;
        odd.color = colorOdd;

        int l, w, h;
        
        l = boardSize + 2;
        w  = boardSize + 2;
        h = boardHeight + 2;

        nodes = new Node[w, h, l];

        if (grid.Length != 0)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                Vector3 pos = grid[i].transform.position;

                if (pos.y != 0 && pos.y != (h - 1) && pos.x != 0 && pos.x != (w - 1) && pos.z != 0 && pos.z != (l - 1))
                {
                    Node n;

                    n = grid[i].AddComponent<Node>();
                    n.Init(nodeScale, pos);

                    nodes[(int)pos.x, (int)pos.y, (int)pos.z] = n;
                }
                else
                {
                    NodeMesh n;

                    n = grid[i].AddComponent<NodeMesh>();

                    #region Color Assignment
                    if (pos.y == 0)
                    {
                        if (pos.x == 0)
                        {
                            if (pos.z % 2 != 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else if (pos.z == 0)
                        {
                            if (pos.x % 2 != 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else if (pos.x == (w - 1))
                        {
                            if (pos.z % 2 == 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else if (pos.z == (l - 1))
                        {
                            if (pos.x % 2 == 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else
                        {
                            if (pos.x % 2 != 0 && pos.z % 2 != 0 || pos.x % 2 == 0 && pos.z % 2 == 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                    }
                    else if (pos.y == (h - 1))
                    {
                        if (pos.x % 2 != 0 && pos.z % 2 != 0 || pos.x % 2 == 0 && pos.z % 2 == 0)
                        {
                            n.SetColor(even);
                        }
                        else
                        {
                            n.SetColor(odd);
                        }
                    }
                    else
                    {
                        if (pos.x == 0)
                        {
                            if (pos.z % 2 == 0 && pos.y % 2 != 0 || pos.z % 2 != 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else if (pos.x == (w - 1))
                        {
                            if (pos.z % 2 != 0 && pos.y % 2 != 0 || pos.z % 2 == 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else if (pos.z == 0)
                        {
                            if (pos.x % 2 == 0 && pos.y % 2 != 0 || pos.x % 2 != 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else if (pos.z == (l - 1))
                        {
                            if (pos.x % 2 != 0 && pos.y % 2 != 0 || pos.x % 2 == 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even);
                            }
                            else
                            {
                                n.SetColor(odd);
                            }
                        }
                        else
                        {
                            n.SetColor(even);
                        }
                    }
                    #endregion

                    n.Init(nodeScale, pos);

                    nodes[(int)pos.x, (int)pos.y, (int)pos.z] = n;

                    if(dropWithAnimation)
                        StartCoroutine(n.Move(transform.position + new Vector3(pos.x, pos.y, pos.z) +
                            new Vector3(0, (boardHeight + 25), 0), transform.position + new Vector3(pos.x, pos.y, pos.z), nodeDropSpeed));

                    yield return wait;
                }
            }
        }
    }

    public void CreateMapExterior()
    {
        if (grid.Length != 0)
            DestroyMap();

        grid = new GameObject[(boardSize + 2) * (boardHeight + 2) * (boardSize + 2)];

        int i = 0;
        for (int y = 0; y < (boardHeight + 2);  y++)
        {
            for (int z = 0; z < (boardSize + 2); z++)
            {
                for (int x = 0; x < (boardSize + 2); x++)
                {
                    GameObject nodeObject = new GameObject("Node");

                    nodeObject.transform.parent = transform;
                    nodeObject.transform.position = new Vector3(x, y, z);
                    grid[i] = nodeObject;
                    i++;
                }
            }
        }

        mapType = MapType.EXTERIOR_EMPTY;

        if (Application.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(CreateNodesExterior());
        }
            
    }

    public void CreateMapInterior()
    {
        if (grid.Length != 0)
            DestroyMap();

        grid = new GameObject[(boardSize + 2) * (boardHeight + 2) * (boardSize + 2)];
        
        int i = 0;
        for (int y = 0; y < (boardHeight + 2); y++)
        {
            for (int z = 0; z < (boardSize + 2); z++)
            {
                for (int x = 0; x < (boardSize + 2); x++)
                {
                    GameObject nodeObject = new GameObject("Node");

                    nodeObject.transform.parent = transform;
                    nodeObject.transform.position = new Vector3(x, y, z);
                    grid[i] = nodeObject;
                    i++;
                }
            }
        }
        
        gizmoGridHeight = boardHeight;
        gizmoGridSize = boardSize;

        mapType = MapType.INTERIOR_EMPTY;

        if (Application.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(CreateNodesInterior());
        }
    }

    public void DestroyMap()
    {
       
        for (int i = 0; i < grid.Length; i++)
        {
            DestroyImmediate(grid[i]);
        }

        grid = new GameObject[0];

        if (Application.isPlaying)
            StopAllCoroutines();

        mapType = MapType.NONE;
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        int s = gizmoGridSize + 2;
        int h = gizmoGridHeight + 2;

        if (mapType == MapType.INTERIOR_EMPTY)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                Vector3 p = grid[i].transform.position;

                if (p.y != 0 && p.y != (h - 1) &&
                    p.x != 0 && p.x != (s - 1) &&
                    p.z != 0 && p.z != (s - 1))
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawSphere(grid[i].transform.position + transform.position, (float)nodeScale / 10f);
            }
        }
        else if (mapType == MapType.EXTERIOR_EMPTY)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                Vector3 p = grid[i].transform.position;

                if (p.y == 0 || p.y == (boardHeight + 1) || p.x == 0 || p.x == (boardSize + 1) || p.z == 0 || p.z == (boardSize + 1))
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawSphere(grid[i].transform.position + transform.position, (float)nodeScale / 10f);
            }
        }
    }
}
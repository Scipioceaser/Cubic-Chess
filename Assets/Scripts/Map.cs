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
    [Range(1, 20)]
    public int boardSize = 1;
    [Range(1, 20)]
    public int boardHeight = 1;
    [Range(0.1f, 10f)]
    public float nodeDropSpeed = 0.5f;
    [Range(1, 10)]
    public int nodePlacementSpeed = 1;
    [Range(1, 10)]
    public int nodeScale = 1;
    public bool dropWithAnimation = true;
    
    public Color colorEven = Color.white;
    public Color colorOdd = Color.black;
    
    [HideInInspector]
    public GameObject[] grid;

    [SerializeField]
    public MapType mapType;

    [HideInInspector]
    public Node[,,] nodes;
    private List<Vector3> AllNodePositions = new List<Vector3>();
    
    // Have to add these in for the gizmos not do draw in a weird way after re-entering scene mode
    [SerializeField][HideInInspector]
    private int gizmoGridSize = 1;
    [SerializeField][HideInInspector]
    private int gizmoGridHeight = 1;

    private void Awake()
    {
        Globals.scale = nodeScale;
        Globals.mapSize = boardSize;
        Globals.mapHeight = boardHeight;

        if (mapType == MapType.INTERIOR_EMPTY)
        {
            StartCoroutine(CreateNodesInterior());
        }
        else if (mapType == MapType.EXTERIOR_EMPTY)
        {
            StartCoroutine(CreateNodesExterior());
        }
    }
    
    #region Node and Position functions
    public Vector3 WorldPosFromNodePos(Vector3 nodePosition)
    {
        return (nodePosition + transform.position);
    }

    public Node NodeFromNodeVector(Vector3 nodePosition)
    {
        if (AllNodePositions.Contains(nodePosition))
        {
            return nodes[Mathf.RoundToInt(nodePosition.x), Mathf.RoundToInt(nodePosition.y), Mathf.RoundToInt(nodePosition.z)];
        }
        else
        {
            Debug.LogWarning("Node not found at: " + nodePosition);
            return null;
        }
    }

    public Node NodeFromWorldPoints(Vector3 position)
    {
        Node n = null;
        
        n = nodes[(int)position.x, (int)position.y, (int)position.z];

        if (n == null)
            Debug.LogWarning("No Node Selected at position: " + position);

        return n;
    }

    public List<Node> GetNeighbours(Node node, int depth = 1)
    {
        List<Node> neighbours = new List<Node>();

        for (int z = -depth; z <= depth; z++)
        {
            for (int x = -depth; x <= depth; x++)
            {
                for (int y = -depth; y <= depth; y++)
                {
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    int cx = (int)node.position.x + x;
                    int cy = (int)node.position.y + y;
                    int cz = (int)node.position.z + z;

                    if (cx >= 0 && cx < (boardSize + 2) && cy >= 0 && cy < (boardHeight + 2) && cz >= 0 && cz < (boardSize + 2))
                    {
                        neighbours.Add(nodes[cx, cy, cz]);
                    }
                }
            }
        }
    
        return neighbours;
    }
    #endregion

    private void DetermineExteriorNodesCentreRender()
    {
        foreach (Node node in nodes)
        {
            if (node != null && node.GetType() == typeof(NodeMesh))
            {
                Vector3 pos = node.transform.position;

                if (pos.y > 1 && pos.y <= (boardHeight - 1) && pos.z > 1 && pos.z <= (boardSize - 1) && pos.x > 1 && pos.x <= (boardSize - 1))
                {
                    node.SendMessage("SetRender", false);
                }
            }
        }
    }

    public void ResetColors()
    {
        foreach (Node node in nodes)
        {
            if (node.GetType() == typeof(NodeMesh))
            {
                NodeMesh n = (NodeMesh)node;
                n.SetColor(n.color.color);
            }
        }
    }

    public IEnumerator CreateNodesExterior()
    {
        WaitForSeconds wait = new WaitForSeconds(((float)nodePlacementSpeed / 1000f));

        Material even = new Material(Shader.Find("Node"));
        Material odd = new Material(Shader.Find("Node"));
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
                    n.nodeCollider.size = new Vector3(nodeScale, nodeScale, nodeScale);
                    n.nodeCollider.isTrigger = true;

                    AllNodePositions.Add(n.position);
                    nodes[(int)pos.x, (int)pos.y, (int)pos.z] = n;
                }
                else
                {
                    NodeMesh n;
                    n = grid[i].AddComponent<NodeMesh>();
                    n.transform.name = "NodeMesh";

                    
                    
                    n.Init(nodeScale, pos);
                    n.nodeCollider.size = new Vector3(nodeScale, nodeScale, nodeScale);

                    if (pos.y % 2 == 0)
                    {
                        if (pos.x % 2 == 0 && pos.z % 2 == 0 || pos.x % 2 != 0 && pos.z % 2 != 0)
                        {
                            n.SetColor(even.color);
                            n.color = even;
                        }
                        else
                        {
                            n.SetColor(odd.color);
                            n.color = odd;
                        }
                    }
                    else
                    {
                        if (pos.x % 2 == 0 && pos.z % 2 == 0 || pos.x % 2 != 0 && pos.z % 2 != 0)
                        {
                            n.SetColor(odd.color);
                            n.color = odd;
                        }
                        else
                        {
                            n.SetColor(even.color);
                            n.color = even;
                        }
                    }

                    AllNodePositions.Add(n.position);
                    nodes[(int)pos.x, (int)pos.y, (int)pos.z] = n;
                    
                    if (dropWithAnimation)
                        StartCoroutine(n.Move(transform.position + new Vector3(pos.x, pos.y, pos.z) +
                            new Vector3(0, (boardHeight + 25), 0), transform.position + new Vector3(pos.x, pos.y, pos.z), nodeDropSpeed));

                    yield return wait;
                }
            }
            //TODO: This could be optimized.
            Camera.main.gameObject.GetComponent<CameraController>().SendMessage("SetCentrePosition");
            DetermineExteriorNodesCentreRender();
        }
    }

    public IEnumerator CreateNodesInterior()
    {
        WaitForSeconds wait = new WaitForSeconds(((float)nodePlacementSpeed / 1000f));
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
                    n.nodeCollider.size = new Vector3(nodeScale, nodeScale, nodeScale);
                    n.nodeCollider.isTrigger = true;

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
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else if (pos.z == 0)
                        {
                            if (pos.x % 2 != 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else if (pos.x == (w - 1))
                        {
                            if (pos.z % 2 == 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else if (pos.z == (l - 1))
                        {
                            if (pos.x % 2 == 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else
                        {
                            if (pos.x % 2 != 0 && pos.z % 2 != 0 || pos.x % 2 == 0 && pos.z % 2 == 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                    }
                    else if (pos.y == (h - 1))
                    {
                        if (pos.x % 2 != 0 && pos.z % 2 != 0 || pos.x % 2 == 0 && pos.z % 2 == 0)
                        {
                            n.SetColor(even.color);
                        }
                        else
                        {
                            n.SetColor(odd.color);
                        }
                    }
                    else
                    {
                        if (pos.x == 0)
                        {
                            if (pos.z % 2 == 0 && pos.y % 2 != 0 || pos.z % 2 != 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else if (pos.x == (w - 1))
                        {
                            if (pos.z % 2 != 0 && pos.y % 2 != 0 || pos.z % 2 == 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else if (pos.z == 0)
                        {
                            if (pos.x % 2 == 0 && pos.y % 2 != 0 || pos.x % 2 != 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else if (pos.z == (l - 1))
                        {
                            if (pos.x % 2 != 0 && pos.y % 2 != 0 || pos.x % 2 == 0 && pos.y % 2 == 0)
                            {
                                n.SetColor(even.color);
                            }
                            else
                            {
                                n.SetColor(odd.color);
                            }
                        }
                        else
                        {
                            n.SetColor(even.color);
                        }
                    }
                    #endregion

                    n.Init(nodeScale, pos);
                    n.nodeCollider.size = new Vector3(nodeScale, nodeScale, nodeScale);

                    nodes[(int)pos.x, (int)pos.y, (int)pos.z] = n;

                    if(dropWithAnimation)
                        StartCoroutine(n.Move(transform.position + new Vector3(pos.x, pos.y, pos.z) +
                            new Vector3(0, (boardHeight + 25), 0), transform.position + new Vector3(pos.x, pos.y, pos.z), nodeDropSpeed));

                    yield return wait;
                }
            }
            Camera.main.gameObject.GetComponent<CameraController>().SendMessage("SetCentrePosition");
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
                    GameObject nodeObject = null;

                    if (y == 0 || y == (boardHeight + 1) || x == 0 || x == (boardSize + 1) || z == 0 || z == (boardSize + 1))
                    {
                        nodeObject = new GameObject("Node");
                        BoxCollider c = nodeObject.AddComponent<BoxCollider>();
                        c.size = new Vector3(nodeScale, nodeScale, nodeScale);
                        c.isTrigger = true;
                    }
                    else
                    {
                        nodeObject = new GameObject("NodeMesh");
                        BoxCollider c = nodeObject.AddComponent<BoxCollider>();
                        c.size = new Vector3(nodeScale, nodeScale, nodeScale);
                    }
                    
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
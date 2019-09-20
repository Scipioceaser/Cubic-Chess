using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generate : MonoBehaviour
{
    [Range(1, 50)]
    public int boardSize = 1;
    [Range(1, 50)]
    public int boardHeight = 1;
    [Range(0.1f, 10f)]
    public float nodeDropSpeed = 0.5f;
    [Range(1, 10)]
    public int nodeScale = 1;
    public bool dropWithAnimation = true;
    
    public Color colorEven = Color.white;
    public Color colorOdd = Color.black;

    [HideInInspector]
    public GameObject[] grid;

    // Have to add these in for the gizmos not do draw in a weird way after re-entering scene mode
    [SerializeField][HideInInspector]
    private int gizmoGridSize = 1;
    [SerializeField][HideInInspector]
    private int gizmoGridHeight = 1;

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(CreateNodes());
    }

    private IEnumerator CreateNodes()
    {
        WaitForSeconds wait = new WaitForSeconds(0.001f);
        Material even = new Material(Shader.Find("Standard"));
        Material odd = new Material(Shader.Find("Standard"));
        even.color = colorEven;
        odd.color = colorOdd;

        int l, w, h;

        if (boardSize % 2 == 0)
            boardSize++;

        l = boardSize + 2;
        w  = boardSize + 2;
        h = boardHeight + 2;
        
        if (grid.Length != 0)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                Vector3 pos = grid[i].transform.position;
                
                if (pos.y != 0 && pos.y != (h - 1) && pos.x != 0 && pos.x != (w - 1) && pos.z != 0 && pos.z != (l - 1))
                {
                    Node n;

                    n = grid[i].AddComponent<Node>();
                    n.Init(nodeScale, transform.position + new Vector3(pos.x, pos.y, pos.z));
                }
                else
                {
                    NodeMesh n;

                    n = grid[i].AddComponent<NodeMesh>();

                    if (i % 2 == 0)
                    {
                        n.SetColor(even);
                    }
                    else
                    {
                        n.SetColor(odd);
                    }
                    
                    n.Init(nodeScale, transform.position + new Vector3(pos.x, pos.y, pos.z));
                    
                    if(dropWithAnimation)
                        StartCoroutine(n.Move(transform.position + new Vector3(pos.x, pos.y, pos.z) +
                            new Vector3(0, (boardHeight + 25), 0), transform.position + new Vector3(pos.x, pos.y, pos.z), nodeDropSpeed));
                    
                    yield return wait;
                }
            }
        }
    }

    public void CreateMap()
    {
        int length, width, height;

        if (boardSize % 2 == 0)
            boardSize++;

        length = boardSize + 2;
        width = boardSize + 2;
        height = boardHeight + 2;

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
                    nodeObject.transform.position = new Vector3(x, y, z);
                    grid[i] = nodeObject;
                    i++;
                }
            }
        }
        
        gizmoGridHeight = boardHeight;
        gizmoGridSize = boardSize;

        if (Application.isPlaying)
            StopAllCoroutines();
            StartCoroutine(CreateNodes());
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
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;
        
        int s = gizmoGridSize + 2;
        int h = gizmoGridHeight + 2;

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

            Gizmos.DrawSphere(grid[i].transform.position, (float)nodeScale / 10f);
        }
    }
}
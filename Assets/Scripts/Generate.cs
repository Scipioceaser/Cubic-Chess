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

    public Vector3 WorldPosToNodePos(Vector3 position)
    {
        return (position + transform.position);
    }

    private IEnumerator CreateNodes()
    {
        WaitForSeconds wait = new WaitForSeconds(0.001f);
        Material even = new Material(Shader.Find("Standard"));
        Material odd = new Material(Shader.Find("Standard"));
        even.color = colorEven;
        odd.color = colorOdd;

        int l, w, h;
        
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
                    n.Init(nodeScale, n.gameObject.transform.position);
                }
                else
                {
                    NodeMesh n;

                    n = grid[i].AddComponent<NodeMesh>();

                    #region Color Assignment
                    if (pos.y == 0 || pos.y == (h - 1))
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

                        //if ((pos.x == 0 || pos.x == (w - 1)) && (pos.z != 0 || pos.z != (l - 1)))
                        //{
                        //    if (pos.z % 2 == 0 && pos.y % 2 != 0 || pos.z % 2 != 0 && pos.y % 2 == 0)
                        //    {
                        //        n.SetColor(even);
                        //    }
                        //    else
                        //    {
                        //        n.SetColor(odd);
                        //    }
                        //}
                        //else if ((pos.z == 0 || pos.z == (l - 1)) && (pos.x != 0 || pos.x != (l - 1)))
                        //{
                        //    if (pos.x % 2 == 0 && pos.y % 2 != 0 || pos.x % 2 != 0 && pos.y % 2 == 0)
                        //    {
                        //        n.SetColor(even);
                        //    }
                        //    else
                        //    {
                        //        n.SetColor(odd);
                        //    }
                        //}
                        else
                        {
                            n.SetColor(even);
                        }
                    }
                    #endregion

                    n.Init(nodeScale, n.gameObject.transform.position);
                    
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

        if (Application.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(CreateNodes());
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

            Gizmos.DrawSphere(grid[i].transform.position + transform.position, (float)nodeScale / 10f);
        }
    }
}
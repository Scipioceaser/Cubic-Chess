using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera controls")]
    public float mouseRotationSpeed = 2f;
    public float rotateSmoothness = 0.5f;
    public float maxCameraDistance = 10f;
    public float minCameraDistance = .5f;
    public float mouseYMinLimit = -360f;
    public float mouseYMaxLimit = 360f;
    
    private Map mapObject;
    private Camera playerCamera;
    private Vector3 mapCentre;
    private Vector3 playerCameraOffset;
    private float playerCameraDistance = 5f;

    private float mouseX;
    private float mouseY;

    private Node selectedNode;
    private Unit selectedUnit;
    
    private List<Node> nodesToColor = new List<Node>();

    [Header("Colors")]
    public Color validPositionColor = Color.green;
    public Color selecedNodeMeshColor = Color.red;
    public Color selectedOutlineColor = new Color(1.0f, 0.53f, 0.015f, 1.0f);
    
    // Start is called before the first frame update
    private void Awake()
    {
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        playerCamera = GetComponent<Camera>();
        
        Vector3 angles = transform.eulerAngles;
        mouseX = angles.x;
        mouseY = angles.y;

        SetCentrePosition();
    }
    
    public void SetCentrePosition()
    {
        Vector3 nPos = new Vector3((mapObject.nodeScale / 2f), (mapObject.nodeScale / 2f), (mapObject.nodeScale / 2f));
        Vector3 mPos = new Vector3((mapObject.boardSize + 1) / 2, (mapObject.boardHeight + 1) / 2, (mapObject.boardSize + 1) / 2);
        mapCentre = mPos + nPos;
        Globals.mapCenterPoint = mapCentre;
        playerCameraOffset = transform.position - mapCentre;
        OrbitCamera(1);
    }

    private void Update() 
    {
        if (Globals.meshNodesCreated == (Globals.mapSize * Globals.mapSize * Globals.mapHeight))
        {
            if (selectedUnit != null)
            {
                foreach (Vector3 vector in selectedUnit.GetValidMovePositions(selectedUnit.currentNode.position))
                {
                    nodesToColor.Add(mapObject.NodeFromNodeVector(vector));
                }
            }

            //TODO: Add more dynamic node coloring system.
            if (Input.GetMouseButton(0))
            {
                mapObject.ResetColors();

                selectedNode = GetNodeFromMouse();
                nodesToColor.Add(selectedNode);
                
                if (selectedNode != null)
                {
                    if (selectedNode.nodeUnit != null && selectedUnit != selectedNode.nodeUnit)
                    {
                        selectedUnit = selectedNode.nodeUnit;
                        print(selectedUnit.name);
                    }
                    else if (selectedNode.nodeUnit == null)
                    {
                        // Move unit code here. And the valid move positions should be calculated when the unit is selected.
                        // With a chech for if the position without the original unit the player then clicks on is a valid position.
                        if (selectedUnit != null)
                        {
                            if (selectedUnit.GetValidMovePositions(selectedUnit.currentNode.position).Contains(selectedNode.transform.position))
                            {
                                selectedUnit.SetPositions(selectedUnit.GetValidMovePositions(selectedUnit.currentNode.position));

                                if (selectedUnit.positions.Contains(selectedNode.position))
                                {
                                    selectedUnit.MoveAlongPath(selectedNode.position);
                                }

                                selectedUnit.SetOutlineWidthAndColor(Color.clear, 1f);
                                selectedNode = null;
                                selectedUnit = null;
                                mapObject.ResetColors();
                            }
                        }

                        if (selectedUnit != null)
                            selectedUnit.SetOutlineWidthAndColor(Color.clear, 1f);

                        selectedNode = null;
                        selectedUnit = null;
                        mapObject.ResetColors();
                    }

                    if (selectedNode && selectedUnit)
                    {
                        foreach (Node node in nodesToColor)
                        {
                            if (node == selectedNode)
                            {
                                selectedUnit.SetOutlineWidthAndColor(selectedOutlineColor, 1.035f);
                                ColorSelectedNodeMesh(node, selecedNodeMeshColor);
                            }
                            else
                            {
                                ColorSelectedNodeMesh(node, validPositionColor);
                            }
                        }
                    }
                }
                else
                {
                    selectedUnit.SetOutlineWidthAndColor(Color.clear, 1f);
                    mapObject.ResetColors();
                }
                
                nodesToColor.Clear();
            }
        }
    }
    
    private void LateUpdate()
    {
        if (Globals.meshNodesCreated == (Globals.mapSize * Globals.mapSize * Globals.mapHeight))
            OrbitCamera();
    }

    private void ColorSelectedNodeMesh(Node node, Color toColor)
    {
        List<Node> nodes = mapObject.GetNeighbours(node);
        NodeMesh n = null;

        if (nodes.Count == 0)
            return;

        // This gets the NodeMesh that's closest to the selected node.
        float d = Vector3.Distance(nodes[0].position, node.position);
        foreach (Node nodeObject in nodes)
        {
            if (nodeObject.GetType() == typeof(NodeMesh) && Vector3.Distance(node.position, nodeObject.position) < d)
            {
                d = Vector3.Distance(node.position, nodeObject.position);
                n = (NodeMesh)nodeObject;
            }
        }
        
        if (n != null)
            n.SetFaceColor(node, toColor);
    }

    //TODO: Make sure this actually works and it isn't an illusion. Also remove need for raycasting. 
    //It makes the math wonky.
    private Node GetNodeFromMouse()
    {
        Node node = null;
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseHit;
        
        // Raycasts to node trigger colliders
        if (Physics.Raycast(mouseRay, out mouseHit))
        {
            Vector3 p = mouseHit.point;

            int px, py, pz;

            px = Mathf.RoundToInt(Mathf.Abs(p.x));
            py = Mathf.RoundToInt(Mathf.Abs(p.y));
            pz = Mathf.RoundToInt(Mathf.Abs(p.z));

            if (px > (mapObject.boardSize + 1))
                px = (mapObject.boardSize + 1);

            if (py > (mapObject.boardHeight + 1))
                py = (mapObject.boardHeight + 1);

            if (pz > (mapObject.boardSize + 1))
                pz = (mapObject.boardSize + 1);

            p = new Vector3(px, py, pz);
            // Returns the node, not nodeMesh
            node = mapObject.NodeFromWorldPoints(p);
        }
        
        return node;
    }

    private void OrbitCamera(int i = 0)
    {
        if (Input.GetMouseButton(1) || i == 1)
        {
            mouseX += Input.GetAxis("Mouse X") * mouseRotationSpeed * playerCameraDistance * rotateSmoothness;
            mouseY -= Input.GetAxis("Mouse Y") * mouseRotationSpeed * rotateSmoothness * playerCameraDistance;

            mouseY = ClampAngle(mouseY, mouseYMinLimit, mouseYMaxLimit);

            Quaternion r = Quaternion.Euler(mouseY, mouseX, 0);

            playerCameraDistance = Mathf.Clamp(playerCameraDistance * 5, minCameraDistance, maxCameraDistance);

            Vector3 negplayerCameraDistance = new Vector3(0, 0, -playerCameraDistance);
            Vector3 position = r * negplayerCameraDistance + mapCentre;

            transform.position = position;
            transform.rotation = r;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}

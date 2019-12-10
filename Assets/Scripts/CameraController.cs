using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

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

    [SerializeField]
    private Node selectedNode;
    [SerializeField]
    private Unit selectedUnit;
    
    private List<Node> nodesToColor = new List<Node>();

    [Header("Colors")]
    public Color validPositionColor = Color.green;
    public Color selecedNodeMeshColor = Color.blue;
    public Color selectedOutlineColor = new Color(1.0f, 0.53f, 0.015f, 1.0f);
    public Color EnemyNodeMeshColor = Color.red;

    [Header("Sound")]
    public AudioClip mouseSound;
    private AudioSource source;

    [Header("Victory")]
    public ParticleSystem confetti;
    public TextMeshProUGUI text;
    private bool playedConfetti = false;
    
    // Start is called before the first frame update
    private void Awake()
    {
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        playerCamera = GetComponent<Camera>();
        source = GetComponent<AudioSource>();
        
        Vector3 angles = transform.eulerAngles;
        mouseX = angles.x;
        mouseY = angles.y;
    }

    private void Start()
    {
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
        if (GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_WIN))
            PlayVictoryConfetti();

        if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_WIN))
            text.SetText("Defeat...");

        if (mapObject.transform.childCount == ((Globals.mapSize + 2) * (Globals.mapSize + 2) * (Globals.mapHeight + 2)) && !PauseMenu.paused)
        {
            if (GameStateManager.stateManager.CheckState(GameStateManager.State.AI_TURN_THINK)
                || GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_MOVE))
            {
                mapObject.ResetColors();
                mapObject.ResetUnitOutlines();
            }

            if (selectedUnit != null)
            {
                nodesToColor.Clear();

                foreach (Vector3 vector in selectedUnit.GetValidMovePositions(selectedUnit.currentNode.position))
                {
                    nodesToColor.Add(mapObject.NodeFromNodeVector(vector));
                }
                
                selectedUnit.SetOutlineWidthAndColor(selectedOutlineColor, 1.035f);
                ColorSelectedNodeMesh(selectedUnit.currentNode, selecedNodeMeshColor);
            }

            if (Input.GetMouseButton(0) && GameStateManager.stateManager.CheckState(GameStateManager.State.PLAYER_TURN_THINK))
            {
                selectedNode = GetNodeFromMouse();
                
                if (selectedNode != null)
                {
                    if (selectedNode.nodeUnit != null && selectedNode.nodeUnit.unitTeam == mapObject.playerTeam)
                    {
                        mapObject.ResetUnitOutlines();
                        mapObject.ResetColors();

                        selectedUnit = selectedNode.nodeUnit;

                        if (!source.isPlaying)
                            source.PlayOneShot(mouseSound);
                    }
                    else if (selectedUnit != null && selectedNode.nodeUnit == null || selectedUnit != null && selectedNode.nodeUnit.unitTeam != mapObject.playerTeam)
                    {
                        List<Vector3> movePositions = selectedUnit.GetValidMovePositions(selectedUnit.currentNode.position);

                        if (movePositions.Contains(selectedNode.position))
                        {
                            selectedUnit.SetPositions(selectedUnit.GetValidMovePositions(selectedUnit.currentNode.position));
                            
                            if (selectedUnit.positions.Contains(selectedNode.position))
                            {
                                if (selectedNode.nodeUnit != null)
                                {
                                    if (selectedNode.nodeUnit.unitTeam != selectedUnit.unitTeam)
                                    {
                                        selectedNode.nodeUnit.Fight();
                                    }
                                }

                                selectedUnit.MoveAlongPath(selectedNode.position);
                                GameStateManager.stateManager.SetState(GameStateManager.State.PLAYER_TURN_MOVE, 0.0001f);

                                if (!source.isPlaying)
                                    source.PlayOneShot(mouseSound);
                            }

                            selectedUnit = null;
                            selectedNode = null;
                            mapObject.ResetUnitOutlines();
                            mapObject.ResetColors();
                        }
                    }
                }
                else
                {
                    selectedUnit = null;
                    selectedNode = null;
                    mapObject.ResetUnitOutlines();
                    mapObject.ResetColors();
                }
                
                if (selectedNode && selectedUnit)
                {
                    foreach (Node node in nodesToColor)
                    {
                        if (node.nodeUnit != null)
                        {
                            if (node.nodeUnit.unitTeam != mapObject.playerTeam)
                            {
                                ColorSelectedNodeMesh(node, EnemyNodeMeshColor);
                            }
                        }
                        else
                        {
                            List<Vector3> vPos = selectedUnit.GetValidMovePositions(selectedUnit.unAdjustedPosition);

                            if (vPos.Contains(node.position))
                                ColorSelectedNodeMesh(node, validPositionColor);
                        }
                    }
                }
            }
        }
    }
    
    private void LateUpdate()
    {
        if (mapObject.grid.Length == ((Globals.mapSize + 2) * (Globals.mapSize + 2) * (Globals.mapHeight + 2)))
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

    public void PlayVictoryConfetti()
    {
        if (playedConfetti)
            return;

        text.SetText("Victory!");
        confetti.Play();
        playedConfetti = true;
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

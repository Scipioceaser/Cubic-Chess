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

    public Material selectedNodeMaterial;
    
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
        playerCameraOffset = transform.position - mapCentre;
        OrbitCamera(1);
    }

    private void Update() 
    {
        //TODO: Add more dynamic node coloring system.
        if (Input.GetMouseButton(0))
        {
            selectedNode = GetNodeFromMouse();
            mapObject.ResetColors();
            ColorSelectedNodeMesh(selectedNode);
        }    
    }
    
    private void LateUpdate()
    {
        OrbitCamera();
    }

    private void ColorSelectedNodeMesh(Node node)
    {
        List<Node> nodes = mapObject.GetNeighbours(node);
        NodeMesh n = null;

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
            n.SetColor(selectedNodeMaterial);
    }

    //TODO: Make sure this actually works and it isn't an illusion. Also remove need for raycasting.
    private Node GetNodeFromMouse()
    {
        Node node = null;
        Ray mouseRay = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit mouseHit;
        
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

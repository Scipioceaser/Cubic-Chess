using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraDistance = 5f;

    private Node[,,] sceneNodes;
    private Map mapObject;
    private Camera camera;
    private Vector3 mapCentre;
    [SerializeField]
    private List<Vector3> cameraMoveSphere = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        camera = GetComponent<Camera>();
        transform.position = (mapCentre + new Vector3(2.5f, 10f, -cameraDistance));
    }
    
    public void SetCentrePosition()
    {
        Vector3 nPos = new Vector3((mapObject.nodeScale / 2f), (mapObject.nodeScale / 2f), (mapObject.nodeScale / 2f));
        Vector3 mPos = new Vector3((mapObject.boardSize + 1) / 2, (mapObject.boardHeight + 1) / 2, (mapObject.boardSize + 1) / 2);
        mapCentre = mPos + nPos;
    }
}

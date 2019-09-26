using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Node[,,] sceneNodes;
    private Map mapObject;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

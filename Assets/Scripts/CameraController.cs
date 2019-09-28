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
    private Camera camera;
    private Vector3 mapCentre;
    private Vector3 cameraOffset;
    private float cameraDistance = 5f;

    private float mouseX;
    private float mouseY;
    
    // Start is called before the first frame update
    private void Awake()
    {
        mapObject = GameObject.FindGameObjectWithTag("Map").GetComponent<Map>();
        camera = GetComponent<Camera>();
        
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
        cameraOffset = transform.position - mapCentre;
        OrbitCamera(1);
    }

    private void Update() 
    {

    }

    private void LateUpdate()
    {
        OrbitCamera();
    }
    
    private void OrbitCamera(int i = 0)
    {
        if (Input.GetMouseButton(1) || i == 1)
        {
            mouseX += Input.GetAxis("Mouse X") * mouseRotationSpeed * cameraDistance * rotateSmoothness;
            mouseY -= Input.GetAxis("Mouse Y") * mouseRotationSpeed * rotateSmoothness * cameraDistance;

            mouseY = ClampAngle(mouseY, mouseYMinLimit, mouseYMaxLimit);

            Quaternion r = Quaternion.Euler(mouseY, mouseX, 0);

            cameraDistance = Mathf.Clamp(cameraDistance * 5, minCameraDistance, maxCameraDistance);

            Vector3 negcameraDistance = new Vector3(0, 0, -cameraDistance);
            Vector3 position = r * negcameraDistance + mapCentre;

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

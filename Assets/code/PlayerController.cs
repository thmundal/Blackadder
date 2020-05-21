using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    private new Camera camera;

    [SerializeField]
    private float maxCameraDistance = 10;
    [SerializeField]
    private float minCameraDistance = -2;
    [SerializeField]
    private Vector3 cameraPivotPoint = new Vector3(1, 2, -2);
    [SerializeField]
    private Vector3 cameraPosition;
    [SerializeField, Range(0, 10)]
    private float mouseSensitivity = 0.01f;
    [SerializeField]
    private float controlledCameraDistance;

    private float cameraXRotation;
    private float cameraYRotation;

    private float cameraDistance => -(this.maxCameraDistance + Mathf.Max(this.minCameraDistance, Mathf.Min(this.controlledCameraDistance, this.maxCameraDistance)));

    // Start is called before the first frame update
    void Start()
    {
        this.AttachMainCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.MoveForward();
        }

        if(Input.GetKey(KeyCode.S))
        {
            this.MoveBackwards();
        }

        if(Input.GetKey(KeyCode.A))
        {
            this.MoveLeft();
        }

        if(Input.GetKey(KeyCode.D))
        {
            this.MoveRight();
        }

        // Forward scroll
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            this.controlledCameraDistance = Mathf.Max(this.minCameraDistance, this.controlledCameraDistance - 1);
        }

        // Backwards scroll
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            this.controlledCameraDistance = Mathf.Min(this.maxCameraDistance, this.controlledCameraDistance + 1);
        }

        //this.camera.transform.localPosition = new Vector3(this.cameraPosition.x, this.cameraPosition.y, this.cameraDistance);
        this.SetCameraRotaionalPosition(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        this.camera.transform.LookAt(this.transform.position + this.cameraPivotPoint);
    }

    void SetCameraRotaionalPosition(float xDiff, float yDiff, float zDiff = 0)
    {
       
        this.cameraXRotation += xDiff * this.mouseSensitivity;
        this.cameraYRotation += yDiff * this.mouseSensitivity;

        float x = this.cameraDistance * Mathf.Cos(this.cameraXRotation) * Mathf.Sin(this.cameraYRotation);
        float y = this.cameraDistance * Mathf.Sin(this.cameraXRotation) * Mathf.Sin(this.cameraYRotation);
        float z = this.cameraDistance * Mathf.Cos(this.cameraYRotation);
        
        Vector3 position = new Vector3(x, y, z);
        this.camera.transform.localPosition = position;
        //this.cameraPosition = position;

        //Vector3 direction = Quaternion.Euler(this.cameraXRotation, this.cameraYRotation, 0) * (this.cameraPosition - this.cameraPivotPoint) + this.cameraPivotPoint;
        //this.cameraPosition = direction;
        //this.camera.transform.localPosition = Quaternion.Euler(xDiff, yDiff, 0) * this.cameraPivotPoint;
        //this.camera.transform.RotateAround(this.cameraPivotPoint, Vector3.up, cameraXRotation * this.mouseSensitivity);
        //this.camera.transform.RotateAround(this.cameraPivotPoint, Vector3.right, cameraYRotation * -this.mouseSensitivity);
    }

    void AttachMainCamera()
    {
        this.camera = Camera.main;

        if(!this.camera)
        {
            this.camera = (new GameObject("Player Camera", typeof(Camera))).GetComponent<Camera>();
        }

        this.cameraPosition = this.cameraPivotPoint + new Vector3(0, 0, this.cameraDistance);

        this.camera.transform.parent = this.transform;
        this.camera.transform.position = this.cameraPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

﻿using System.Collections;
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
    private Vector3 cameraPosition;
    [SerializeField, Range(0, 10)]
    private float mouseSensitivity = 0.01f;
    [SerializeField]
    private float controlledCameraDistance;
    [SerializeField, Range(1, 100)]
    private float cameraSmoothingMultiplier;

    private float cameraXRotation;
    private float cameraYRotation;

    private float cameraDistance => -(this.maxCameraDistance + Mathf.Max(this.minCameraDistance, Mathf.Min(this.controlledCameraDistance, this.maxCameraDistance)));

    [SerializeField]
    private bool playerCameraFree = false;

    // Start is called before the first frame update
    void Start()
    {
        this.AttachMainCamera();
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();
        this.Sprint(Input.GetKey(KeyCode.LeftShift));

        this.playerCameraFree = !Input.GetKey(KeyCode.Mouse1);

        if (playerCameraFree)
        {
            int x = 0;
            int y = 0;
            bool moveForward = false;

            if (Input.GetKey(KeyCode.W))
            {
                x += 1;
                moveForward = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                x -= 1;
                moveForward = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                y += 1;
                moveForward = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                y -= 1;
                moveForward = true;
            }
            float deltaAngleDeg = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            this.characterRotation *= Quaternion.AngleAxis(deltaAngleDeg, this.rigidBody.transform.up);
            
            if(this.grounded) 
            {
                this.MoveForward(moveForward);
                this.MoveBackwards(false);
                this.MoveLeft(false);
                this.MoveRight(false);
            }
        }
        else
        {
            
            if(this.grounded) 
            {
                this.MoveForward(Input.GetKey(KeyCode.W));
                this.MoveBackwards(Input.GetKey(KeyCode.S));
                this.MoveLeft(Input.GetKey(KeyCode.A));
                this.MoveRight(Input.GetKey(KeyCode.D));
            }
        }
        
        if(this.movementState != MovementState.Still || !this.playerCameraFree)
        {
            this.RotateToCharacterRotation();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            this.Jump();
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

        this.SetCameraPosition(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    void SetCameraPosition(float xDiff, float yDiff)
    {
        this.cameraXRotation += xDiff * this.mouseSensitivity * Time.deltaTime;
        this.cameraYRotation += yDiff * this.mouseSensitivity * Time.deltaTime;

        float y = -this.cameraDistance * Mathf.Cos(this.cameraYRotation);
        float x = -this.cameraDistance * Mathf.Cos(this.cameraXRotation) * Mathf.Sin(this.cameraYRotation);
        float z = this.cameraDistance * Mathf.Sin(this.cameraXRotation) * Mathf.Sin(this.cameraYRotation);

        this.cameraPosition = new Vector3(x, y, z);
        this.camera.transform.position = this.transform.position + this.cameraPosition;
        this.camera.transform.LookAt(this.transform.position);

        var v = this.camera.transform.eulerAngles;
        var t = this.rigidBody.transform.eulerAngles;

        this.characterRotation = Quaternion.Euler(t.x, v.y, t.z);
    }

    void AttachMainCamera()
    {
        this.camera = Camera.main;

        if(!this.camera)
        {
            this.camera = (new GameObject("Player Camera", typeof(Camera))).GetComponent<Camera>();
        }

        this.cameraPosition = new Vector3(0, 0, this.cameraDistance);

        this.camera.transform.position = this.cameraPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

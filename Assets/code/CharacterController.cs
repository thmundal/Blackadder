using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    protected float characterRotationRate;

    protected Quaternion characterRotation;
    protected MovementState movementState;
    // Base movement speed of the character being controlled
    [SerializeField]
    protected float baseMovementSpeed;

    // Base jump strength
    [SerializeField]
    protected float baseJumpStrength;

    [SerializeField, ReadOnly]
    protected bool grounded;

    private Animator anim;
    protected Animator animator
    {
        get
        {
            if(!this.anim)
            {
                this.anim = this.GetComponentInChildren<Animator>();
            }
            return this.anim;
        }
    }

    private Rigidbody rb;
    protected Rigidbody rigidBody
    {
        get
        {
            if(!this.rb)
            {
                this.rb = this.gameObject.GetComponent<Rigidbody>();
            }
            return this.rb;
        }
    }

    private CapsuleCollider coll;
    protected CapsuleCollider collider
    {
        get
        {
            if(!this.coll)
            {
                this.coll = this.GetComponentInChildren<CapsuleCollider>();
            }
            return this.coll;
        }
    }

    // TODO: This should be calculated from a stats system
    private float movementSpeed => this.baseMovementSpeed;
    public void Start()
    {
        this.characterRotation = this.transform.rotation;
    }
    public void Update()
    {
        if(this.grounded)
        {
            this.animator.SetBool("jumping", false);
        } 
        else
        {
            this.animator.SetBool("jumping", true);
        }
        this.animator.SetBool("forward", (this.movementState & MovementState.Forward) != 0);
        this.animator.SetBool("strafe_left", (this.movementState & MovementState.Left) != 0);
        this.animator.SetBool("strafe_right", (this.movementState & MovementState.Right) != 0);
    }
    private void FixedUpdate()
    {
        if(this.collider)
        {
            Vector3 center = this.collider.bounds.center;
            float length = (this.collider.height / 2) + 0.05f;
            Debug.DrawRay(this.collider.bounds.center, this.collider.transform.TransformDirection(Vector3.down) * length, Color.red);
            this.grounded = Physics.Raycast(this.collider.bounds.center, this.collider.transform.TransformDirection(Vector3.down), length);
        }
    }

    protected void Move(Vector3 direction, float speed)
    {
        //this.rigidBody.MovePosition(this.rigidBody.transform.position + (direction * speed * Time.deltaTime));
        this.transform.position += direction * speed * Time.deltaTime;
    }

    protected void Jump()
    {
        if(this.grounded)
        {
            this.rigidBody.AddForce(Vector3.up * this.baseJumpStrength);
        }
    }
    
    protected void MoveForward(bool active)
    {
        if(active)
        {
            this.RotateToCharacterRotation();
            this.Move(this.transform.forward, this.movementSpeed);
            this.movementState |= MovementState.Forward;
        }
        else
        {
            this.movementState &= ~MovementState.Forward;
        }
    }

    protected void MoveBackwards(bool active)
    {
        if(active)
        {
            this.RotateToCharacterRotation();
            this.Move(-this.transform.forward, this.movementSpeed);
            this.movementState |= MovementState.Backward;
        }
        else
        {
            this.movementState &= ~MovementState.Backward;
        }
    }

    protected void MoveRight(bool active)
    {
        if(active) 
        {
            this.RotateToCharacterRotation();
            this.Move(this.transform.right, this.movementSpeed);
            this.movementState |= MovementState.Right;
        }
        else 
        {
            this.movementState &= ~MovementState.Right;
        }
    }

    protected void MoveLeft(bool active)
    {
        if(active) 
        {
            this.RotateToCharacterRotation();
            this.Move(-this.transform.right, this.movementSpeed);
            this.movementState |= MovementState.Left;
        }
        else
        {
            this.movementState &= ~MovementState.Left;
        }
    }

    protected void RotateToCharacterRotation()
    {
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.characterRotation, this.characterRotationRate * Time.deltaTime);
        Debug.Log(this.transform.rotation);
    }
}

public enum MovementState
{
    Still       = 0b0000,
    Forward     = 0b0001,
    Backward    = 0b0010,
    Right       = 0b0100,
    Left        = 0b1000
}
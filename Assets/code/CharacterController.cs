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

    [SerializeField]
    private bool canJump;

    private readonly Vector3 groundColliderOffset = new Vector3(0, 0.0f, 0);
    private readonly Vector3 capsuleColliderOffset = new Vector3(0, 0.9f, 0);
    private Animator _animator;
    private GroundCollider _groundCollider;
    protected GroundCollider groundCollider {
        get {
            if(!this._groundCollider)
            {
                this._groundCollider = this.GetComponentInChildren<GroundCollider>();
            }
            return this._groundCollider;
        }
    }
    protected Animator animator
    {
        get
        {
            if(!this._animator)
            {
                this._animator = this.GetComponentInChildren<Animator>();
            }
            return this._animator;
        }
    }

    private Rigidbody _rigidBody;
    protected Rigidbody rigidBody
    {
        get
        {
            if(!this._rigidBody)
            {
                this._rigidBody = this.gameObject.GetComponent<Rigidbody>();
            }
            return this._rigidBody;
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
            this.animator.SetBool("falling", false);
            this.animator.SetBool("grounded", true);
            if(!this.canJump)
            {
                this.Invoke("SetCanJump", 0.25f);
            }
        }
        else if(this.rigidBody.velocity.y < 0)
        {
            this.animator.SetBool("grounded", false);
            this.animator.SetBool("falling", true);
            this.canJump = false;
        }
        else if(this.canJump)
        {
            this.animator.SetBool("jumping", true);
            this.canJump = false;
        }

        bool forward = (this.movementState & MovementState.Forward) != 0;
        bool backward = (this.movementState & MovementState.Backward) != 0;
        bool left = (this.movementState & MovementState.Left) != 0;
        bool right = (this.movementState & MovementState.Right) != 0;

        int horizontalDirection = (forward || backward ? (forward ? 1 : -1) : 0);
        int verticalDirection = (right || left ? (right ? 1 : -1) : 0);

        bool horVert = (forward || backward) && (left || right);

        float horizontalValue = 1.0f;
        float verticalValue = 1.0f;

        this.animator.SetFloat("vertical", verticalValue * horizontalDirection);
        this.animator.SetFloat("horizontal", horizontalValue * verticalDirection);
    }

    private void SetCanJump()
    {
        this.canJump = true;
    }

    private void FixedUpdate()
    {
        if(this.collider)
        {
            if(!this.grounded && this.rigidBody.velocity.y > 0)
            {
                //this.groundCollider.position = new Vector3(0, 1.5f, 0);
                this.collider.center = new Vector3(0, 1.5f, 0);
            }
            else
            {
                //this.groundCollider.position = this.groundColliderOffset;
                this.collider.center = this.capsuleColliderOffset;
            }

            this.grounded = this.groundCollider.grounded;
            //Vector3 center = this.collider.bounds.center;
            //float length = (this.collider.height / 2) + 0.07f;
            //Debug.DrawRay(this.collider.bounds.center, this.collider.transform.TransformDirection(Vector3.down) * length, Color.red);
            //this.grounded = Physics.Raycast(this.collider.bounds.center, this.collider.transform.TransformDirection(Vector3.down), length);
        }
    }

    protected void Move(Vector3 direction, float speed)
    {
        //this.rigidBody.MovePosition(this.rigidBody.transform.position + (direction * speed * Time.deltaTime));
        this.transform.position += direction * speed * Time.deltaTime;
    }

    protected void Jump()
    {
        if(this.grounded && this.canJump)
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
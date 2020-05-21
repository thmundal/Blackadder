using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Base movement speed of the character being controlled
    [SerializeField]
    protected float baseMovementSpeed;

    // Base jump strength
    [SerializeField]
    protected float baseJumpStrength;

    // TODO: This should be calculated from a stats system
    private float movementSpeed => this.baseMovementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    protected void Move(Vector3 direction, float speed)
    {
        this.transform.position += direction * speed * Time.deltaTime;
    }

    protected void Jump()
    {

    }
    
    protected void MoveForward()
    {
        this.Move(this.transform.forward, this.movementSpeed);
    }

    protected void MoveBackwards()
    {
        this.Move(-this.transform.forward, this.movementSpeed);
    }

    protected void MoveRight()
    {
        this.Move(this.transform.right, this.movementSpeed);
    }

    protected void MoveLeft()
    {
        this.Move(-this.transform.right, this.movementSpeed);
    }
}

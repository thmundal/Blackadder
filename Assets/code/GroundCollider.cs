using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    private BoxCollider boxCollider;
    public bool grounded;

    public Vector3 position
    {
        set
        {
            this.transform.localPosition = value;
        }
        get
        {
            return this.transform.localPosition;
        }
    }
    private void Start()
    {
        this.boxCollider = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        this.grounded = true;
        Vector3 bottomOfCollider = this.boxCollider.center * 0.5f - this.transform.position;
        Vector3 otherColliderTop = other.bounds.center * 0.5f + other.gameObject.transform.position;

    }

    private void OnTriggerExit(Collider other)
    {
        this.grounded = false;
    }

    private void OnTriggerStay(Collider other)
    {
        this.grounded = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{


    private Vector3 mousePosition;
    public float moveSpeed = 0.1f;
    public Rigidbody2D myRigidBody;

    void Start ()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        //transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
        myRigidBody.MovePosition(Vector2.Lerp(transform.position, mousePosition, moveSpeed));
    }
}

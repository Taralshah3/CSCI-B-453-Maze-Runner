using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    float playerSpeed = (float)0.25;
    float moveSpeed = 5f;
    Vector3 movement;
    public bool isGameOver = false;

    public Rigidbody rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY;

    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = transform.position.y;
        movement.z = Input.GetAxisRaw("Vertical");

    }

    void FixedUpdate() {
        float moveSpeed = 5f;
        if(!isGameOver)  {
            rb.MovePosition(rb.position + movement*moveSpeed  * Time.fixedDeltaTime);
        }
        

    }
}

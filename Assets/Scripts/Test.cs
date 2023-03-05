using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 moveDirection;
    private bool isFacingLeft = true;
    private Vector2 facingLeft;
    private float horizontalInput;

    private void Start()
    {
        facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        if (Input.GetAxis("Horizontal") != 0)
        {
            horizontalInput = Input.GetAxis("Horizontal");
        }
    }

    void FixedUpdate()
    {
        Move();
        if (horizontalInput > 0 && isFacingLeft)
        {
            isFacingLeft = false;
            Flip();
        }
        if (horizontalInput < 0 && !isFacingLeft)
        {
            isFacingLeft = true;
            Flip();
        }
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed * Time.deltaTime, moveDirection.y * moveSpeed * Time.deltaTime);
    }

    void Flip()
    {
        if (!isFacingLeft)
        {
            transform.localScale = facingLeft;
        }
        if (isFacingLeft)
        {
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
        }
    }
}

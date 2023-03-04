using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float Speed;
    public float Impulse;
    
    private Vector2 Direction = Vector2.right;
    private Rigidbody2D Rigid2D;
    private bool bounce = false;
    private float Gravity;


    // Start is called before the first frame update
    void Start()
    {
        Rigid2D = GetComponent<Rigidbody2D>();
        Gravity = Rigid2D.gravityScale;
        Rigid2D.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (bounce == true)
            transform.Translate(Direction * Speed * Time.deltaTime);
        else
        {
            Rigid2D.velocity = (Direction + Vector2.down) * Speed;
        }
    }

    public void SetDirection(Vector2 Dir)
    {
        Direction = Dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            //kill enemy
        }
        Destroy(this.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(bounce == false)
        {
            bounce = true;
            Rigid2D.gravityScale = Gravity;
        }
        if (Rigid2D.velocity.y <= 0)
        {
            Rigid2D.AddForce(Vector2.up * Impulse, ForceMode2D.Impulse);
        }
    }
}

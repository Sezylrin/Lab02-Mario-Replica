using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUps : int
    {
        OneUp = 1,
        Mushroom = 2,
        FireFlower = 3,
        Star = 4,
    }
    public PowerUps Type;
    private Rigidbody2D Rigid2D;
    private Vector2 Direction = Vector2.right;
    public float Impulse;
    public float Speed;
    private bool Spawning = true;
    private float Target;
    public BoxCollider2D Box;
    public CircleCollider2D Circle;
    private float gravity;
    // Start is called before the first frame update
    void Start()
    {
        Rigid2D = GetComponent<Rigidbody2D>();
        if (Rigid2D != null)
        {
            gravity = Rigid2D.gravityScale;
            Rigid2D.gravityScale = 0;
        }
        Target = transform.position.y + 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Spawning == true)
        {
            if (Target - transform.position.y > 0.05)
                transform.Translate(Vector2.up * 2 * Time.deltaTime);
            else
            {
                transform.position = new Vector3(transform.position.x, Target, 0);
                Spawning = false;
                if (Type == PowerUps.FireFlower)
                    return;
                Box.enabled = true;
                Circle.enabled = true;
                Rigid2D.gravityScale = gravity;
            }
        }
        if (Spawning == false)
            transform.Translate(Direction * Speed * Time.deltaTime);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Type != PowerUps.Star)
            return;
        if (Rigid2D.velocity.y <= 0)
        {
            Rigid2D.AddForce(Vector2.up * Impulse, ForceMode2D.Impulse);
        }
    }

    public void SetDirection(Vector2 Dir)
    {
        Direction = Dir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Direction *= -1;
    }

}

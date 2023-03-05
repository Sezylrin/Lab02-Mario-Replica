using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        Goomba,
        Koopa
    };

    public EnemyType type;
    public float speed;
    private Transform enemyTransform;
    private Animator enemyAnimator;
    private BoxCollider2D enemyCollider;
    private Rigidbody2D enemyRigidbody;
    private AudioSource enemyAudioSource;
    private SpriteRenderer enemySpriteRenderer;
    
    void Awake()
    {
        enemyTransform = this.gameObject.transform;
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<BoxCollider2D>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyAudioSource = GetComponent<AudioSource>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = -3;
    }

    // Update is called once per frame
    void Update()
    {
        enemyTransform.position = enemyTransform.position + new Vector3(speed * Time.deltaTime, 0, 0); //Moves the enemy
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 7 || col.gameObject.tag == "Enemy" || col.gameObject.tag == "Shell")
        {
            speed *= -1; //Changes enemy direction if it hits a wall
        }
        if (col.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "Enemy")
            {
                print("Player Took Damage");
                //Hurt player
            }
            else if (this.gameObject.tag == "Shell")
            {
                this.gameObject.tag = "MovingShell";
                CancelInvoke();
                enemyAnimator.SetTrigger("Death");
                foreach (ContactPoint2D hit in col.contacts) // Finds the impact point then determines direction shell should move
                {
                    if (hit.normal.x > 0)
                    {
                        speed = 6;
                    }
                    else speed = -6;
                }
            }
        }
        if (col.gameObject.tag == "MovingShell")
        {
            death();
            foreach (ContactPoint2D hit in col.contacts) // Finds the impact point then determines direction shell should move
            {
                if (hit.normal.x > 0)
                {
                    enemyRigidbody.AddForce(new Vector2(100, 100));
                }
                else
                {
                    enemyRigidbody.AddForce(new Vector2(-100, 100));
                }
            }
        }
    }

    public void stomp()
    {
        enemyAnimator.SetTrigger("Death");
        speed = 0;
        enemyAudioSource.Play();

        //Grant player score

        if (type == EnemyType.Goomba)
        {
            print("Goomba Died");
            enemyCollider.enabled = false;
            enemyRigidbody.isKinematic = true;
            enemyTransform.position = enemyTransform.position + new Vector3(0, -0.31f, 0);
            Invoke("despawn", 1f);
        }
        else if (type == EnemyType.Koopa)
        {
            print("Koopa Died");
            this.gameObject.tag = "Shell";
            enemyCollider.size = new Vector2(1, 0.9f);
            enemyTransform.position = enemyTransform.position + new Vector3(0, -0.3f, 0);
            Invoke("regrow", 7f);
        }
    }

    void despawn()
    {
        Destroy(this.gameObject);
    }

    void regrow()
    {
        Invoke("respawn", 3f);
        enemyAnimator.SetTrigger("Regrow");
    }

    void respawn()
    {
        enemyAnimator.SetTrigger("Respawn");
        this.gameObject.tag = "Enemy";
        enemyCollider.size = new Vector2(1, 1.5f);
        enemyTransform.position = enemyTransform.position + new Vector3(0, 0.345f, 0);
        speed = -3;
    }

    void death()
    {
        if (type == EnemyType.Koopa)
        {
            enemyAnimator.SetTrigger("Death");
        }
        enemySpriteRenderer.flipY = false;
        enemyCollider.enabled = false;
    }
}

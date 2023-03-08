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
    public PlayerManager playerScript;
    public bool dead;
    public bool ToActivate = true;
    private GameObject Mario;
    void Awake()
    {
        
        enemyTransform = this.gameObject.transform;
        enemyAnimator = GetComponent<Animator>();
        enemyCollider = GetComponent<BoxCollider2D>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyAudioSource = GetComponent<AudioSource>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        playerScript = GameObject.Find("Mario").GetComponent<PlayerManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Mario = GameObject.Find("Mario");
        speed = 0;
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {

        Debug.Log(Mathf.Abs(Mario.transform.position.x - transform.position.x));
        if (Mathf.Abs(Mario.transform.position.x - transform.position.x) < 11 && ToActivate == true)
        {
            speed = -2;
            ToActivate = false;
        }
        enemyTransform.position = enemyTransform.position + new Vector3(speed * Time.deltaTime, 0, 0); //Moves the enemy
        if (enemyTransform.position.y < -20)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 7 || (col.gameObject.tag == "Enemy" && this.gameObject.tag == "Enemy") || col.gameObject.tag == "Shell")
        {
            speed *= -1; //Changes enemy direction if it hits a wall
        }
        if (col.gameObject.tag == "Player")
        {
            if (this.gameObject.tag == "Enemy" || this.gameObject.tag == "MovingShell")
            {
                if (playerScript.Invincible == true)
                {
                    death((Vector2)col.gameObject.transform.position);
                }
                else
                {
                    playerScript.TakeDamage();
                    print("Player Took Damage");
                }
            }
            else if (this.gameObject.tag == "Shell")
            {
                this.gameObject.tag = "MovingShell";
                CancelInvoke();
                enemyAnimator.SetTrigger("Death");
                float x = col.gameObject.transform.position.x - enemyTransform.position.x;
                if (x > 0)
                {
                    speed = -6;
                }
                else speed = 6;
            }
        }
        if (col.gameObject.tag == "MovingShell" || col.gameObject.tag == "Fireball")
        {
            death((Vector2)col.gameObject.transform.position);

        }
        if (col.gameObject.tag == "Barrier")
        {
            Destroy(this.gameObject);
        }
    }

    public void stomp()
    {
        dead = true;
        enemyAnimator.SetTrigger("Death");
        speed = 0;
        enemyAudioSource.Play();
        bouncePlayer();
        //Grant player score

        if (type == EnemyType.Goomba)
        {
            print("Goomba Died");
            this.gameObject.tag = "Untagged";
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
        dead = false;
        enemyAnimator.SetTrigger("Respawn");
        this.gameObject.tag = "Enemy";
        enemyCollider.size = new Vector2(1, 1.5f);
        enemyTransform.position = enemyTransform.position + new Vector3(0, 0.345f, 0);
        speed = -3;
    }

    public void death(Vector2 position)
    {
        dead = true;
        float x = position.x - enemyTransform.position.x;
        if (type == EnemyType.Koopa)
        {
            enemyAnimator.SetTrigger("Death");
        }
        enemySpriteRenderer.flipY = false;
        enemyCollider.enabled = false;
        enemyAudioSource.Play();

        if (x > 0)
        {
            enemyRigidbody.AddForce(new Vector2(100, 100));
        }
        else
        {
            enemyRigidbody.AddForce(new Vector2(-100, 100));
        }
    }

    public void bouncePlayer()
    {
        playerScript.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(20, 700));
    }
}

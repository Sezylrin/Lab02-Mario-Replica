using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int PlayerState = 1;
    public float InvincibleTimer = 8;
    public float ShotDelay = 0.25f;
    public float BurstDelay = 0.5f;
    public Sprite[] Sprites;
    public GameObject MarioSprite;
    public GameObject FireProjectile;
    public float offset;
    public bool Invincible = false;

    private MarioAnim MarioAnimation;
    private Vector2 Direction = Vector2.left;
    private Animator MarioAnimator;
    private BoxCollider2D Collider;
    private SpriteRenderer SpriteRender;
    public float Timer;
    public float FireTimer = 0;
    public int BurstCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        SpriteRender = MarioSprite.GetComponent<SpriteRenderer>();
        Collider = GetComponent<BoxCollider2D>();
        MarioAnimator = GetComponentInChildren<Animator>();
        MarioAnimation = GetComponent<MarioAnim>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FireTimer > BurstDelay * -1)
            FireTimer -= Time.deltaTime;
        else
            BurstCount = 1;
        if (Invincible == true)
        {
            Timer -= Time.deltaTime;
            if (Timer < 0)
            {
                Invincible = false;
            }
        }
        if (PlayerState == 3)
        {
            if (Input.GetKeyDown(KeyCode.X) && FireTimer <= 0)
            {
                GameObject Fireball = Instantiate(FireProjectile, transform.position + (new Vector3(offset * Direction.x, 0.5f, 0)), Quaternion.identity);
                Fireball.GetComponent<FireBall>().SetDirection(Direction);
                if (BurstCount == 1)
                {
                    FireTimer = ShotDelay;
                    BurstCount++;
                }
                else
                {
                    FireTimer = BurstDelay;
                    BurstCount = 1;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PowerUp")
        {
            PowerUp Collected = collision.GetComponent<PowerUp>();
            if (Collected.Type == PowerUp.PowerUps.Star)
            {
                Timer = InvincibleTimer;
                Invincible = true;
            }
            else
            {
                if ((int)Collected.Type > PlayerState)
                {
                    Time.timeScale = 0;
                    MarioAnimation.Play = false;
                    PlayerState++;
                    MarioPowerUp();
                    if (PlayerState == 2)
                    {
                        MarioSprite.transform.Translate(Vector3.up * 0.5f, Space.Self);
                    }
                }
            }
            if (Collected.Type == PowerUp.PowerUps.OneUp)
            {
                GameManager.Instance.AddLives();
            }
            Collider.size = new Vector2(1, (PlayerState > 1 ? 2 : 1));
            Collider.offset = new Vector2(0, (PlayerState > 1 ? 0.5f : 0));
            Destroy(collision.gameObject);
        }
    }

    private void MarioPowerUp()
    {
        if (PlayerState == 2)
        {
            Debug.Log("ran");
            MarioAnimator.Play("Grow");
        }
        if(PlayerState == 3)
        {
            MarioAnimator.Play("Grow-Fire");
        }
    }

    public void TakeDamage()
    {
        //TODO: Implement functionality
        PlayerState = 1;
        MarioSprite.transform.Translate(Vector3.up * 0.5f * -1, Space.Self);
        //PlayerState = 0 means player has died
        if (PlayerState == 0)
        {
            GameManager.Instance.HandleMarioDeath();
        }
    }
}

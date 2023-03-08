using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] AudioClip powerupClip;
    [SerializeField] AudioClip oneUpClip;

    public AudioSource AudioSource { get; private set; }
    public AudioClip marioDieClip;
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
    private Vector2 Direction = Vector2.right;
    private Animator MarioAnimator;
    private BoxCollider2D Collider;
    private PlayerMovement MarioMove;
    private bool HandledDeath;
    public float Timer;
    public float FireTimer = 0;
    public int BurstCount = 1;
    // Start is called before the first frame update
    void Start()
    {
        MarioMove = GetComponent<PlayerMovement>();
        Collider = GetComponent<BoxCollider2D>();
        MarioAnimator = GetComponentInChildren<Animator>();
        MarioAnimation = GetComponent<MarioAnim>();
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Direction = MarioMove.IsFacingRight ? Vector2.right : Vector2.left;
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
        if (PlayerState == 0 && !AudioSource.isPlaying)
        {
            if (HandledDeath) return;
            GameManager.Instance.HandleMarioDeath();
            HandledDeath = true;
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
                AudioSource.clip = powerupClip;
            }
            else
            {
                if ((int)Collected.Type > PlayerState)
                {
                    AudioSource.clip = powerupClip;
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
            AudioSource.clip = oneUpClip;
            AudioSource.Play();
            Destroy(collision.gameObject);
        }
    }

    private void MarioPowerUp()
    {
        MarioAnimator.speed = 1;
        if (PlayerState == 2)
        {
            MarioAnimator.Play("Grow");
        }
        if(PlayerState == 3)
        {

            Debug.Log("ran");
            MarioAnimator.Play("Grow-Fire");
        }
    }

    public void TakeDamage()
    {
        //TODO: Implement functionality
        PlayerState = PlayerState > 1 ? 1 : 0;
        MarioSprite.transform.Translate(Vector3.up * 0.5f * -1, Space.Self);
        //PlayerState = 0 means player has died
        if (PlayerState == 0)
        {
            FindObjectOfType<LevelManager>().GetComponent<AudioSource>().Pause();
            AudioSource.clip = marioDieClip;
            AudioSource.Play();
            MarioAnimation.Play = false;
            MarioAnimator.Play("Death");
            Rigidbody2D Rigid = GetComponent<Rigidbody2D>();
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<BoxCollider2D>().excludeLayers = ~0;
            GetComponent<CapsuleCollider2D>().excludeLayers = ~0;
            Rigid.velocity = Vector2.zero;
            Rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        }
    }
}
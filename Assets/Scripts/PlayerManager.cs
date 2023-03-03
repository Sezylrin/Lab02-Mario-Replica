using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int PlayerState = 1;
    public float InvincibleTimer = 8;
    public Sprite[] Sprites;
    public GameObject MarioSprite;

    private Animator MarioAnimator;
    private BoxCollider2D Collider;
    private SpriteRenderer SpriteRender;
    private bool Invincible = false;
    private float Timer;
    // Start is called before the first frame update
    void Start()
    {        
        SpriteRender = MarioSprite.GetComponent<SpriteRenderer>();
        Collider = GetComponent<BoxCollider2D>();
        MarioAnimator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Invincible == true)
        {
            Timer -= Time.deltaTime;
            if (Timer < 0)
            {
                Invincible = false;
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
                    PlayerState++;
                    ChangeSprite();
                }
            }
            if (Collected.Type == PowerUp.PowerUps.OneUp)
            {
                //game manager icriment lives
            }
            MarioAnimator.Play("Grow", 0, 0);
            MarioSprite.transform.Translate(Vector3.up * (PlayerState > 1 ? 0.5f : 0), Space.Self);
            Collider.size = new Vector2(1, (PlayerState > 1 ? 2 : 1));
            Collider.offset = new Vector2(0, (PlayerState > 1 ? 0.5f : 0));
            Destroy(collision.gameObject);
        }
    }

    private void ChangeSprite()
    {
        Debug.Log("Ran");
        SpriteRender.sprite = Sprites[PlayerState];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int PlayerState = 1;
    public bool Invincible = false;
    public float InvincibleTimer = 8;
    private float Timer;
    // Start is called before the first frame update
    void Start()
    {
        
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
                    PlayerState = (int)Collected.Type;
                }
            }
            Destroy(collision.gameObject);
        }
    }
}

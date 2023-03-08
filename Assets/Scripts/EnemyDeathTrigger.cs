using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathTrigger : MonoBehaviour
{
    private Transform DeathTriggerTransform;
    private GameObject Enemy;
    private Enemy EnemyScript;
    private BoxCollider2D triggerCollider;

    void Awake()
    {
        DeathTriggerTransform = this.gameObject.transform;
        Enemy = DeathTriggerTransform.parent.gameObject;
        EnemyScript = Enemy.GetComponent<Enemy>();
        triggerCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EnemyScript.dead)
        {
            triggerCollider.enabled = false;
        }
        else triggerCollider.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //print("Killed Enemy");
            EnemyScript.stomp(); //If player jumps on top of enemy, the enemy dies
            triggerCollider.enabled = false;
            Invoke("enableCollider", 10f);
        }
    }

    void enableCollider()
    {
        triggerCollider.enabled = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBreakingBits : MonoBehaviour
{
    Rigidbody2D Rigid2D;
    private float timer;
    public Vector2 direction;
    private bool Once = true;
    // Start is called before the first frame update
    void Start()
    {
        Rigid2D = GetComponent<Rigidbody2D>();
        Rigid2D.excludeLayers = ~0;
        timer = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if(Rigid2D && Once)
        {
            Launch(direction);
            Once = false;
        }
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Launch(Vector2 direction)
    {
        if (Rigid2D)
        Rigid2D.AddForce(direction, ForceMode2D.Impulse);
    }
}

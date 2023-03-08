using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera : MonoBehaviour
{
    public GameObject player;
    private double currentXPos;
    public double previousXPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > previousXPos) {
            currentXPos = player.transform.position.x;
            double difference = currentXPos - previousXPos;
            this.gameObject.transform.position += new Vector3((float)difference, 0, 0);
        }
        previousXPos = this.gameObject.transform.position.x;
    }
}
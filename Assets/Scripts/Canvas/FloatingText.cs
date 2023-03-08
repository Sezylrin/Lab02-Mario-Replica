using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1.0f;
    public float lifetime = 1.5f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * floatSpeed * Time.unscaledDeltaTime);
    }
}

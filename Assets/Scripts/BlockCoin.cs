using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCoin : MonoBehaviour
{
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameManager.Instance.IncrementCoinCount();
        GameManager.Instance.AddToScore(200);
        GameManager.Instance.DisplayFloatingText("200", gameObject.transform.position);
        audioSource.Play();
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 2f;
        yield return Move(restingPosition, animatedPosition);
        yield return Move(animatedPosition, restingPosition);
        spriteRenderer.enabled = false;

        Destroy(gameObject, audioSource.clip.length);
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }
}

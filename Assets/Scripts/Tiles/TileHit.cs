using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum BlockTypes : int
{
    BrickTile = 1,
    QuestionTile = 2,
    MultiCoinTile = 3,
    BrickTileWithitem = 4,
}

public class TileHit : MonoBehaviour
{
    [SerializeField] Sprite emptyBlock;
    [SerializeField] private BlockTypes blockType;
    [SerializeField] private GameObject[] possibleItemsToSpawn;

    private bool animating;
    private int maxHits;
    private SpriteRenderer spriteRenderer;
    private bool timerStarted = false;
    private AudioSource audioSource;
    private BoxCollider2D boxCollider2D;
    private PlayerManager playerManager;
    private Vector3 lastMarioPosition;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        switch (blockType)
        {
            case BlockTypes.BrickTile:
                maxHits = -1;
                break;
            case BlockTypes.QuestionTile:
                maxHits = 1;
                break;
            case BlockTypes.MultiCoinTile:
                maxHits = -1;
                break;
            case BlockTypes.BrickTileWithitem:
                maxHits = 1;
                break;
            default:
                maxHits = -1;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!animating && maxHits != 0 && collision.gameObject.CompareTag("Player"))
        {
            playerManager = collision.gameObject.GetComponent<PlayerManager>();
            if (TestCollision(collision.transform, transform, Vector2.up))
            {
                lastMarioPosition = collision.gameObject.GetComponent<Transform>().position;
                Hit();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (animating && collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (!enemy.dead)
            {
                enemy.death(lastMarioPosition);
            }
        }
    }

    private void Hit()
    {
        //Enable the sprite if it is an invisible block
        spriteRenderer.enabled = true;

        if (playerManager.PlayerState > 1 && blockType == BlockTypes.BrickTile)
        {
            DestroyBrickTile();
        }
        else
        {
            HitTile();
        }
    }

    private void DestroyBrickTile()
    {
        maxHits = 0;
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        audioSource.Play();
        Destroy(gameObject, audioSource.clip.length);
        //TODO: Animate destruction of block then destroy tile
    }

    private void HitTile()
    {
        StartCoroutine(Animate());
        if (maxHits != 0 && possibleItemsToSpawn.Length == 1)
        {
            GameObject itemToSpawn = possibleItemsToSpawn[0];
            if (itemToSpawn != null && itemToSpawn.name == "BlockCoin")
            {
                Instantiate(itemToSpawn, transform.position, Quaternion.identity);
            }
        }

        maxHits--;

        if (maxHits == 0)
        {
            spriteRenderer.sprite = emptyBlock;
        }

        if (blockType == BlockTypes.MultiCoinTile && !timerStarted)
        {
            timerStarted = true;
            StartCoroutine(Enable10SecBlockTimer());
        }
    }

    private IEnumerator Animate()
    {
        animating = true;

        Vector3 restingPosition = transform.localPosition;
        Vector3 animatedPosition = restingPosition + Vector3.up * 0.5f;
        yield return Move(restingPosition, animatedPosition);
        yield return Move(animatedPosition, restingPosition);

        //If the length is 1, it's a regular item tile so spawn the item
        if (possibleItemsToSpawn.Length == 1)
        {
            GameObject itemToSpawn = possibleItemsToSpawn[0];
            if (itemToSpawn != null && itemToSpawn.name != "BlockCoin")
            {
                Instantiate(itemToSpawn, transform.position, Quaternion.identity);
            }
        }

        //If the length is 2, it's a question tile with a mushroom inside of it
        if (possibleItemsToSpawn.Length == 2)
        {
            if (playerManager.PlayerState == 1)
            {
                GameObject mushroom = possibleItemsToSpawn.FirstOrDefault(obj => obj.name == "Mushroom");
                Instantiate(mushroom, transform.position, Quaternion.identity);
            }
            else if (playerManager.PlayerState > 1)
            {
                GameObject fireFlower = possibleItemsToSpawn.FirstOrDefault(obj => obj.name == "Fire Flower");
                Instantiate(fireFlower, transform.position, Quaternion.identity);
            }
        }

        animating = false;
    }

    private IEnumerator Move(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;
        float duration = 0.125f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = to;
    }

    private IEnumerator Enable10SecBlockTimer()
    {
        yield return new WaitForSeconds(10);

        maxHits = 1;
    }

    private bool TestCollision(Transform transformToTest, Transform other, Vector2 testDirection)
    {
        Vector2 direction = other.position - transformToTest.position;
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f;
    }
}

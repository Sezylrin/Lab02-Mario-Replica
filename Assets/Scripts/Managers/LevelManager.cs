using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Loader.Scene scene;

    private void Awake()
    {
        GameManager.Instance.NewGame(scene);
        StartCoroutine(Wait5Seconds());
    }

    private IEnumerator Wait5Seconds()
    {
        yield return new WaitForSeconds(5);
        //GameObject.Find("Mario").GetComponent<PlayerManager>().TakeDamage();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Loader.Scene scene;

    private void Awake()
    {
        GameManager.Instance.NewGame(scene);
    }
}
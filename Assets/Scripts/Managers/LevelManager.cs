using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.NewGame(Loader.Scene.OneOne);
    }
}
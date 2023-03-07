using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvasManager : MonoBehaviour
{
    private void Start()
    {
        Loader.Load(Loader.Scene.OneOne);
    }
}

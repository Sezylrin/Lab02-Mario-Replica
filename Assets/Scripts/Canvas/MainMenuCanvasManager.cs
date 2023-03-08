using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private RectTransform goombaRectTransform;

    private int selectedOptionIndex = 0;
    private Vector2[] optionPositions = new Vector2[2];
    private bool animating = false;

    private void Awake()
    {
        optionPositions[0] = new Vector2(goombaRectTransform.anchoredPosition.x, goombaRectTransform.anchoredPosition.y);
        optionPositions[1] = new Vector2(goombaRectTransform.anchoredPosition.x, -32f);
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !animating)
        {
            animating = true;
            selectedOptionIndex = selectedOptionIndex == 0 ? 1 : 0;
            StartCoroutine(MoveGoomba(goombaRectTransform.anchoredPosition, optionPositions[selectedOptionIndex]));
        }
        else if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && !animating)
        {
            animating = true;
            selectedOptionIndex = selectedOptionIndex == 1 ? 0 : 1;
            StartCoroutine(MoveGoomba(goombaRectTransform.anchoredPosition, optionPositions[selectedOptionIndex]));
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedOptionIndex == 0)
            {
                GameManager.Instance.SetWorld("1-1");
                Loader.Load(Loader.Scene.PreLevel);
            }
            else
            {
                Application.Quit();
            }
        }
    }

    private IEnumerator MoveGoomba(Vector2 from, Vector2 to)
    {
        const float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            goombaRectTransform.anchoredPosition = Vector2.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        goombaRectTransform.anchoredPosition = to;
        animating = false;
    }
}

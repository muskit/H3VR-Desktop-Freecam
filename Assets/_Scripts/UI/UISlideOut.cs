using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISlideOut : MonoBehaviour
{
    public Button slideButton;
    public bool slideFromRight;

    private bool isOpen;
    public float animationTime;

    private RectTransform rectTransform;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        slideButton.onClick.AddListener(HandleButton);

        CheckState();
    }

    private void HandleButton()
    {
        isOpen = !isOpen;
        CheckState();
    }

    // check current state, animate accordingly
    public void CheckState()
    {
        // text
        if (isOpen)
        {
            slideButton.GetComponentInChildren<Text>().text = slideFromRight ? "►" : "◄";
        }
        else
        {
            slideButton.GetComponentInChildren<Text>().text = slideFromRight ? "◄" : "►";
        }

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsedTime = 0;
        if (isOpen) // opening animation
        {
            float targetPoint = 0;
            if (slideFromRight)
            {
                Vector2 startingPos = rectTransform.anchoredPosition;
                Vector2 anchoredPos = rectTransform.anchoredPosition;
                while (anchoredPos.x > targetPoint)
                {
                    anchoredPos.x = Mathf.Lerp(startingPos.x, targetPoint, elapsedTime / animationTime);
                    rectTransform.anchoredPosition = anchoredPos;
                    elapsedTime += Time.deltaTime;
                    yield return null; // wait for next frame
                }
                anchoredPos.x = targetPoint;
                rectTransform.anchoredPosition = anchoredPos;
            }
        }
        else // closing animation
        {
            float targetPoint = rectTransform.sizeDelta.x;
            if (slideFromRight)
            {
                Vector2 startingPos = rectTransform.anchoredPosition;
                Vector2 anchoredPos = rectTransform.anchoredPosition;
                while (anchoredPos.x < targetPoint)
                {
                    anchoredPos.x = Mathf.Lerp(startingPos.x, targetPoint, elapsedTime / animationTime);
                    rectTransform.anchoredPosition = anchoredPos;
                    elapsedTime += Time.deltaTime;
                    yield return null; // wait for next frame
                }
                anchoredPos.x = targetPoint;
                rectTransform.anchoredPosition = anchoredPos;
            }
        }

        yield return null;
    }
}

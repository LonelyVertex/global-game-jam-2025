using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public float duration;

    [Space]
    public Image image;

    private EventSystem _eventSystem;

    private void Start()
    {
        _eventSystem = EventSystem.current;

        if (FindObjectsOfType<FadeController>().Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        _eventSystem.enabled = false;

        var c = image.color;
        c.a = 0.0f;
        image.color = c;

        image.enabled = true;

        for (var current = 0.0f; current < 1.0f; current += Time.deltaTime / duration)
        {
            c.a = current;
            image.color = c;

            yield return null;
        }

        c.a = 1.0f;
        image.color = c;
    }

    private IEnumerator FadeInCoroutine()
    {
        var c = image.color;
        c.a = 1.0f;
        image.color = c;

        image.enabled = true;

        for (var current = 0.0f; current < 1.0f; current += Time.deltaTime / duration)
        {
            c.a = 1.0f - current;
            image.color = c;

            yield return null;
        }

        c.a = 0.0f;
        image.color = c;

        image.enabled = false;

        _eventSystem.enabled = true;
    }
}

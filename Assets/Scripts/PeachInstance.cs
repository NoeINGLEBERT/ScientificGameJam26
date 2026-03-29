using UnityEngine;
using UnityEngine.UI;
using System;

public class PeachInstance : MonoBehaviour
{
    public Image image;
    public Button button;

    private RectTransform rect;
    private Color baseColor;

    private bool isAnimating = false;

    private Action onClickCallback;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (image == null)
            image = GetComponent<Image>();

        if (button == null)
            button = GetComponent<Button>();

        baseColor = image.color;

        button.onClick.AddListener(HandleClick);
    }

    public void SetClickCallback(Action callback)
    {
        onClickCallback = callback;
    }

    void HandleClick()
    {
        if (isAnimating) return;
        onClickCallback?.Invoke();
    }

    // =========================
    // 🎨 COLOR
    // =========================
    public void UpdateColor(float quality)
    {
        Color dark = baseColor * 0.4f;
        image.color = Color.Lerp(dark, baseColor, quality);
    }

    // =========================
    // ✨ PULSE
    // =========================
    public void Pulse()
    {
        if (isAnimating) return;

        float scale = 1f + Mathf.Sin(Time.time * 6f) * 0.1f;

        rect.localScale = Vector3.Lerp(
            rect.localScale,
            Vector3.one * scale,
            Time.deltaTime * 10f
        );
    }

    // =========================
    // 💀 FALL
    // =========================
    public void FallAndFade()
    {
        if (isAnimating) return;
        StartCoroutine(FallRoutine());
    }

    System.Collections.IEnumerator FallRoutine()
    {
        isAnimating = true;
        button.interactable = false;

        Vector2 start = rect.anchoredPosition;
        Vector2 end = start + Vector2.down * 150f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 2f;

            rect.anchoredPosition = Vector2.Lerp(start, end, t);

            Color c = image.color;
            c.a = 1f - t;
            image.color = c;

            yield return null;
        }

        Debug.Log("YES");

        SetVisible(false);
    }

    // =========================
    // 🍑 PICK
    // =========================
    public void PickAndFade()
    {
        if (isAnimating) return;
        StartCoroutine(PickRoutine());
    }

    System.Collections.IEnumerator PickRoutine()
    {
        isAnimating = true;
        button.interactable = false;

        Vector2 start = rect.anchoredPosition;
        Vector2 end = start + Vector2.up * 100f;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;

            rect.anchoredPosition = Vector2.Lerp(start, end, t);

            Color c = image.color;
            c.a = 1f - t;
            image.color = c;

            yield return null;
        }

        Debug.Log("YES");

        SetVisible(false);
    }

    public void SetVisible(bool visible)
    {
        // GameObject
        gameObject.SetActive(visible);

        // Components
        if (image != null)
            image.enabled = visible;

        if (button != null)
        {
            button.enabled = visible;
            button.interactable = visible;
        }

        // Reset animation state when reappearing
        if (visible)
        {
            isAnimating = false;

            // Reset alpha (important after fade)
            Color c = image.color;
            c.a = 1f;
            image.color = c;

            // Reset scale
            rect.localScale = Vector3.one;
        }
    }
}
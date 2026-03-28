using UnityEngine;
using UnityEngine.EventSystems;

public class HoverArrows : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    public class ArrowData
    {
        public RectTransform rect;
        public CanvasGroup canvasGroup;
        public Vector2 direction; // e.g. (-1,0) for left, (1,0) for right

        [HideInInspector] public Vector2 visiblePos;
        [HideInInspector] public Vector2 hiddenPos;
    }

    [Header("Arrows")]
    public ArrowData[] arrows;

    [Header("Animation")]
    public float slideDistance = 20f;
    public float speed = 8f;

    private float targetAlpha = 0f;

    void Start()
    {
        foreach (var arrow in arrows)
        {
            arrow.visiblePos = arrow.rect.anchoredPosition;
            arrow.hiddenPos = arrow.visiblePos + arrow.direction * slideDistance;

            arrow.rect.anchoredPosition = arrow.hiddenPos;

            if (arrow.canvasGroup != null)
                arrow.canvasGroup.alpha = 0f;
        }
    }

    void Update()
    {
        foreach (var arrow in arrows)
        {
            // Move
            arrow.rect.anchoredPosition = Vector2.Lerp(
                arrow.rect.anchoredPosition,
                targetAlpha > 0 ? arrow.visiblePos : arrow.hiddenPos,
                Time.deltaTime * speed
            );

            // Fade
            if (arrow.canvasGroup != null)
            {
                arrow.canvasGroup.alpha = Mathf.Lerp(
                    arrow.canvasGroup.alpha,
                    targetAlpha,
                    Time.deltaTime * speed
                );
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetAlpha = 1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetAlpha = 0f;
    }
}
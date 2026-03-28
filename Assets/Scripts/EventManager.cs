using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class EventManager : MonoBehaviour
{
    [Header("References")]
    public TimelineSystem timeline;
    public TreeSystem tree;

    [Header("Events")]
    public TreeEvent[] monthlyEvents; // 12 entries

    [Header("UI")]
    public CanvasGroup eventPanel;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public Button choiceAButton;
    public Button choiceBButton;

    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;

    public TextMeshProUGUI previewText;

    private TreeEvent currentEvent;

    void Start()
    {
        timeline.OnMonthEnded += TriggerEvent;

        eventPanel.alpha = 0f;
        eventPanel.gameObject.SetActive(false);
    }

    // =========================
    // EVENT TRIGGER
    // =========================
    void TriggerEvent(int month)
    {
        currentEvent = monthlyEvents[month];

        ShowEvent(currentEvent);
    }

    // =========================
    // SHOW UI
    // =========================
    void ShowEvent(TreeEvent e)
    {
        eventPanel.gameObject.SetActive(true);

        titleText.text = e.title;
        descriptionText.text = e.description;

        choiceAText.text = e.choiceA.name;
        choiceBText.text = e.choiceB.name;

        previewText.text = "";

        // Assign buttons
        choiceAButton.onClick.RemoveAllListeners();
        choiceBButton.onClick.RemoveAllListeners();

        choiceAButton.onClick.AddListener(() => SelectChoice(e.choiceA));
        choiceBButton.onClick.AddListener(() => SelectChoice(e.choiceB));

        // Hover preview
        AddHover(choiceAButton, e.choiceA.previewText);
        AddHover(choiceBButton, e.choiceB.previewText);

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            eventPanel.alpha = t;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime * 3f;
            eventPanel.alpha = t;
            yield return null;
        }

        eventPanel.gameObject.SetActive(false);

        timeline.ResumeAfterEvent();
    }

    // =========================
    // CHOICE
    // =========================
    void SelectChoice(Choice choice)
    {
        ApplyEffect(choice.effect);

        StartCoroutine(FadeOut());
    }

    // =========================
    // EFFECTS
    // =========================
    void ApplyEffect(EventEffect effect)
    {
        //switch (effect)
        //{
        //    case EventEffect.AddWaterSmall:
        //        tree.AddWater(2f);
        //        break;

        //    case EventEffect.AddWaterLarge:
        //        tree.AddWater(5f);
        //        break;

        //    case EventEffect.RemoveBranches:
        //        RemoveBranches(2);
        //        break;

        //    case EventEffect.BoostFruits:
        //        BoostFruits();
        //        break;

        //    case EventEffect.DamageLeaves:
        //        DamageLeaves();
        //        break;
        //}
    }

    void RemoveBranches(int amount)
    {
        if (tree.branchHydricStatus.Length <= amount) return;

        System.Array.Resize(ref tree.branchHydricStatus, tree.branchHydricStatus.Length - amount);
    }

    void BoostFruits()
    {
        for (int i = 0; i < tree.peachQuality.Length; i++)
            tree.peachQuality[i] += 0.2f;
    }

    void DamageLeaves()
    {
        for (int i = 0; i < tree.leafHydricStatus.Length; i++)
            tree.leafHydricStatus[i] -= 0.2f;
    }

    // =========================
    // HOVER PREVIEW
    // =========================
    void AddHover(Button button, string text)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        var enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((_) => previewText.text = text);

        var exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((_) => previewText.text = "");

        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }
}
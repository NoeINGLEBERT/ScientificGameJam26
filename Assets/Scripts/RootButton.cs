using UnityEngine;
using UnityEngine.UI;

public class RootButton : MonoBehaviour
{
    public Image iconImage;
    public Image cooldownImage;
    public float cooldown = 5f;
    public float waterAmount = 3f;

    float timer = 0f;
    bool coolingDown = false;

    Color initialColor;

    private void Start()
    {
        initialColor = iconImage.color;
    }

    void Update()
    {
        if (!coolingDown) return;

        timer -= Time.deltaTime;
        cooldownImage.fillAmount = 1f - (timer / cooldown);

        if (timer <= 0f)
        {
            coolingDown = false;
            cooldownImage.fillAmount = 0f;
            iconImage.color = initialColor;
            iconImage.rectTransform.sizeDelta = new Vector2(45, 45);
        }
    }

    public void OnClick()
    {
        if (coolingDown) return;

        GameManager.Instance.AddWater(waterAmount);
        StartCooldown();
    }

    void StartCooldown()
    {
        timer = cooldown;
        coolingDown = true;
        cooldownImage.fillAmount = 1f;
        Color deactivatedColor = initialColor * 0.5f;
        deactivatedColor.a = 1f;
        iconImage.color = deactivatedColor;
        iconImage.rectTransform.sizeDelta = new Vector2(40, 40);
    }
}
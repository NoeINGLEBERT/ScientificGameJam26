using UnityEngine;
using UnityEngine.UI;

public class RootButton : MonoBehaviour
{
    [Header("UI")]
    public Button rootButton;          // bouton à cliquer
    public Image waterBar;             // barre d'eau
    public Image buttonBackground;     // image du bouton à griser

    [Header("Settings")]
    public float clickConsumption = 0.2f; // 20% par clic
    public float cooldownDuration = 30f;  // temps pour recharger
    public float waterMultiplier = 1f;
    public float waterAmount = 3f;

    private float currentWater = 1f;
    private bool isCoolingDown = false;
    private float cooldownTimer = 0f;
    private float cooldownMultiplier = 1f;

    private Color normalColor;
    public Color disabledColor = Color.gray;

    private void Start()
    {
        currentWater = 1f;
        UpdateWaterBar();

        if (buttonBackground != null)
            normalColor = buttonBackground.color;
    }

    private void Update()
    {
        if (!isCoolingDown) return;

        cooldownTimer += Time.deltaTime * cooldownMultiplier;
        currentWater = Mathf.Clamp01(cooldownTimer / cooldownDuration);
        UpdateWaterBar();

        if (buttonBackground != null)
            buttonBackground.color = disabledColor;

        if (cooldownTimer >= cooldownDuration)
        {
            isCoolingDown = false;
            rootButton.interactable = true;
            cooldownDuration = 30f; // remet la durée originale
            cooldownMultiplier = 1f; // remet la vitesse normale

            if (buttonBackground != null)
                buttonBackground.color = normalColor;
        }
    }

    public void OnClick()
    {
        if (isCoolingDown) return;
        GameManager.Instance.AddWater(waterAmount * waterMultiplier);
        currentWater -= clickConsumption;
        currentWater = Mathf.Max(currentWater, 0f);
        UpdateWaterBar();

        if (currentWater <= 0f)
            StartCooldown();
    }

    private void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = 0f;
        rootButton.interactable = false;

        if (buttonBackground != null)
            buttonBackground.color = disabledColor;
    }

    private void UpdateWaterBar()
    {
        if (waterBar != null)
            waterBar.fillAmount = currentWater;
    }
    public void AccelerateCooldown(float multiplier)
    {
        if (isCoolingDown)
        {
            cooldownMultiplier = multiplier;
        }
    }
}
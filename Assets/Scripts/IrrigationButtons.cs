using UnityEngine;
using UnityEngine.UI;

public class IrrigationButtons : MonoBehaviour
{
    [Header("UI")]
    public Button rootButton;       // bouton cliquable
    public Image[] chargeImages;    // images représentant les charges

    [Header("Settings")]
    public RootButton targetRootButton; // optionnel : RootButton pour réduire le cooldown
    public float accelerationMultiplier = 4f; // facteur d'accélération

    private int chargesRemaining;
    private int maxCharges;

    private void Start()
    {
        if (chargeImages == null || chargeImages.Length == 0)
        {
            Debug.LogWarning("Aucune image de charge assignée !");
            return;
        }

        maxCharges = chargeImages.Length;
        chargesRemaining = maxCharges;

        // toutes les images sont actives au départ
        foreach (Image img in chargeImages)
        {
            img.color = Color.white;
        }

        if (rootButton != null)
            rootButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        if (chargesRemaining <= 0)
            return; // plus de charges

        // grise l'image correspondante
        int indexToGray = maxCharges - chargesRemaining;
        if (indexToGray >= 0 && indexToGray < chargeImages.Length)
        {
            chargeImages[indexToGray].color = Color.gray;
        }

        chargesRemaining--;

        // accélère tous les RootButton actuellement en cooldown
        RootButton[] allButtons = FindObjectsOfType<RootButton>();
        foreach (RootButton rb in allButtons)
        {
            if (rb != null)
            {
                rb.AccelerateCooldown(accelerationMultiplier);
            }
        }

        // désactive le bouton si plus de charges
        if (chargesRemaining <= 0 && rootButton != null)
        {
            rootButton.interactable = false;
        }
    }
}
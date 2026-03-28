using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreeUI : MonoBehaviour
{
    [Header("References")]
    public TreeSystem tree;

    [Header("UI")]
    public Image hydricBarFill;
    public TextMeshProUGUI warningText;

    [Header("Colors")]
    public Color dryColor = Color.red;
    public Color optimalColor = Color.green;
    public Color wetColor = Color.blue;

    [Header("Warning Colors")]
    public Color warningDryColor = new Color(1f, 0.5f, 0f);   // orange
    public Color warningWetColor = new Color(0.5f, 0.7f, 1f); // light blue

    [Header("Animation")]
    public float colorLerpSpeed = 5f;

    void Update()
    {
        UpdateHydricBar();
        UpdateWarnings();
    }

    // =========================
    // GLOBAL HYDRIC BAR
    // =========================
    void UpdateHydricBar()
    {
        float avg = tree.GetAverageHydricStatus();

        hydricBarFill.fillAmount = avg;

        // -------------------------
        // BASE COLOR (GLOBAL)
        // -------------------------
        Color targetColor;

        if (avg < tree.optimalMin)
            targetColor = dryColor;
        else if (avg > tree.optimalMax)
            targetColor = wetColor;
        else
            targetColor = optimalColor;

        // -------------------------
        // WARNING OVERRIDE
        // -------------------------
        bool hasDryWarning = false;
        bool hasWetWarning = false;

        float leafAvg = GetAverage(tree.leafHydricStatus);
        float fruitAvg = GetAverage(tree.peachHydricStatus);
        float treeAvg = GetTreeCoreAverage();

        CheckWarning(treeAvg, ref hasDryWarning, ref hasWetWarning);
        CheckWarning(leafAvg, ref hasDryWarning, ref hasWetWarning);
        CheckWarning(fruitAvg, ref hasDryWarning, ref hasWetWarning);

        // Only override if global is "fine"
        if (avg >= tree.optimalMin && avg <= tree.optimalMax)
        {
            if (hasDryWarning)
                targetColor = warningDryColor;
            else if (hasWetWarning)
                targetColor = warningWetColor;
        }

        // -------------------------
        // SMOOTH LERP
        // -------------------------
        hydricBarFill.color = Color.Lerp(
            hydricBarFill.color,
            targetColor,
            Time.deltaTime * colorLerpSpeed
        );
    }

    void CheckWarning(float value, ref bool dry, ref bool wet)
    {
        if (value < tree.optimalMin)
            dry = true;
        else if (value > tree.optimalMax)
            wet = true;
    }

    // =========================
    // WARNINGS SYSTEM
    // =========================
    void UpdateWarnings()
    {
        string warning = "";

        float leafAvg = GetAverage(tree.leafHydricStatus);
        float fruitAvg = GetAverage(tree.peachHydricStatus);
        float treeAvg = GetTreeCoreAverage();

        // Tree (trunk + branches)
        //if (treeAvg < tree.optimalMin)
        //    warning += $"Tree is too dry!\n";
        //else if (treeAvg > tree.optimalMax)
        //    warning += $"Tree is overwatered!\n";
        warning += $"Tree: {(int)(treeAvg * 100)}\n";

        // Leaves
        //if (leafAvg < tree.optimalMin)
        //    warning += $"Leaves are drying!\n";
        //else if (leafAvg > tree.optimalMax)
        //    warning += $"Leaves are soaked!\n";
        warning += $"Leaves: {(int)(leafAvg * 100)}\n";

        // Fruits
        //if (fruitAvg < tree.optimalMin)
        //    warning += $"Fruits lack water!\n";
        //else if (fruitAvg > tree.optimalMax)
        //    warning += $"Fruits are overwatered!\n";
        warning += $"Fruits: {(int)(fruitAvg * 100)}\n";

        warningText.text = warning;
    }

    // =========================
    // HELPERS
    // =========================
    float GetAverage(float[] array)
    {
        if (array.Length == 0) return 0.6f;

        float sum = 0f;

        for (int i = 0; i < array.Length; i++)
            sum += array[i];

        return sum / array.Length;
    }

    float GetTreeCoreAverage()
    {
        float sum = tree.trunkHydricStatus;

        for (int i = 0; i < tree.branchHydricStatus.Length; i++)
            sum += tree.branchHydricStatus[i];

        int count = 1 + tree.branchHydricStatus.Length;

        return sum / count;
    }
}
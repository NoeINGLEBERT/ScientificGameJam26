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

        // Color based on range
        if (avg < tree.optimalMin)
            hydricBarFill.color = dryColor;
        else if (avg > tree.optimalMax)
            hydricBarFill.color = wetColor;
        else
            hydricBarFill.color = optimalColor;
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
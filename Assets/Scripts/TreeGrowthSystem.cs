using UnityEngine;

public class TreeGrowthSystem : MonoBehaviour
{
    public TimelineSystem timeline;
    public TreeSystem tree;

    [Header("Settings")]
    public int leavesPerBranch = 2;
    public int flowersPerBranch = 2;
    public Vector2Int fruitsPerBranch = new Vector2Int(1, 2);

    // Runtime
    private float monthDuration;
    private int currentMonth = -1;

    private int targetLeaves;
    private int targetFlowers;
    private int targetFruits;

    // Accumulators (TIME-BASED)
    private float leafAcc;
    private float flowerAcc;
    private float fruitAcc;
    private float leafFallAcc;

    // 🍑 Deterministic fruit plan
    private int[] fruitPlan; // how many fruits each flower will produce

    private int initialLeafCount;

    void Start()
    {
        monthDuration = timeline.monthDuration;
        currentMonth = timeline.GetCurrentMonth();

        InitMonth(currentMonth);
    }

    void Update()
    {
        int newMonth = timeline.GetCurrentMonth();

        if (newMonth != currentMonth)
        {
            currentMonth = newMonth;
            InitMonth(currentMonth);
        }

        UpdateGrowth();
    }

    // =========================
    // INIT PER MONTH
    // =========================
    void InitMonth(int month)
    {
        leafAcc = 0f;
        flowerAcc = 0f;
        fruitAcc = 0f;
        leafFallAcc = 0f;

        targetLeaves = 0;
        targetFlowers = 0;
        targetFruits = 0;

        int branchCount = tree.branchHydricStatus.Length;

        switch (month)
        {
            case 1: // FEB
            case 2: // MAR
                targetFlowers = branchCount;
                break;

            case 3: // APRIL
                int flowerCount = branchCount;

                targetLeaves = flowerCount;

                break;

            case 8:
                initialLeafCount = tree.leafHydricStatus.Length;
                break;
        }
    }

    // =========================
    // MAIN UPDATE
    // =========================
    void UpdateGrowth()
    {
        switch (currentMonth)
        {
            case 1:
            case 2:
                UpdateFlowers();
                break;

            case 3:
                UpdateLeaves();
                UpdateFruitConversion();
                UpdateFruitGrowth();
                break;

            case 4:
            case 5:
            case 6:
            case 7:
                UpdateFruitGrowth();
                break;

            case 8:
                UpdateLeafFall();
                break;
        }
    }

    // =========================
    // 🌸 FLOWERS
    // =========================
    void UpdateFlowers()
    {
        if (targetFlowers <= 0) return;

        float duration = GetEffectiveDuration();
        float rate = targetFlowers / duration;

        flowerAcc += rate * Time.deltaTime;

        int toSpawn = Mathf.FloorToInt(flowerAcc);
        if (toSpawn <= 0) return;

        flowerAcc -= toSpawn;
        AddFlowersBatch(toSpawn);
    }

    void AddFlowersBatch(int count)
    {
        int len = tree.flowerBloom.Length;
        System.Array.Resize(ref tree.flowerBloom, len + count);

        for (int i = 0; i < count; i++)
        {
            tree.flowerBloom[len + i] = 0f;
            targetFlowers--;
        }

            
    }

    // =========================
    // 🌿 LEAVES
    // =========================
    void UpdateLeaves()
    {
        if (targetLeaves <= 0) return;

        float duration = GetEffectiveDuration();
        float rate = targetLeaves / duration;

        leafAcc += rate * Time.deltaTime;

        int toSpawn = Mathf.FloorToInt(leafAcc);
        if (toSpawn <= 0) return;

        leafAcc -= toSpawn;

        AddLeavesBatch(toSpawn);
    }

    void AddLeavesBatch(int count)
    {
        int len = tree.leafHydricStatus.Length;
        System.Array.Resize(ref tree.leafHydricStatus, len + count);

        for (int i = 0; i < count; i++)
            tree.leafHydricStatus[len + i] = 0.5f;
    }

    // =========================
    // 🍑 FRUIT CONVERSION
    // =========================
    private int fruitPlanIndex = 0;

    void UpdateFruitConversion()
    {
        int flowerCount = tree.flowerBloom.Length;
        if (flowerCount == 0) return;

        float duration = GetEffectiveDuration();

        // Convert ALL flowers within effective duration
        float rate = flowerCount / duration;

        fruitAcc += rate * Time.deltaTime;

        int toConvert = Mathf.FloorToInt(fruitAcc);
        if (toConvert <= 0) return;

        fruitAcc -= toConvert;

        // Clamp to available flowers
        toConvert = Mathf.Min(toConvert, tree.flowerBloom.Length);

        for (int i = 0; i < toConvert; i++)
        {
            ConvertFlowerToFruit();
        }
    }

    void ConvertFlowerToFruit()
    {
        int flowerCount = tree.flowerBloom.Length;
        if (flowerCount == 0) return;

        int index = Random.Range(0, flowerCount);

        // Remove flower (swap-back)
        tree.flowerBloom[index] = tree.flowerBloom[flowerCount - 1];
        System.Array.Resize(ref tree.flowerBloom, flowerCount - 1);

        // 🔥 Random fruit count per flower
        int fruitCount = Random.Range(fruitsPerBranch.x, fruitsPerBranch.y + 1);

        AddFruitsBatch(fruitCount);
    }

    void AddFruitsBatch(int count)
    {
        int len = tree.peachHydricStatus.Length;

        System.Array.Resize(ref tree.peachHydricStatus, len + count);
        System.Array.Resize(ref tree.peachQuality, len + count);
        System.Array.Resize(ref tree.fruitSize, len + count);

        for (int i = 0; i < count; i++)
        {
            int idx = len + i;

            tree.peachHydricStatus[idx] = 0.5f;
            tree.peachQuality[idx] = 0.5f;
            tree.fruitSize[idx] = 0.1f;
        }
    }

    // =========================
    // 🍑 FRUIT GROWTH
    // =========================
    void UpdateFruitGrowth()
    {
        if (tree.fruitSize == null) return;

        float growthPerSecond = (1f - 0.1f) / (4f * monthDuration);
        int month = timeline.GetCurrentMonth();

        for (int i = 0; i < tree.fruitSize.Length; i++)
        {
            tree.fruitSize[i] += growthPerSecond * Time.deltaTime;

            if (month < 7)
                tree.fruitSize[i] = Mathf.Min(tree.fruitSize[i], 0.95f);

            tree.fruitSize[i] = Mathf.Clamp01(tree.fruitSize[i]);
        }
    }

    // =========================
    // 🍂 LEAF FALL
    // =========================
    void UpdateLeafFall()
    {
        int current = tree.leafHydricStatus.Length;
        if (current == 0) return;

        float duration = GetEffectiveDuration();

        // ✅ FIX: use initial count, not current
        float rate = initialLeafCount / duration;

        leafFallAcc += rate * Time.deltaTime;

        int toRemove = Mathf.FloorToInt(leafFallAcc);
        if (toRemove <= 0) return;

        leafFallAcc -= toRemove;

        // Clamp so we never try removing more than we have
        toRemove = Mathf.Min(toRemove, current);

        RemoveLeavesBatch(toRemove);
    }

    void RemoveLeavesBatch(int count)
    {
        int len = tree.leafHydricStatus.Length;
        int newLen = Mathf.Max(0, len - count);

        System.Array.Resize(ref tree.leafHydricStatus, newLen);
    }

    float GetEffectiveDuration()
    {
        return monthDuration * (1f / 3f);
    }
}
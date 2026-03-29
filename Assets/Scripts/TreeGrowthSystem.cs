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
    private float monthTimer;
    private float monthDuration;

    private int currentMonth = -1;

    private int targetLeaves;
    private int targetFlowers;
    private int targetFruits;

    private int grownLeaves;
    private int grownFlowers;
    private int grownFruits;

    void Start()
    {
        monthDuration = timeline.monthDuration;
        currentMonth = timeline.GetCurrentMonth();

        InitMonth(currentMonth);
    }

    void Update()
    {
        int newMonth = timeline.GetCurrentMonth();

        // Detect month change
        if (newMonth != currentMonth)
        {
            currentMonth = newMonth;
            InitMonth(currentMonth);
        }

        monthTimer += Time.deltaTime;

        float progress = Mathf.Clamp01(monthTimer / monthDuration);

        UpdateGrowth(progress);
    }

    // =========================
    // INIT PER MONTH
    // =========================
    void InitMonth(int month)
    {
        monthTimer = 0f;

        grownLeaves = 0;
        grownFlowers = 0;
        grownFruits = 0;

        int branchCount = tree.branchHydricStatus.Length;

        switch (month)
        {
            case 1: // FEBRUARY
                targetLeaves = branchCount * leavesPerBranch;
                tree.leafHydricStatus = new float[0];
                break;

            case 2: // MARCH
                targetFlowers = tree.leafHydricStatus.Length;
                tree.flowerBloom = new float[0];
                break;

            case 3: // APRIL
                targetFruits = tree.flowerBloom.Length;

                tree.peachHydricStatus = new float[0];
                tree.peachQuality = new float[0];
                tree.fruitSize = new float[0];

                break;

            case 8: // SEPTEMBER
                // Leaf fall handled progressively
                break;
        }
    }

    // =========================
    // MAIN UPDATE
    // =========================
    void UpdateGrowth(float progress)
    {
        switch (currentMonth)
        {
            case 1: UpdateLeaves(progress); break;
            case 2: UpdateFlowers(progress); break;
            case 3:
                UpdateFruitConversion(progress); // 🌸 → 🍑 over April
                UpdateFruitGrowth(progress);
                break;
            case 4:
            case 5:
            case 6:
            case 7:
                UpdateFruitGrowth(progress);
                break;
            case 8: UpdateLeafFall(progress); break;
        }
    }

    void UpdateFruitConversion(float progress)
    {
        int expected = Mathf.FloorToInt(progress * targetFruits);

        while (grownFruits < expected && tree.flowerBloom.Length > 0)
        {
            ConvertFlowerToFruit();
            grownFruits++;
        }
    }

    void ConvertFlowerToFruit()
    {
        int flowerCount = tree.flowerBloom.Length;
        if (flowerCount == 0) return;

        // 🔀 Pick random flower
        int index = Random.Range(0, flowerCount);

        // Remove that flower
        for (int i = index; i < flowerCount - 1; i++)
            tree.flowerBloom[i] = tree.flowerBloom[i + 1];

        System.Array.Resize(ref tree.flowerBloom, flowerCount - 1);

        // 🌸 → 🍑 Spawn 1 or 2 fruits
        int fruitToSpawn = Random.Range(1, 3);

        for (int f = 0; f < fruitToSpawn; f++)
        {
            int fruitCount = tree.peachHydricStatus.Length;

            System.Array.Resize(ref tree.peachHydricStatus, fruitCount + 1);
            System.Array.Resize(ref tree.peachQuality, fruitCount + 1);
            System.Array.Resize(ref tree.fruitSize, fruitCount + 1);

            tree.peachHydricStatus[fruitCount] = 0.5f;
            tree.peachQuality[fruitCount] = 0.5f;
            tree.fruitSize[fruitCount] = 0.1f;
        }
    }

    // =========================
    // 🌿 LEAVES (FEB)
    // =========================
    void UpdateLeaves(float progress)
    {
        int expected = Mathf.FloorToInt(progress * targetLeaves);

        while (grownLeaves < expected)
        {
            AddLeaf();
            grownLeaves++;
        }
    }

    void AddLeaf()
    {
        int len = tree.leafHydricStatus.Length;

        System.Array.Resize(ref tree.leafHydricStatus, len + 1);
        tree.leafHydricStatus[len] = 0.5f;
    }

    // =========================
    // 🌸 FLOWERS (MARCH)
    // =========================
    void UpdateFlowers(float progress)
    {
        int expected = Mathf.FloorToInt(progress * targetFlowers);

        while (grownFlowers < expected)
        {
            AddFlower();
            grownFlowers++;
        }
    }

    void AddFlower()
    {
        int len = tree.flowerBloom.Length;

        System.Array.Resize(ref tree.flowerBloom, len + 1);
        tree.flowerBloom[len] = 0f;
    }

    // =========================
    // 🍑 FRUITS (APRIL → AUG)
    // =========================
    void StartFruits(int branchCount)
    {
        // Remove flowers
        tree.flowerBloom = new float[0];

        targetFruits = 0;

        for (int i = 0; i < branchCount; i++)
            targetFruits += Random.Range(fruitsPerBranch.x, fruitsPerBranch.y + 1);

        tree.peachHydricStatus = new float[targetFruits];
        tree.peachQuality = new float[targetFruits];
        tree.fruitSize = new float[targetFruits];

        for (int i = 0; i < targetFruits; i++)
        {
            tree.peachHydricStatus[i] = 0.5f;
            tree.peachQuality[i] = 0.5f;
            tree.fruitSize[i] = 0.1f; // small start
        }
    }

    void UpdateFruitGrowth(float progress)
    {
        if (tree.fruitSize == null) return;

        float growthPerSecond = (1f - 0.1f) / (4f * monthDuration);

        int month = timeline.GetCurrentMonth();

        for (int i = 0; i < tree.fruitSize.Length; i++)
        {
            // Grow continuously
            tree.fruitSize[i] += growthPerSecond * Time.deltaTime;

            // 🚫 Don't allow full growth before August
            if (month < 7)
                tree.fruitSize[i] = Mathf.Min(tree.fruitSize[i], 0.95f);

            // Clamp final size
            tree.fruitSize[i] = Mathf.Clamp01(tree.fruitSize[i]);
        }
    }

    // =========================
    // 🍂 LEAF FALL (SEPT)
    // =========================
    void UpdateLeafFall(float progress)
    {
        int total = tree.leafHydricStatus.Length;
        int targetRemaining = Mathf.CeilToInt((1f - progress) * total);

        while (tree.leafHydricStatus.Length > targetRemaining)
        {
            RemoveLeaf();
        }
    }

    void RemoveLeaf()
    {
        int len = tree.leafHydricStatus.Length;
        if (len == 0) return;

        System.Array.Resize(ref tree.leafHydricStatus, len - 1);
    }
}
using UnityEngine;

public class TreeSystem : MonoBehaviour
{
    [Header("Tree Parts")]
    public float trunkHydricStatus;
    public float[] branchHydricStatus;
    public float[] leafHydricStatus;
    public float[] peachHydricStatus;
    public float[] peachQuality;

    [Header("Consumption Rates")]
    public float consumptionRate = 1f;
    public float trunkConsumption = 0.03f;
    public float branchConsumption = 0.03f;
    public float leafConsumption = 0.95f;
    public float peachConsumption = 0.02f;

    [Header("Storage Capacity")]
    public float trunkCapacity = 60f;
    public float branchCapacity = 60f;
    public float leafCapacity = 10f;
    public float peachCapacity = 15f;

    [Header("Hydric Settings")]
    public float minHydric = 0f;
    public float maxHydric = 1f;

    [Header("Optimal Range")]
    public float optimalMin = 0.4f;
    public float optimalMax = 0.7f;

    [Header("Quality")]
    public float qualityGainRate = 0.2f;
    public float qualityLossRate = 0.3f;

    [Header("Season")]
    public float seasonalMultiplier = 1f;

    [Header("Runtime State")]
    public bool isPaused = false;

    public float[] flowerBloom;
    public float[] fruitSize;

    public float consumptionMultiplier = 1f;
    void Start()
    {
        InitializeArrays();
    }

    void Update()
    {
        if (isPaused) return;

        float dt = Time.deltaTime;

        DrainAll(consumptionRate * consumptionMultiplier * dt);
        UpdateFruitQuality(dt);
    }

    void InitializeArrays()
    {
        trunkHydricStatus = 0.5f;

        for (int i = 0; i < branchHydricStatus.Length; i++)
            branchHydricStatus[i] = 0.5f;

        for (int i = 0; i < leafHydricStatus.Length; i++)
            leafHydricStatus[i] = 0.5f;

        for (int i = 0; i < peachHydricStatus.Length; i++)
        {
            peachHydricStatus[i] = 0.5f;
            peachQuality[i] = 0.5f;
        }
    }

    // =========================
    // WATER DISTRIBUTION (FINAL)
    // =========================
    public void AddWater(float amount)
    {
        float totalDemand =
            trunkConsumption +
            branchHydricStatus.Length * branchConsumption +
            leafHydricStatus.Length * leafConsumption +
            peachHydricStatus.Length * peachConsumption;

        if (totalDemand <= 0f) return;

        float trunkShare = trunkConsumption / totalDemand;
        float branchShare = (branchHydricStatus.Length * branchConsumption) / totalDemand;
        float leafShare = (leafHydricStatus.Length * leafConsumption) / totalDemand;
        float peachShare = (peachHydricStatus.Length * peachConsumption) / totalDemand;

        float remaining = 0f;

        remaining += DistributeToOptimal(ref trunkHydricStatus, amount * trunkShare);
        remaining += DistributeToOptimal(branchHydricStatus, amount * branchShare);
        remaining += DistributeToOptimal(leafHydricStatus, amount * leafShare);
        remaining += DistributeToOptimal(peachHydricStatus, amount * peachShare);

        // NEW: Overflow redistribution (bad for tree)
        if (remaining > 0f)
        {
            DistributeOverflowSmart(remaining);
        }
    }

    // =========================
    // SMART DISTRIBUTION
    // =========================

    float DistributeToOptimal(ref float value, float water)
    {
        float need = Mathf.Max(0f, optimalMax - 0.1f - value);
        float used = Mathf.Min(need, water);

        value += used;
        return water - used;
    }

    float DistributeToOptimal(float[] parts, float water)
    {
        if (parts.Length == 0) return water;

        float totalNeed = 0f;

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] < optimalMax - 0.1f)
                totalNeed += (optimalMax - 0.1f - parts[i]);
        }

        if (totalNeed <= 0f)
            return water;

        float remaining = water;

        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i] >= optimalMax - 0.1f) continue;

            float need = optimalMax - 0.1f - parts[i];
            float share = need / totalNeed;

            float given = water * share;

            parts[i] += given;
            remaining -= given;
        }

        return remaining;
    }

    void DistributeOverflowSmart(float water)
    {
        const int MAX_ITERATIONS = 5;

        // =========================
        // PHASE 1: Fill to optimalMax
        // =========================
        water = DistributeTowardsTarget(water, optimalMax - 0.1f, MAX_ITERATIONS);

        if (water <= 0.0001f)
            return;

        // =========================
        // PHASE 2: Overflow to maxHydric
        // =========================
        water = DistributeTowardsTarget(water, maxHydric, MAX_ITERATIONS);
    }

    float DistributeTowardsTarget(float water, float target, int iterations)
    {
        for (int iter = 0; iter < iterations; iter++)
        {
            float totalNeed = 0f;

            totalNeed += Mathf.Max(0f, target - trunkHydricStatus);
            totalNeed += GetTotalNeed(branchHydricStatus, target);
            totalNeed += GetTotalNeed(leafHydricStatus, target);
            totalNeed += GetTotalNeed(peachHydricStatus, target);

            if (totalNeed <= 0f)
                return water;

            float used = 0f;

            used += Fill(ref trunkHydricStatus, water, totalNeed, target);
            used += Fill(branchHydricStatus, water, totalNeed, target);
            used += Fill(leafHydricStatus, water, totalNeed, target);
            used += Fill(peachHydricStatus, water, totalNeed, target);

            water -= used;

            if (water <= 0.0001f)
                break;
        }

        return water;
    }

    float GetTotalNeed(float[] parts, float target)
    {
        float total = 0f;

        for (int i = 0; i < parts.Length; i++)
            total += Mathf.Max(0f, target - parts[i]);

        return total;
    }

    float Fill(ref float value, float water, float totalNeed, float target)
    {
        float need = Mathf.Max(0f, target - value);
        if (need <= 0f) return 0f;

        float share = need / totalNeed;
        float given = Mathf.Min(need, water * share);

        value += given;
        return given;
    }

    float Fill(float[] parts, float water, float totalNeed, float target)
    {
        float used = 0f;

        for (int i = 0; i < parts.Length; i++)
        {
            float need = Mathf.Max(0f, target - parts[i]);
            if (need <= 0f) continue;

            float share = need / totalNeed;
            float given = Mathf.Min(need, water * share);

            parts[i] += given;
            used += given;
        }

        return used;
    }

    void AddToArray(float[] array, float value)
    {
        for (int i = 0; i < array.Length; i++)
            array[i] += value;
    }

    void ClampArray(float[] array)
    {
        for (int i = 0; i < array.Length; i++)
            array[i] = Mathf.Clamp(array[i], minHydric, maxHydric);
    }

    // =========================
    // DRAIN
    // =========================
    void DrainAll(float dt)
    {
        trunkHydricStatus -= (trunkConsumption / trunkCapacity) * seasonalMultiplier * dt;
        trunkHydricStatus = Mathf.Clamp(trunkHydricStatus, minHydric, maxHydric);

        Drain(branchHydricStatus, branchConsumption, branchCapacity, dt);
        Drain(leafHydricStatus, leafConsumption, leafCapacity, dt);
        Drain(peachHydricStatus, peachConsumption, peachCapacity, dt);
    }

    void Drain(float[] parts, float consumption, float capacity, float dt)
    {
        float drainRate = consumption / capacity;

        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] -= drainRate * seasonalMultiplier * dt;
            parts[i] = Mathf.Clamp(parts[i], minHydric, maxHydric);
        }
    }

    // =========================
    // QUALITY
    // =========================
    void UpdateFruitQuality(float dt)
    {
        for (int i = 0; i < peachHydricStatus.Length; i++)
        {
            if (peachHydricStatus[i] >= optimalMin && peachHydricStatus[i] <= optimalMax)
                peachQuality[i] += qualityGainRate * dt;
            else
                peachQuality[i] -= qualityLossRate * dt;

            peachQuality[i] = Mathf.Clamp01(peachQuality[i]);
        }
    }

    // =========================
    // AVERAGE HYDRIC STATUS
    // =========================
    public float GetAverageHydricStatus()
    {
        float total = trunkHydricStatus;
        int count = 1;

        total += SumArray(branchHydricStatus, ref count);
        total += SumArray(leafHydricStatus, ref count);
        total += SumArray(peachHydricStatus, ref count);

        return total / count;
    }

    float SumArray(float[] array, ref int count)
    {
        float sum = 0f;

        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
            count++;
        }

        return sum;
    }

    // =========================
    // SCORE
    // =========================
    public float GetScore()
    {
        float score = 0f;

        for (int i = 0; i < peachQuality.Length; i++)
        {
            score += Mathf.Pow(peachQuality[i], 2f);
        }

        return score;
    }
}
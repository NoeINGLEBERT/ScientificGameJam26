using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TreeSystem tree;

    void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Debug.LogWarning(score);
    }

    public void AddWater(float amount)
    {
        tree.AddWater(amount);
    }

    public float score = 0f;
    public int peachesCollected = 0;

    public void CollectPeach(float quality)
    {
        float value = quality * quality;

        score += value;
        peachesCollected++;

        Debug.Log($"Collected peach → +{value} | Total: {score} | Count: {peachesCollected}");
    }
}
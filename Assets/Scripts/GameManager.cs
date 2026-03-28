using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TreeSystem tree;

    void Awake()
    {
        Instance = this;
    }

    public void AddWater(float amount)
    {
        tree.AddWater(amount);
    }
}
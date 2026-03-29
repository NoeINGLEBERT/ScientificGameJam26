using UnityEngine;
using UnityEngine.UI;

public class TreeVisual : MonoBehaviour
{
    public TreeSystem tree;

    [Header("Branches")]
    public Image[] branches;

    [Header("Leaves")]
    public Image[] leaves;

    [Header("Leaf Sprites")]
    public Sprite[] smallLeaves;
    public Sprite[] fullLeaves;

    [Header("Flowers")]
    public Image[] flowers;

    [Header("Flower Sprites")]
    public Sprite[] budSprites;
    public Sprite[] bloomSprites;

    [Header("Fruits")]
    public Image[] fruits;

    void Update()
    {
        UpdateBranches();
        UpdateLeaves();
        UpdateFlowers();
        UpdateFruits();
    }

    // =========================
    // 🌳 BRANCHES
    // =========================
    void UpdateBranches()
    {
        int branchCount = tree.branchHydricStatus.Length;

        for (int i = 0; i < branches.Length; i++)
        {
            branches[i].gameObject.SetActive(i < branchCount);
        }
    }

    // =========================
    // 🍃 LEAVES
    // =========================
    void UpdateLeaves()
    {
        int leafCount = tree.leafHydricStatus.Length;
        int branchCount = tree.branchHydricStatus.Length;

        if (branchCount == 0) return;

        for (int i = 0; i < leaves.Length; i++)
        {
            if (i < leafCount)
            {
                leaves[i].gameObject.SetActive(true);

                int quotient = i / branchCount;   // growth stage
                int remainder = i % branchCount; // which leaf slot

                if (remainder < smallLeaves.Length)
                {
                    if (quotient == 0)
                        leaves[i].sprite = smallLeaves[remainder];
                    else
                        leaves[i].sprite = fullLeaves[remainder];
                }
            }
            else
            {
                leaves[i].gameObject.SetActive(false);
            }
        }
    }

    // =========================
    // 🌸 FLOWERS
    // =========================
    void UpdateFlowers()
    {
        int flowerCount = tree.flowerBloom.Length;
        int branchCount = tree.branchHydricStatus.Length;

        if (branchCount == 0) return;

        for (int i = 0; i < flowers.Length; i++)
        {
            if (i < flowerCount)
            {
                flowers[i].gameObject.SetActive(true);

                int quotient = i / branchCount;
                int remainder = i % branchCount;

                if (remainder < budSprites.Length)
                {
                    if (quotient == 0)
                        flowers[i].sprite = budSprites[remainder];
                    else
                        flowers[i].sprite = bloomSprites[remainder];
                }
            }
            else
            {
                flowers[i].gameObject.SetActive(false);
            }
        }
    }

    // =========================
    // 🍑 FRUITS
    // =========================
    void UpdateFruits()
    {
        int fruitCount = tree.fruitSize.Length;

        for (int i = 0; i < fruits.Length; i++)
        {
            if (i < fruitCount)
            {
                fruits[i].gameObject.SetActive(true);

                float size = tree.fruitSize[i];

                // Scale fruit based on size
                fruits[i].transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 1f, size);
            }
            else
            {
                fruits[i].gameObject.SetActive(false);
            }
        }
    }
}
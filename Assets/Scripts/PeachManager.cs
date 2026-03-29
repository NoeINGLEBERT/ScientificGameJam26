using UnityEngine;
using UnityEngine.UI;

public class PeachManager : MonoBehaviour
{
    public TreeSystem tree;

    public PeachInstance[] peaches;

    public TreeVisual visual;

    void Start()
    {
        AutoAssignPeaches();
    }

    void Update()
    {
        UpdatePeaches();
        CheckAllInvisible();
    }

    void CheckAllInvisible()
    {
        if (peaches == null || peaches.Length == 0) return;

        for (int i = 0; i < tree.peachHydricStatus.Length; i++)
        {
            var p = peaches[i];

            if (p == null) continue;

            // If ANY peach is still visible → stop
            if (p.image != null && p.image.enabled)
                return;
        }

        // 🔥 All peaches invisible → clear tree
        tree.peachHydricStatus = new float[0];
        tree.peachQuality = new float[0];
        tree.fruitSize = new float[0];

        Debug.Log("All peaches gone → tree reset");
    }

    // =========================
    // AUTO ASSIGN (NO INSPECTOR)
    // =========================
    void AutoAssignPeaches()
    {
        Debug.Log("peacibzqd");

        if (visual == null || visual.fruits == null)
        {
            Debug.LogError("TreeVisual or fruits not found!");
            peaches = new PeachInstance[0];
            return;
        }

        Image[] fruitImages = visual.fruits;

        Debug.Log(visual.fruits.Length);

        peaches = new PeachInstance[fruitImages.Length];

        for (int i = 0; i < fruitImages.Length; i++)
        {
            if (fruitImages[i] != null)
            {
                peaches[i] = fruitImages[i].GetComponent<PeachInstance>();

                int index = i;

                if (peaches[i] != null)
                {
                    peaches[i].SetClickCallback(() => OnPeachClicked(index));
                }
                else
                {
                    Debug.LogWarning($"No PeachInstance on {fruitImages[i].name}");
                }
            }
        }
    }

    // =========================
    // HANDLE ARRAY SIZE CHANGES
    // =========================
    void SyncIfNeeded()
    {
        int target = tree.peachQuality.Length;

        if (peaches.Length == target) return;

        int count = Mathf.Min(peaches.Length, target);

        PeachInstance[] newPeaches = new PeachInstance[count];

        for (int i = 0; i < count; i++)
        {
            newPeaches[i] = peaches[i];
        }

        peaches = newPeaches;
    }

    // =========================
    // UPDATE
    // =========================
    void UpdatePeaches()
    {
        for (int i = 0; i < peaches.Length; i++)
        {
            var p = peaches[i];

            if (p == null || !p.gameObject.activeSelf) continue;

            float quality = tree.peachQuality[i];
            float size = tree.fruitSize[i];

            // 🎨 Color always updates
            p.UpdateColor(quality);

            // 💀 Fall
            if (quality < 0.25f)
            {
                p.FallAndFade();
                continue;
            }

            // ✨ Pulse (only at full size)
            if (size >= 1f)
            {
                p.Pulse();
            }
        }
    }

    // =========================
    // CLICK (UI BUTTON)
    // =========================
    void OnPeachClicked(int i)
    {
        if (i >= tree.fruitSize.Length) return;
        if (i >= tree.peachQuality.Length) return;

        if (tree.fruitSize[i] >= 1f)
        {
            float quality = tree.peachQuality[i];

            // 🍑 REWARD
            GameManager.Instance.CollectPeach(quality);

            // 🎬 Animation
            peaches[i].PickAndFade();
        }
    }
}
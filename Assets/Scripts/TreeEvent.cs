using UnityEngine;

[CreateAssetMenu(menuName = "TreeGame/Event")]
public class TreeEvent : ScriptableObject
{
    public string title;
    [TextArea(3, 6)] public string description;

    public Choice choiceA;
    public Choice choiceB;
}

[System.Serializable]
public class Choice
{
    public string name;
    [TextArea(2, 4)] public string previewText;

    public EventEffect effect;
}

public enum EventEffect
{
    None
}
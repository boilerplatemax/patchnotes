using UnityEngine;
using TMPro; // for TextMeshPro UI, optional

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [Header("Resource Counts")]
    public int wood = 0;
    public int stone = 0;

    [Header("UI")]
    public TMP_Text woodText;
    public TMP_Text stoneText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        UpdateUI();
    }

    public void AddResource(Resource.ResourceType type, int amount)
    {
        switch (type)
        {
            case Resource.ResourceType.Tree:
                wood += amount;
                break;
            case Resource.ResourceType.Mine:
                stone += amount;
                break;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (woodText != null) woodText.text = $"Wood: {wood}";
        if (stoneText != null) stoneText.text = $"Stone: {stone}";
    }
}

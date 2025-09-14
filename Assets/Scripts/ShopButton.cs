using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ShopButton : MonoBehaviour
{
    [Header("Shop Settings")]
    public int cost = 25;
    public Color affordableColor = Color.white;
    public Color unaffordableColor = new Color(1f, 1f, 1f, 0.4f);
    public Color fillAffordableColor = Color.green;
    public Color fillUnaffordableColor = Color.red;

    [Header("Cooldown Settings")]
    public float cooldownDuration = 15f;

    [Header("UI Elements")]
    public Image cooldownFill; // assign a child Image set as "Filled" type

    private Button button;
    private Image fillImage; // main button background
    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;

    void Awake()
    {
        button = GetComponent<Button>();
        fillImage = button.GetComponent<Image>();

        button.onClick.AddListener(StartCooldown);

        if (cooldownFill != null)
            cooldownFill.fillAmount = 1f; // start full
    }

    void Update()
    {
        if (isCoolingDown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                EndCooldown();
            }

            if (cooldownFill != null)
                cooldownFill.fillAmount = 1f - (cooldownTimer / cooldownDuration);
        }
    }

    public void UpdateInteractable(int currentCurrency)
    {
        bool canAfford = currentCurrency >= cost;
        bool canClick = canAfford && !isCoolingDown;

        if (button != null)
            button.interactable = canClick;

        if (fillImage != null)
            fillImage.color = canClick ? fillAffordableColor : fillUnaffordableColor;

        Color childTint = canClick ? affordableColor : unaffordableColor;
        ApplyTintToChildren(childTint);
    }

    void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownDuration;
        button.interactable = false;

        if (fillImage != null)
            fillImage.color = fillUnaffordableColor;

        if (cooldownFill != null)
            cooldownFill.fillAmount = 0f; // empty start

        ApplyTintToChildren(unaffordableColor);
    }

    void EndCooldown()
    {
        isCoolingDown = false;
        cooldownTimer = 0f;

        if (cooldownFill != null)
            cooldownFill.fillAmount = 1f; // full when ready
    }

    private void ApplyTintToChildren(Color tint)
    {
        foreach (var img in GetComponentsInChildren<Image>())
        {
            if (img != fillImage && img != cooldownFill)
                img.color = tint;
        }

        foreach (var txt in GetComponentsInChildren<TextMeshProUGUI>())
            txt.color = tint;

        foreach (var txt in GetComponentsInChildren<Text>())
            txt.color = tint;
    }
}

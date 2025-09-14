using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Camera mainCamera; // assign in inspector

    [Header("Currency")]
    public int currency = 0;

    [Header("Prefabs & References")]
    public DraggableWindow window; // assign your main window GO
    public GameObject unitPrefab;       // base unit
    public GameObject specialUnitPrefab; // <-- NEW unit type
    public GameObject boomboxPrefab;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip buySuccessSFX;
    public AudioClip buyFailSFX;

    [Header("Window Upgrade Settings")]
    public float cameraZDecrease = -1f;   // how much camera moves back each upgrade
    public float windowIncreaseAmount = 2f;
    public int maxWindowUpgrades = 8;
    private int currentWindowUpgrades = 0;
    public GameObject windowUpgradeButtonGO; // optional UI button to hide when maxed

    [Header("Click Radius Upgrade")]
    public float clickRadiusIncreaseAmount = 2f;
    public int maxClickRadiusUpgrades = 10;
    private int currentClickRadiusUpgrades = 0;
    public GameObject clickRadiusButtonGO; // optional UI button to hide when maxed
    public ClickManager clickManager; // reference to your ClickManager script

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int GetCurrency() => currency;

    public void AddCurrency(int amount)
    {
        currency += amount;
        if (UIManager.Instance != null)
            UIManager.Instance.UpdateCurrencyUI(currency);
    }

    public bool TrySpendCurrency(int amount)
    {
        if (currency >= amount)
        {
            currency -= amount;
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateCurrencyUI(currency);

            if (audioSource && buySuccessSFX)
                audioSource.PlayOneShot(buySuccessSFX);

            return true;
        }

        if (audioSource && buyFailSFX)
            audioSource.PlayOneShot(buyFailSFX);

        return false;
    }

    // Window upgrade
// Window upgrade
public void BuyWindowSize(int cost)
{
    if (window == null || mainCamera == null) return;
    if (currentWindowUpgrades >= maxWindowUpgrades) return;

    if (TrySpendCurrency(cost))
    {
        // Increase window size
        window.transform.localScale += new Vector3(windowIncreaseAmount, windowIncreaseAmount, 0f);

        // ✅ Re-clamp position after resize
        Vector3 clampedPos = window.ClampToMaxDistance(window.transform.position);
        window.transform.position = clampedPos;

        // Zoom out camera (orthographic)
        mainCamera.orthographicSize += windowIncreaseAmount * 0.5f;

        currentWindowUpgrades++;

        // Hide the button if max upgrades reached
        if (windowUpgradeButtonGO != null && currentWindowUpgrades >= maxWindowUpgrades)
            windowUpgradeButtonGO.SetActive(false);
    }
}


    // Click radius upgrade
    public void BuyClickRadiusUpgrade(int cost)
    {
        if (clickManager == null) return;
        if (currentClickRadiusUpgrades >= maxClickRadiusUpgrades) return;

        if (TrySpendCurrency(cost))
        {
            clickManager.clickRadius += clickRadiusIncreaseAmount;
            currentClickRadiusUpgrades++;

            // Hide the button if max upgrades reached
            if (clickRadiusButtonGO != null && currentClickRadiusUpgrades >= maxClickRadiusUpgrades)
                clickRadiusButtonGO.SetActive(false);
        }
    }

    // Generalized Buy function
    public void BuyItem(GameObject prefab, int cost)
    {
        if (prefab == null || window == null) return;

        if (TrySpendCurrency(cost))
            Instantiate(prefab, window.transform.position, Quaternion.identity);
    }

    // Convenience methods for UI buttons
    public void BuyUnit(int cost) => BuyItem(unitPrefab, cost);
    public void BuySpecialUnit(int cost) => BuyItem(specialUnitPrefab, cost); // <-- NEW
    public void BuyBoombox(int cost) => BuyItem(boomboxPrefab, cost);
    public void BuyWindowUpgrade(int cost) => BuyWindowSize(cost);
}

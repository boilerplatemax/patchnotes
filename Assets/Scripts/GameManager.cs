using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Camera mainCamera; // assign in inspector

    [Header("Currency")]
public int currency = 0;
public int totalCurrencyEarned = 0; // NEW


    [Header("Prefabs & References")]
    public DraggableWindow window; // assign your main window GO
    public GameObject boomboxPrefab;

    [Header("Bugs")]
    public GameObject[] bugPrefabs;   // assign bug prefabs here
    public int[] bugCosts;            // costs aligned by index to bugPrefabs

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip buySuccessSFX;
    public AudioClip buyFailSFX;

[Header("Window Upgrade Settings")]
public float cameraZDecrease = -1f;   // how much camera moves back each upgrade
public float windowIncreaseAmount = 4f;
public int maxWindowUpgrades = 8;
private int currentWindowUpgrades = 0;
    public GameObject windowUpgradeButtonGO; // optional UI button to hide when maxed

public TMP_Text windowUpgradeCostText;
    public int[] windowUpgradeCosts = { 50, 100, 200, 325, 500, 650, 800, 1000 };


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

    void Start()
{
    UpdateWindowUpgradeCostUI();
}

    public int GetCurrency() => currency;

public void AddCurrency(int amount)
{
    currency += amount;
    totalCurrencyEarned += amount; // track total earned

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

    // ---------------------------
    // WINDOW UPGRADE
    // ---------------------------
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

    // ---------------------------
    // CLICK RADIUS UPGRADE
    // ---------------------------
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

    // ---------------------------
    // BUG PURCHASE
    // ---------------------------
    public void BuyBug(int index)
    {
        if (bugPrefabs == null || bugPrefabs.Length == 0) return;
        if (index < 0 || index >= bugPrefabs.Length) return;

        GameObject prefab = bugPrefabs[index];
        int cost = (bugCosts != null && index < bugCosts.Length) ? bugCosts[index] : 0;

        if (TrySpendCurrency(cost))
            Instantiate(prefab, window.transform.position, Quaternion.identity);
    }

    // ---------------------------
    // OTHER ITEMS
    // ---------------------------
    public void BuyBoombox(int cost) => BuyItem(boomboxPrefab, cost);

    private void BuyItem(GameObject prefab, int cost)
    {
        if (prefab == null || window == null) return;

        if (TrySpendCurrency(cost))
            Instantiate(prefab, window.transform.position, Quaternion.identity);
    }

private void UpdateWindowUpgradeCostUI()
{
    if (windowUpgradeCostText != null)
    {
        if (currentWindowUpgrades >= maxWindowUpgrades)
            windowUpgradeCostText.text = "MAX";
        else
            windowUpgradeCostText.text = $"{windowUpgradeCosts[currentWindowUpgrades]}";
    }

    // 🔥 Tell button script the new cost
    if (windowUpgradeButtonGO != null)
    {
        ShopButton shopBtn = windowUpgradeButtonGO.GetComponent<ShopButton>();
        if (shopBtn != null && currentWindowUpgrades < maxWindowUpgrades)
            shopBtn.SetCost(windowUpgradeCosts[currentWindowUpgrades]);
    }
}

   public void BuyWindowUpgrade()
{
    if (window == null || mainCamera == null) return;
    if (currentWindowUpgrades >= maxWindowUpgrades) return;

    int cost = windowUpgradeCosts[Mathf.Min(currentWindowUpgrades, windowUpgradeCosts.Length - 1)];

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

        // Update the cost display
        UpdateWindowUpgradeCostUI();

        // Hide the button if max upgrades reached
        if (windowUpgradeButtonGO != null && currentWindowUpgrades >= maxWindowUpgrades)
            windowUpgradeButtonGO.SetActive(false);
    }
}

}

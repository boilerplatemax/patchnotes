using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Currency UI")]
    public TMP_Text currencyText;

    [Header("Shop Buttons")]
    public List<ShopButton> shopButtons = new List<ShopButton>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Keep button states refreshed
        foreach (var shopBtn in shopButtons)
        {
            shopBtn.UpdateInteractable(GameManager.Instance.GetCurrency());
        }
    }

    public void UpdateCurrencyUI(int newAmount)
    {
        if (currencyText != null)
            currencyText.text = "" + newAmount;
    }
}

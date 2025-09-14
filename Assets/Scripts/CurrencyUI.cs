using UnityEngine;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    public TextMeshProUGUI currencyText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            currencyText.text = "" + GameManager.Instance.currency;
        }
    }
}

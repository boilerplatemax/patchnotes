using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ShopButtonCooldown : MonoBehaviour
{
    public float cooldownDuration = 15f;

    private Button button;
    private float cooldownTimer = 0f;
    private bool isCoolingDown = false;

    void Awake()
    {
        button = GetComponent<Button>();

        // Wrap the button’s normal onClick with cooldown start
        button.onClick.AddListener(StartCooldown);
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
        }
    }

    void StartCooldown()
    {
        isCoolingDown = true;
        cooldownTimer = cooldownDuration;
        button.interactable = false;
    }

    void EndCooldown()
    {
        isCoolingDown = false;
        cooldownTimer = 0f;
        button.interactable = true;
    }
}

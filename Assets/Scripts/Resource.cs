using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum ResourceType { Tree, Mine }
    public ResourceType resourceType;

    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Depleted Form")]
    public GameObject depletedPrefab;

    [Header("Resource Yield")]
    public int yieldAmount = 1; // how much is given to player when depleted

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            OnDepleted();
        }
    }

    private void OnDepleted()
    {
        // Give resources to player
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddResource(resourceType, yieldAmount);
        }

        // Spawn depleted version
        if (depletedPrefab != null)
        {
            Instantiate(depletedPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}

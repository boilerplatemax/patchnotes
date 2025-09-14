using System.Collections.Generic;
using UnityEngine;

public class BoomBox : MonoBehaviour
{
    [Header("Boombox Settings")]
    public float attractionRadius = 5f;       // distance units start moving toward it
    public float attractionForce = 5f;        // physics force applied toward boombox
    public float autoClickInterval = 1f;      // seconds between auto-earn
    public float lifetime = 30f;              // auto-destroy after X seconds
    public GameObject floatingTextPrefab;     // optional: reuse unit text prefab

    private float timer = 0f;
    private float clickTimer = 0f;
    private List<Unit> nearbyUnits = new List<Unit>();

    void Update()
    {
        timer += Time.deltaTime;
        clickTimer += Time.deltaTime;

        // Auto-destroy
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Find units in radius
        Unit[] allUnits = GameObject.FindObjectsOfType<Unit>();
        nearbyUnits.Clear();

        foreach (var unit in allUnits)
        {
            if (unit.canEarn && Vector2.Distance(transform.position, unit.transform.position) <= attractionRadius)
            {
                nearbyUnits.Add(unit);

                // Apply small force toward boombox
                Rigidbody2D rb = unit.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 dir = (transform.position - unit.transform.position).normalized;
                    rb.AddForce(dir * attractionForce * Time.deltaTime, ForceMode2D.Force);
                }
            }
        }

        // Auto-click units every interval
        if (clickTimer >= autoClickInterval)
        {
            clickTimer = 0f;

            foreach (var unit in nearbyUnits)
            {
                if (unit.canEarn)
                {
                    // Give currency
                    GameManager.Instance.AddCurrency(unit.gainPerClick);

                    // Spawn floating text
                    if (unit.floatingTextPrefab != null)
                    {
                        GameObject textObj = Instantiate(unit.floatingTextPrefab,
                            unit.transform.position + Vector3.up * 1f, Quaternion.identity);
                        FloatingText ft = textObj.GetComponent<FloatingText>();
                        if (ft != null) ft.ShowText("+" + unit.gainPerClick);
                    }
                }
            }
        }
    }

    // Optional: visualize radius in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}

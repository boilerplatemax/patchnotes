using UnityEngine;

public class ClickManager : MonoBehaviour
{
    [Header("Click Settings")]
    public float clickRadius = 2f; // area of effect
    public LayerMask unitLayer; // optional, if you want to filter

    [Header("Audio Settings")]
    public AudioClip clickSound;   // assign in Inspector
    public AudioSource audioSource; // assign in Inspector (or auto-add below)

    [Header("Tutorial")]
    public GameObject tutorialText; // assign in Inspector

    private bool tutorialHidden = false;

    void Awake()
    {
        // Auto-setup if not set in Inspector
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // left click
        {
            Vector3 clickWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickWorldPos.z = 0f;

            Unit[] allUnits = GameObject.FindObjectsOfType<Unit>();
            bool clickedAny = false;

            foreach (var unit in allUnits)
            {
                if (!unit.canEarn) continue;

                if (Vector2.Distance(unit.transform.position, clickWorldPos) <= clickRadius)
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

                    clickedAny = true;
                }
            }

            // Play sound if at least one unit was clicked
            if (clickedAny)
            {
                if (clickSound != null)
                    audioSource.PlayOneShot(clickSound);

                // Hide tutorial text if it's still visible
                if (!tutorialHidden && tutorialText != null)
                {
                    tutorialText.SetActive(false);
                    tutorialHidden = true;
                }
            }
        }
    }
}

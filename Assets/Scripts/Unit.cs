using UnityEngine;
using TMPro; // If using TextMeshPro for floating text

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderForce = 0.01f;
    public float wanderIntervalMin = 1f;
    public float wanderIntervalMax = 2f;
    public float moveSpeed = 2f;

    [Header("Click Settings")]
    public int gainPerClick = 1;
    public GameObject floatingTextPrefab; // prefab for "+x" text

    [Header("Window Check")]
    public DraggableWindow window;

    [HideInInspector]
    public bool canEarn = true;

    private Rigidbody2D rb;
    private float nextWanderTime;

    [SerializeField]
    private float clickRadius = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ScheduleNextWander();

        // Grab the window from GameManager if not assigned
        if (window == null && GameManager.Instance != null)
        {
            window = GameManager.Instance.window;
        }
    }

    void Update()
    {
        Wander();
    }

    void Wander()
    {
        if (Time.time >= nextWanderTime)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            rb.AddForce(dir * wanderForce, ForceMode2D.Impulse);
            ScheduleNextWander();
        }
    }

    void ScheduleNextWander()
    {
        nextWanderTime = Time.time + Random.Range(wanderIntervalMin, wanderIntervalMax);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == window.gameObject)
        {
            canEarn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == window.gameObject)
        {
            canEarn = false;
        }
    }
}

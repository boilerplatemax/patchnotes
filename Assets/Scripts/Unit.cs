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

    [Header("Visual Settings")]
    public bool lookAtDirection = false; // rotate to face movement direction
    public bool flipOnX = false;         // flip sprite left/right
    private SpriteRenderer spriteRenderer;

    [Header("Click Settings")]
    public bool canBeClicked = true;
    public int gainPerClick = 1;
    public GameObject floatingTextPrefab; // prefab for "+x" text

    [Header("Passive Settings")]
    public bool canGenerate = false;
    public int gainPerCycle = 1;
    public float generateInterval = 2f;

    [Header("Spawn Settings")]
    public bool canSpawn = false;
    public GameObject spawnPrefab;
    public float spawnInterval = 5f;
    public Vector2 spawnOffset = Vector2.zero;

    [Header("Window Check")]
    public DraggableWindow window;

    [HideInInspector] public bool canEarn = true;

    private Rigidbody2D rb;
    private float nextWanderTime;
    private float nextGenerateTime;
    private float nextSpawnTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        ScheduleNextWander();

        if (window == null && GameManager.Instance != null)
            window = GameManager.Instance.window;

        nextGenerateTime = Time.time + generateInterval;
        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        Wander();
        HandlePassiveGeneration();
        HandleSpawning();
        HandleVisuals();
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

    void HandlePassiveGeneration()
    {
        if (!canGenerate || !canEarn) return;

        if (Time.time >= nextGenerateTime)
        {
            GameManager.Instance.AddCurrency(gainPerCycle);

            if (floatingTextPrefab != null)
            {
                GameObject textObj = Instantiate(floatingTextPrefab,
                    transform.position + Vector3.up * 1f, Quaternion.identity);
                FloatingText ft = textObj.GetComponent<FloatingText>();
                if (ft != null) ft.ShowText("+" + gainPerCycle);
            }

            nextGenerateTime = Time.time + generateInterval;
        }
    }

    void HandleSpawning()
    {
        if (!canSpawn || spawnPrefab == null || !canEarn) return;

        if (Time.time >= nextSpawnTime)
        {
            Vector3 spawnPos = transform.position + (Vector3)spawnOffset;
            Instantiate(spawnPrefab, spawnPos, Quaternion.identity);

            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void HandleVisuals()
    {
        Vector2 velocity = rb.velocity;

        // Rotate to look at movement direction
        if (lookAtDirection && velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Flip sprite left/right
        if (flipOnX && spriteRenderer != null)
        {
            if (velocity.x > 0.05f) spriteRenderer.flipX = false;
            else if (velocity.x < -0.05f) spriteRenderer.flipX = true;
        }
    }

    void ScheduleNextWander()
    {
        nextWanderTime = Time.time + Random.Range(wanderIntervalMin, wanderIntervalMax);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == window.gameObject)
            canEarn = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == window.gameObject)
            canEarn = false;
    }
}

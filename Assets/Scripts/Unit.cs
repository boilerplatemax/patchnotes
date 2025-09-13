using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderForce = 0.01f;
    public float wanderIntervalMin = 1f;
    public float wanderIntervalMax = 2f;
    public float bounceForce = 0.05f;
    public float moveSpeed = 2f;

    [Header("Work Settings")]
    public float workInterval = 1f;
    public int workAmount = 1;

    [Header("Window Check")]
    public DraggableWindow window;

    private Rigidbody2D rb;
    private float nextWanderTime;
    private Resource targetResource;
    private float workTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ScheduleNextWander();
    }

    void Update()
    {
        // Only units inside the window can work
        if (!IsUnitInsideWindow())
        {
            targetResource = null;
            Wander();
            return;
        }

        // Find the closest resource inside the window
        if (targetResource == null)
        {
            FindNearestResource();
        }

        if (targetResource != null)
        {
            MoveTowardsResource();

            // Check if unit is overlapping the resource
            if (IsOverlappingResource(targetResource))
            {
                DoWork();
            }
        }
        else
        {
            Wander();
        }
    }

    bool IsUnitInsideWindow()
    {
        foreach (Transform t in window.unitsInside)
            if (t == transform) return true;
        return false;
    }

    void FindNearestResource()
    {
        float closestDistance = Mathf.Infinity;
        Resource closest = null;
        Collider2D windowCollider = window.GetComponent<Collider2D>();
        if (windowCollider == null) return;

        Resource[] resources = GameObject.FindObjectsOfType<Resource>();
        foreach (var res in resources)
        {
            Collider2D resCol = res.GetComponent<Collider2D>();
            if (resCol == null) continue;

            // Must be inside window bounds
            if (!windowCollider.OverlapPoint(resCol.bounds.center)) continue;

            float dist = Vector2.Distance(transform.position, res.transform.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = res;
            }
        }

        targetResource = closest;
    }

    void MoveTowardsResource()
    {
        Vector2 dir = ((Vector2)targetResource.transform.position - rb.position).normalized;
        rb.velocity = dir * moveSpeed;
    }

    bool IsOverlappingResource(Resource res)
    {
        Collider2D resCol = res.GetComponent<Collider2D>();
        if (resCol == null) return false;

        // Check if any part of unit collider overlaps the resource
        Collider2D unitCol = GetComponent<Collider2D>();
        if (unitCol == null) return false;

        ContactFilter2D filter = new ContactFilter2D();
        Collider2D[] results = new Collider2D[1];
        return unitCol.OverlapCollider(filter, results) > 0 && results[0] == resCol;
    }

    void DoWork()
    {
        workTimer -= Time.deltaTime;
        if (workTimer <= 0f)
        {
            targetResource.TakeDamage(workAmount);
            workTimer = workInterval;
            rb.velocity = Vector2.zero; // stop moving while working
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("WindowBorder"))
        {
            Vector2 bounceDir = collision.contacts[0].normal;
            rb.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);
        }
    }
}

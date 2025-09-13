using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Unit : MonoBehaviour
{
    [Header("Wander Settings")]
    public float wanderForce = 1f;      // force applied for random wandering
    public float wanderIntervalMin = 1f; // min seconds between random forces
    public float wanderIntervalMax = 3f; // max seconds between random forces

    [Header("Bounce Settings")]
    public float bounceForce = 2f;

    private Rigidbody2D rb;
    private float nextWanderTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ScheduleNextWander();
    }

    void Update()
    {
        // Random wandering timer
        if (Time.time >= nextWanderTime)
        {
            ApplyRandomForce();
            ScheduleNextWander();
        }
    }

    void ApplyRandomForce()
    {
        //Pick a random way to go
        Vector2 dir = Random.insideUnitCircle.normalized;
        rb.AddForce(dir * wanderForce, ForceMode2D.Impulse);
    }

    void ScheduleNextWander()
    {
        nextWanderTime = Time.time + Random.Range(wanderIntervalMin, wanderIntervalMax);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if we hit the window borders
        if (collision.collider.CompareTag("WindowBorder"))
        {
            //Apply A bounce backwards
            Vector2 bounceDir = collision.contacts[0].normal;
            rb.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);
        }
    }
}

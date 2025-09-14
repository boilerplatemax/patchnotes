using UnityEngine;

public class DraggableWindow : MonoBehaviour
{
    [Header("Camera Follow")]
    public Camera mainCamera;
    public Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float cameraSmoothTime = 0.1f; // lower = snappier
    private Vector3 cameraVelocity = Vector3.zero;

    [Header("Movement SFX")]
    public AudioSource moveSFX;         // optional: for one-shot start sound
    public AudioSource loopSFX;         // optional: for continuous movement sound
    public bool useLoop = true;         // toggle looping sound on/off

    private bool isMoving = false;      // tracks if movement is happening

    [Header("Unit Indicator")]
    public GameObject indicatorPrefab;    // assign an arrow prefab here
    public float orbitRadius = 2f;        // distance from the window center
    [Tooltip("Degrees per second the indicator will move along the circle toward the target angle (not a continuous spin).")]
    public float orbitSpeed = 360f;       // degrees per second for sweeping motion
    [Tooltip("Add/subtract degrees if your sprite points differently (e.g. -90 if arrow art points up).")]
    public float indicatorRotationOffset = -90f;

    private GameObject indicatorInstance;
    private BoxCollider2D windowCollider;
    private float currentIndicatorAngle = 0f; // degrees, current angle around the window

    [Header("Window Drag")]
    public float keyboardSpeed = 5f; //speed of window movement via WASD

    [Header("Movement Bounds")]
    public float maxDistance = 20f;  //for X
    public float maxDistanceY = 20f; //for Y

    private Vector3 offset;
    private Vector3 targetWindowPos;
    private Vector3 lastWindowPos;

    private enum ControlMode { None, Mouse, Keyboard }
    private ControlMode activeMode = ControlMode.None;

    void Start()
    {
        targetWindowPos = transform.position;
        lastWindowPos = transform.position;

        if (mainCamera == null)
            mainCamera = Camera.main;

        windowCollider = GetComponent<BoxCollider2D>();

        // create indicator but hide it
        if (indicatorPrefab != null)
        {
            indicatorInstance = Instantiate(indicatorPrefab, transform.position, Quaternion.identity);
            indicatorInstance.SetActive(false);

            // initialize the indicator at some default angle (0°)
            currentIndicatorAngle = 0f;
            Vector3 initPos = transform.position + new Vector3(Mathf.Cos(0f), Mathf.Sin(0f), 0f) * orbitRadius;
            indicatorInstance.transform.position = initPos;
        }
    }

    void Update()
    {
        HandleControlInput();
        HandleIndicator();
    }

    void LateUpdate()
    {
        HandleCameraFollow();
    }

    void HandleControlInput()
    {
        // Keyboard input=true?
        bool keyboardPressed = Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.01f ||
                               Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;

        if (keyboardPressed && activeMode == ControlMode.None)
        {
            activeMode = ControlMode.Keyboard;
        }
        else if (!keyboardPressed && activeMode == ControlMode.Keyboard)
        {
            activeMode = ControlMode.None;
        }
        else if (activeMode == ControlMode.Keyboard)
        {
            HandleKeyboardInput();
        }

        PlayWindowSounds(keyboardPressed);
    }

    void PlayWindowSounds(bool keyboardPressed)
    {
         // Determine if we are currently moving
        bool currentlyMoving = activeMode == ControlMode.Mouse && Input.GetMouseButton(0)
                            || activeMode == ControlMode.Keyboard && keyboardPressed;

        if (currentlyMoving && !isMoving)
        {
            // Movement just started
            isMoving = true;

            if (useLoop && loopSFX != null && !loopSFX.isPlaying)
                loopSFX.Play();
        }
        else if (!currentlyMoving && isMoving)
        {
            // Movement just stopped
            isMoving = false;

            // Stop looping sound
            if (loopSFX != null && loopSFX.isPlaying)
                loopSFX.Stop();
        }
    }

    void HandleWindowDrag()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            targetWindowPos = mouseWorld + offset;

            Vector3 newPos = Vector3.Lerp(transform.position, targetWindowPos, keyboardSpeed * Time.deltaTime);
            newPos = ClampToMaxDistance(newPos);

            transform.position = newPos;
        }
    }

    void HandleKeyboardInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //Raw Input
        Vector3 input = new Vector3(h, v, 0f);

        //Prevent diagonal speed increase
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        Vector3 keyDelta = input * keyboardSpeed * Time.deltaTime;
        Vector3 newPos = transform.position + keyDelta;

        newPos = ClampToMaxDistance(newPos);

        transform.position = newPos;
    }

    // --- NEW: indicator logic that moves along circle to face nearest Unit ---
    void HandleIndicator()
    {
        if (indicatorInstance == null || windowCollider == null) return;

        // Find all Units
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

        // Check if any are inside the window collider
        bool hasUnitInside = false;
        foreach (var unit in units)
        {
            if (windowCollider.OverlapPoint(unit.transform.position))
            {
                hasUnitInside = true;
                break;
            }
        }

        if (hasUnitInside)
        {
            indicatorInstance.SetActive(false);
            return;
        }

        // Otherwise, find the nearest Unit
        GameObject nearest = null;
        float nearestDist = Mathf.Infinity;
        foreach (var unit in units)
        {
            float dist = Vector2.Distance(transform.position, unit.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = unit;
            }
        }

        if (nearest == null)
        {
            indicatorInstance.SetActive(false);
            return;
        }

        // Show indicator
        indicatorInstance.SetActive(true);

        // compute target angle (degrees) from window center to the unit
        Vector3 toUnit = nearest.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(toUnit.y, toUnit.x) * Mathf.Rad2Deg;

        // move the current indicator angle toward the targetAngle (shortest path)
        currentIndicatorAngle = Mathf.MoveTowardsAngle(currentIndicatorAngle, targetAngle, orbitSpeed * Time.deltaTime);

        // place indicator at orbit position for currentIndicatorAngle
        float rad = currentIndicatorAngle * Mathf.Deg2Rad;
        Vector3 orbitOffset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * orbitRadius;
        indicatorInstance.transform.position = transform.position + orbitOffset;

        // rotate indicator so it points toward the unit
        Vector3 dir = (nearest.transform.position - indicatorInstance.transform.position).normalized;
        float rotZ = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        indicatorInstance.transform.rotation = Quaternion.Euler(0f, 0f, rotZ + indicatorRotationOffset);
    }

    void HandleCameraFollow()
    {
        if (mainCamera == null) return;

        Vector3 targetCamPos = transform.position + cameraOffset;
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetCamPos, ref cameraVelocity, cameraSmoothTime);
    }

    public Vector3 ClampToMaxDistance(Vector3 pos)
    {
        float halfWidth = transform.localScale.x / 2f;
        float halfHeight = transform.localScale.y / 2f;

        float maxX = maxDistance - halfWidth;
        float maxY = maxDistanceY - halfHeight;

        pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
        pos.y = Mathf.Clamp(pos.y, -maxY, maxY);

        return pos;
    }

}

using UnityEngine;

public class DraggableWindow : MonoBehaviour
{
    [Header("Units Inside Window")]
    public Transform[] unitsInside;
    [Range(0f, 1f)]
    public float unitFollowSpeed = 1f; // unit speed within movement

    [Header("Camera Follow")]
    public Camera mainCamera;
    public Vector3 cameraOffset = new Vector3(0, 0, -10);
    public float cameraSmoothTime = 0.1f; // lower = snappier
    private Vector3 cameraVelocity = Vector3.zero;

    [Header("Window Drag")]
    [Range(1f, 50f)]
    public float windowSmoothSpeed = 20f; //higher = snappier window drag
    [Range(0.1f, 5f)]
    public float dragMultiplier = 1f; //multiplies how fast window follows mouse
    public float keyboardSpeed = 5f;   //speed of window movement via WASD


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
    }

    void Update()
    {
        HandleControlInput();
        MoveUnits();
    }

    void LateUpdate()
    {
        HandleCameraFollow();
    }

    void HandleControlInput()
    {
        // Check mouse start
        if (Input.GetMouseButtonDown(0))
        {
            activeMode = ControlMode.Mouse;
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            offset = transform.position - mouseWorld;
        }

        // Release mouse
        if (Input.GetMouseButtonUp(0) && activeMode == ControlMode.Mouse)
        {
            activeMode = ControlMode.None;
        }

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

       //Make units run!
        if (activeMode == ControlMode.Mouse)
        {
            HandleWindowDrag();
        }
        else if (activeMode == ControlMode.Keyboard)
        {
            HandleKeyboardInput();
        }
    }

    void HandleWindowDrag()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            targetWindowPos = mouseWorld + offset;

            Vector3 newPos = Vector3.Lerp(transform.position, targetWindowPos, windowSmoothSpeed * dragMultiplier * Time.deltaTime);
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


    void MoveUnits()
    {
        Vector3 delta = transform.position - lastWindowPos;

        foreach (Transform unit in unitsInside)
        {
            unit.position += delta * unitFollowSpeed;

            if (Mathf.Abs(delta.x) > 0.01f)
            {
                SpriteRenderer sr = unit.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.flipX = delta.x < 0;
                }
            }
        }

        lastWindowPos = transform.position;
    }

    void HandleCameraFollow()
    {
        if (mainCamera == null) return;

        Vector3 targetCamPos = transform.position + cameraOffset;
        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetCamPos, ref cameraVelocity, cameraSmoothTime);
    }

Vector3 ClampToMaxDistance(Vector3 pos)
{
    //Half the width of the curr window screen
    float halfWidth = transform.localScale.x / 2f;
    float halfHeight = transform.localScale.y / 2f;
    //Need 2 clamp distances incase the window isn't square
    float maxX = maxDistance - halfWidth;
    float maxY = maxDistanceY - halfHeight;

    pos.x = Mathf.Clamp(pos.x, -maxX, maxX);
    pos.y = Mathf.Clamp(pos.y, -maxY, maxY);

    return pos;
}


}

using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1f;
    public float offsetMin = -0.5f; // min random offset
    public float offsetMax = 0.5f;  // max random offset

    private TextMeshPro text;

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    public void ShowText(string message)
    {
        text.text = message;

        // Apply random offset
        float offsetX = Random.Range(offsetMin, offsetMax);
        float offsetY = Random.Range(offsetMin, offsetMax);
        transform.position += new Vector3(offsetX, offsetY, 0f);

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }
}

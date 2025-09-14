using UnityEngine;
using TMPro;
using System.Collections;

public class HackManager : MonoBehaviour
{
    public static HackManager Instance;

    [Header("Hack Settings")]
    public int totalCurrencyToWin = 10000;
    public float hackDurationSeconds = 300f; // 5 minutes by default

    [Header("UI")]
    public TMP_Text timerText;       // assign in inspector
    public TMP_Text hackProgressText; // assign in inspector

    private float timeRemaining;
    private bool timerRunning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        StartHackTimer();
    }

    void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;
            GameOver();
        }

        UpdateTimerUI();
        UpdateHackProgressUI();
    }

    // Timer Control

    public void StartHackTimer()
    {
        timeRemaining = hackDurationSeconds;
        timerRunning = true;
    }

    public void StopHackTimer()
    {
        timerRunning = false;
    }
    // UI Updates

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void UpdateHackProgressUI()
    {
        float currency = GameManager.Instance.totalCurrencyEarned; // use total earned
        float percent = CalculateHackPercent(currency);
        hackProgressText.text = $"Hack Progress: {percent:0}%";
    }


    // Non-linear hack % calculation
    private float CalculateHackPercent(float currentCurrency)
    {
        float fraction = Mathf.Clamp01(currentCurrency / totalCurrencyToWin);

        if (fraction <= 0.3f)
            return Mathf.Lerp(0f, 50f, fraction / 0.3f);
        else if (fraction <= 0.6f)
            return Mathf.Lerp(50f, 60f, (fraction - 0.3f) / 0.3f);
        else
            return Mathf.Lerp(60f, 100f, (fraction - 0.6f) / 0.4f);
    }
    // Game Over

    private void GameOver()
    {
        Debug.Log("Time's up! Game Over!");
        // TODO: Add game over UI or logic
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PomodoroTimer : MonoBehaviour
{
    [Header("Settings")]
    public float workDuration = 25 * 60; // 25分钟转换为秒
    public float restDuration = 5 * 60;  // 5分钟转换为秒

    [Header("UI References")]
    public Text timerText;
    public Text statusText;
    public Button startButton;
    public Button resetButton;

    private float currentTime;
    private bool isWorkingPhase = true;
    private bool isRunning = false;

    void Start()
    {
        startButton.onClick.AddListener(ToggleTimer);
        resetButton.onClick.AddListener(ResetTimer);
        ResetTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                SwitchPhase();
            }
        }
    }

    void ToggleTimer()
    {
        isRunning = !isRunning;
        startButton.GetComponentInChildren<Text>().text = isRunning ? "Pause" : "Start";
    }

    void ResetTimer()
    {
        isRunning = false;
        isWorkingPhase = true;
        currentTime = workDuration;
        UpdateTimerDisplay();
        startButton.GetComponentInChildren<Text>().text = "Start";
        statusText.text = "Work Time!";
    }

    void SwitchPhase()
    {
        isWorkingPhase = !isWorkingPhase;
        currentTime = isWorkingPhase ? workDuration : restDuration;
        statusText.text = isWorkingPhase ? "Work Time!" : "Rest Time!";
        // 可选：播放提示音效
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
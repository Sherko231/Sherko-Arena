using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [SerializeField, SuffixLabel("Min")] private float countdownTime = 5f;
    private float _countdownTimeSec;
    
    private TMP_Text _tmp;
    private float _remainingTime;
    private bool _isRunning;
    private bool _isPaused;

    public event Action OnTimerFinish;

    private void Awake()
    {
        _tmp = GetComponent<TMP_Text>();
        _countdownTimeSec = countdownTime * 60f;
    }

    private void Update()
    {
        if (_isRunning && !_isPaused) RunTimer();
    }
    
    private void RunTimer()
    {
        _remainingTime -= Time.deltaTime;
        if (_remainingTime <= 0)
        {
            _remainingTime = 0;
            _isRunning = false;
            OnTimerFinish?.Invoke();
        }
        UpdateTimerText();
    }

    [HorizontalGroup("Buttons1"), Button(ButtonSizes.Small), GUIColor(0.41f, 1f, 0.31f, 0.45f)]
    public void StartTimer()
    {
        _remainingTime = _countdownTimeSec;
        _isRunning = true;
        _isPaused = false;
        UpdateTimerText();
    }

    [HorizontalGroup("Buttons1"), Button(ButtonSizes.Small), GUIColor(1f, 0.42f, 0.44f, 0.45f)]
    public void StopTimer()
    {
        _isRunning = false;
        _isPaused = false;
    }
    
    [HorizontalGroup("Buttons2"), Button(ButtonSizes.Small), GUIColor(0.63f, 0.67f, 1f, 0.45f)]
    public void PauseTimer()
    {
        if (_isRunning && !_isPaused)
        {
            _isPaused = true;
        }
    }
    
    [HorizontalGroup("Buttons2"), Button(ButtonSizes.Small), GUIColor(0.65f, 0.96f, 1f, 0.45f)]
    public void UnPauseTimer()
    {
        if (_isRunning && _isPaused)
        {
            _isPaused = false;
        }
    }

    [HorizontalGroup("Buttons3"), Button(ButtonSizes.Small)]
    public void ResetTimer()
    {
        _remainingTime = _countdownTimeSec;
        _isRunning = true;
        _isPaused = false;
        UpdateTimerText();
    }
    
    private void UpdateTimerText()
    {
        string minutes = ((int)_remainingTime / 60).ToString();
        string seconds = ((int)_remainingTime % 60).ToString("D2");
        _tmp.text = minutes + ":" + seconds;
    }
    
}

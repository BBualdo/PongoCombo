using System;
using UnityEngine;

public class GameTimeManager : MonoBehaviour {
    public static GameTimeManager Instance { get; private set; }
    
    public event EventHandler<OnCountdownChangedEventArgs> OnCountdownChanged;
    public class OnCountdownChangedEventArgs {
        public float countdownTimer;
    }

    public event EventHandler OnCountdownEnd;
    
    public event EventHandler<OnGameTimeUpdateEventArgs> OnGameTimeUpdate;
    public class OnGameTimeUpdateEventArgs {
        public float gameTime;
    }

    public event EventHandler OnGameTimePhaseIncreased;
    
    public event EventHandler OnGamePaused;
    
    [Header("GameSettings")] 
    private float countdownTimer = 3f;
    private float countdownTimerMax = 3f;
    private float gameTimer;
    private float additionalEffectsTimerThreshhold = 60f;
    private int gameTimePhase;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        if (SFXManager.Instance != null) {
            SFXManager.Instance.RegisterGameTimeEvents(this);    
        }
    }

    private void OnDestroy() {
        if (SFXManager.Instance != null) {
            SFXManager.Instance.UnregisterGameTimeEvents(this);    
        }
    }
    
    public void ResetGameTimer() {
        gameTimer = 0f;
        OnGameTimeUpdate?.Invoke(this, new OnGameTimeUpdateEventArgs {
            gameTime = gameTimer
        });
    }
    
    public void TogglePause() {
        OnGamePaused?.Invoke(this, EventArgs.Empty);
        
        if (Time.timeScale != 0) {
            Time.timeScale = 0;
        } else {
            Time.timeScale = 1;
        }
    }

    public void HandleCountdownTimer() {
        countdownTimer -= Time.deltaTime;
        OnCountdownChanged?.Invoke(this, new OnCountdownChangedEventArgs {
            countdownTimer = countdownTimer
        });
        
        if (countdownTimer <= 0) {
            countdownTimer = countdownTimerMax;
            OnCountdownEnd?.Invoke(this, EventArgs.Empty);
            gameTimePhase = 1;
        }
    }

    public void HandleGameTimer() {
        gameTimer += Time.deltaTime;
        OnGameTimeUpdate?.Invoke(this, new OnGameTimeUpdateEventArgs {
            gameTime = gameTimer
        });
                
        if (gameTimer >= additionalEffectsTimerThreshhold * gameTimePhase) {
            gameTimePhase++;
            OnGameTimePhaseIncreased?.Invoke(this, EventArgs.Empty);
        }
    }
}
using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    
    private int gameMode;
    
    public enum State {
        GameCountdown,
        GamePlaying,
        GameOver
    }
    
    public event EventHandler OnGameReset;

    public event EventHandler<OnGameOverEventArgs> OnGameOver;
    public class OnGameOverEventArgs {
        public Player winner;
    }

    public event EventHandler<OnCountdownChangedEventArgs> OnCountdownChanged;
    public class OnCountdownChangedEventArgs {
        public float countdownTimer;
    }
    
    public event EventHandler<OnGameTimeUpdateEventArgs> OnGameTimeUpdate;
    public class OnGameTimeUpdateEventArgs {
        public float gameTime;
    }
    public event EventHandler OnGamePaused;

    [Header("GameSettings")] 
    private float countdownTimer = 3f;
    private float countdownTimerMax = 3f;
    private float gameTimer;
    private float additionalEffectsTimerThreshhold = 60f;
    
    private State state;
    private int gamePhase;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
        }

        gameMode = PlayerPrefs.GetInt(PlayerPrefsHelper.GAME_MODE);
    }

    private void Start() {
        if (SFXManager.Instance != null) {
            SFXManager.Instance.RegisterGameplayEvents(this);    
        }
        
        PlayerManager.Instance.OnBallScored += PlayerManager_OnBallScored;
        
        state = State.GameCountdown;

        ResetGameTimer();
    }

    private void PlayerManager_OnBallScored(object sender, PlayerManager.OnBallScoredEventArgs e) {
        if (e.lostPlayer.IsDead()) {
            OnGameOver?.Invoke(this, new OnGameOverEventArgs {
                winner = e.scoredPlayer
            });

            state = State.GameOver;

            return;
        }

        BallManager.Instance.CreateBall(e.lostPlayer.GetPlayerBallHoldPoint());
        BallManager.Instance.GetCurrentBall().StopMoving();
    }

    private void Update() {
        switch (state) {
            case State.GameCountdown:
                countdownTimer -= Time.deltaTime;
                OnCountdownChanged?.Invoke(this, new OnCountdownChangedEventArgs {
                    countdownTimer = countdownTimer
                });
                if (countdownTimer <= 0) {
                    countdownTimer = countdownTimerMax;
                    state = State.GamePlaying;
                    gamePhase = 1;
                    BallManager.Instance.CreateBall();
                }
                break;
            case State.GamePlaying:
                gameTimer += Time.deltaTime;
                OnGameTimeUpdate?.Invoke(this, new OnGameTimeUpdateEventArgs {
                    gameTime = gameTimer
                });
                
                if (gameTimer >= additionalEffectsTimerThreshhold * gamePhase) {
                    gamePhase++;
                    PlayerManager.Instance.ShrinkPlayers();
                    BallManager.Instance.DoubleBallDamage();
                }
                break;
            case State.GameOver:
                break;
        }

        HandlePause();
    }

    private void HandlePause() {
        if (state != State.GameOver && Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }
    
    private void OnDestroy() {
        PlayerManager.Instance.OnBallScored -= PlayerManager_OnBallScored;
        if (SFXManager.Instance != null) {
            SFXManager.Instance.UnregisterGameplayEvents(this);
        }
    }

    public void RestartGame() {
        PlayerManager.Instance.GetPlayer(PlayerManager.PlayerSide.PlayerL).Reset();
        PlayerManager.Instance.GetPlayer(PlayerManager.PlayerSide.PlayerR).Reset();
        ResetGameTimer();
        
        OnGameReset?.Invoke(this, EventArgs.Empty);

        state = State.GameCountdown;
    }

    public State GetState() {
        return state;
    }

    public int GetGameMode() {
        return gameMode;
    }

    private void ResetGameTimer() {
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
}

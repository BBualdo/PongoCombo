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
    
    public Action<Ball> OnBallSpawned;
    public event EventHandler OnGameReset;

    public event EventHandler<OnGameOverEventArgs> OnGameOver;
    public class OnGameOverEventArgs {
        public Player winner;
    }

    public event EventHandler<OnCountdownChangedEventArgs> OnCountdownChanged;
    public class OnCountdownChangedEventArgs {
        public float countdownTimer;
    }
    public event EventHandler OnBallScored;
    public event EventHandler<OnGameTimeUpdateEventArgs> OnGameTimeUpdate;
    public class OnGameTimeUpdateEventArgs {
        public float gameTime;
    }
    public event EventHandler OnGamePaused;
    public event EventHandler OnPlayersShrink;
    
    [Header("Players")]
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    private Player playerScored;
    private Player playerLost;

    [Header("Ball")]
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Ball ballPrefab;

    [Header("GameSettings")] 
    [SerializeField] private int ballDamageIncreaseValue = 1;
    private float countdownTimer = 3f;
    private float countdownTimerMax = 3f;
    private float gameTimer;
    private float additionalEffectsTimerThreshhold = 60f;
    
    private State state;
    private Ball currentBall;
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
        
        state = State.GameCountdown;

        ResetGameTimer();
        
        GoalLine.OnGoalScored += GoalLine_OnGoalScored;
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
                    CreateBall(ballSpawnPoint);
                }
                break;
            case State.GamePlaying:
                gameTimer += Time.deltaTime;
                OnGameTimeUpdate?.Invoke(this, new OnGameTimeUpdateEventArgs {
                    gameTime = gameTimer
                });
                
                if (gameTimer >= additionalEffectsTimerThreshhold * gamePhase) {
                    gamePhase++;
                    ShrinkPlayers();
                    DoubleBallDamage();
                }
                break;
            case State.GameOver:
                break;
        }

        HandlePause();
    }

    private void DoubleBallDamage() {
        ballDamageIncreaseValue *= 2;
    }

    private void HandlePause() {
        if (state != State.GameOver && Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    private void ShrinkPlayers() {
        float minPlayerHeight = .5f;
        if (leftPlayer.GetPlayerHeight() > minPlayerHeight) {
            leftPlayer.SetPlayerHeight(leftPlayer.GetPlayerHeight() - .5f);
            OnPlayersShrink?.Invoke(this, EventArgs.Empty);
        }
        
        if (rightPlayer.GetPlayerHeight() > minPlayerHeight) {
            rightPlayer.SetPlayerHeight(rightPlayer.GetPlayerHeight() - .5f);
        }
    }

    private void OnDestroy() {
        GoalLine.OnGoalScored -= GoalLine_OnGoalScored;
        if (SFXManager.Instance != null) {
            SFXManager.Instance.UnregisterGameplayEvents(this);
        }
    }

    private void GoalLine_OnGoalScored(object sender, GoalLine.OnGoalScoredEventArgs e) {
        if (e.goalSide == GoalLine.GoalSide.Left) {
            playerScored = rightPlayer;
            playerLost = leftPlayer;
        } else {
            playerScored = leftPlayer;
            playerLost = rightPlayer;
        }
        
        playerLost.TakeDamage(currentBall.GetBallDamage());
        OnBallScored?.Invoke(this, EventArgs.Empty);

        if (IsGameOver()) {
            OnGameOver?.Invoke(this, new OnGameOverEventArgs {
                winner = playerScored
            });

            state = State.GameOver;

            return;
        }

        CreateBall(playerLost.GetPlayerBallHoldPoint());
        currentBall.StopMoving();
    }

    private void CreateBall(Transform spawnPoint) {
        DestroyCurrentBall();
        currentBall = Instantiate(ballPrefab, spawnPoint);
        OnBallSpawned?.Invoke(currentBall);
    }

    private void DestroyCurrentBall() {
        if (currentBall == null) return;
        Destroy(currentBall.gameObject);
    }

    public Ball GetCurrentBall() {
        return currentBall;
    }

    private bool IsGameOver() {
        if (playerLost != null) {
            return playerLost.IsDead();
        }

        return false;
    }

    public void RestartGame() {
        leftPlayer.Reset();
        rightPlayer.Reset();
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

    public float GetBallDamageIncreaseValue() {
        return ballDamageIncreaseValue;
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

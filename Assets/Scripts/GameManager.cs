using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
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
        
    
    [Header("Players")]
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    private Player playerScored;
    private Player playerLost;

    [Header("Ball")]
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Ball ballPrefab;

    [Header("GameSettings")] 
    private float countdownTimer = 3f;
    private float countdownTimerMax = 3f;
    
    public static GameManager Instance { get; private set; }

    private State state;

    private Ball currentBall;

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
        state = State.GameCountdown;
        
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
                    CreateBall(ballSpawnPoint);
                }
                break;
            case State.GamePlaying:
                break;
            case State.GameOver:
                break;
        }
    }

    private void OnDestroy() {
        GoalLine.OnGoalScored -= GoalLine_OnGoalScored;
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
        
        OnGameReset?.Invoke(this, EventArgs.Empty);

        state = State.GameCountdown;
    }

    public State GetState() {
        return state;
    }

    public int GetGameMode() {
        return gameMode;
    }
}

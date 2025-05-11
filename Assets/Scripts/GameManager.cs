using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private int gameMode;
    
    public enum State {
        GamePlaying,
        GameOver
    }
    
    public Action<Ball> OnBallSpawned;
    public EventHandler OnGameReset;

    public EventHandler<OnGameOverEventArgs> OnGameOver;
    public class OnGameOverEventArgs {
        public Player winner;
    }
    
    [Header("Players")]
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    private Player playerScored;
    private Player playerLost;

    [Header("Ball")]
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Ball ballPrefab;
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
        state = State.GamePlaying;
        
        CreateBall(ballSpawnPoint);
        
        GoalLine.OnGoalScored += GoalLine_OnGoalScored;
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

        state = State.GamePlaying;
        
        CreateBall(ballSpawnPoint);
    }

    public State GetState() {
        return state;
    }

    public int GetGameMode() {
        return gameMode;
    }
}

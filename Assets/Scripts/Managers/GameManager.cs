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
    
    private State state;

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
        PlayerManager.Instance.OnBallScored += PlayerManager_OnBallScored;
        GameTimeManager.Instance.OnCountdownEnd += GameTimeManager_OnCountdownEnd;
        GameTimeManager.Instance.OnGameTimePhaseIncreased += GameTimeManager_OnGameTimePhaseIncreased;
        
        state = State.GameCountdown;

        GameTimeManager.Instance.ResetGameTimer();
    }

    private void OnDestroy() {
        PlayerManager.Instance.OnBallScored -= PlayerManager_OnBallScored;
        GameTimeManager.Instance.OnCountdownEnd -= GameTimeManager_OnCountdownEnd;
        GameTimeManager.Instance.OnGameTimePhaseIncreased -= GameTimeManager_OnGameTimePhaseIncreased;
    }
    
    private void Update() {
        switch (state) {
            case State.GameCountdown:
                GameTimeManager.Instance.HandleCountdownTimer();
                break;
            case State.GamePlaying:
                GameTimeManager.Instance.HandleGameTimer();
                break;
            case State.GameOver:
                break;
        }

        HandlePauseInput();
    }
    
    private void GameTimeManager_OnGameTimePhaseIncreased(object sender, EventArgs e) {
        PlayerManager.Instance.ShrinkPlayers();
        BallManager.Instance.DoubleBallDamage();
    }
    
    private void GameTimeManager_OnCountdownEnd(object sender, EventArgs e) {
        state = State.GamePlaying;
        BallManager.Instance.CreateBall();
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
    
    private void HandlePauseInput() {
        if (state != State.GameOver && Input.GetKeyDown(KeyCode.Escape)) {
            GameTimeManager.Instance.TogglePause();
        }
    }
    
    public void RestartGame() {
        PlayerManager.Instance.GetPlayer(PlayerManager.PlayerSide.PlayerL).Reset();
        PlayerManager.Instance.GetPlayer(PlayerManager.PlayerSide.PlayerR).Reset();
        GameTimeManager.Instance.ResetGameTimer();
        
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
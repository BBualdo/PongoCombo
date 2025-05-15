using System;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public enum PlayerSide {
        PlayerL,
        PlayerR
    }
    
    public static PlayerManager Instance { get; private set; }
    
    public event EventHandler OnPlayersShrink;
    
    public event EventHandler<OnBallScoredEventArgs> OnBallScored;
    public class OnBallScoredEventArgs : EventArgs {
        public Player scoredPlayer;
        public Player lostPlayer;
    }
    
    [Header("Players")]
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    private Player playerScored;
    private Player playerLost;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        if (SFXManager.Instance != null) {
            SFXManager.Instance.RegisterPlayerEvents(this);
        }
        
        GoalLine.OnGoalScored += GoalLine_OnGoalScored;
    }

    private void OnDestroy() {
        if (SFXManager.Instance != null) {
            SFXManager.Instance.UnregisterPlayerEvents(this);
        }
        
        GoalLine.OnGoalScored -= GoalLine_OnGoalScored;
    }

    public void ShrinkPlayers() {
        float minPlayerHeight = .5f;
        if (leftPlayer.PlayerHeight > minPlayerHeight) {
            leftPlayer.SetPlayerHeight(leftPlayer.PlayerHeight - .5f);
            OnPlayersShrink?.Invoke(this, EventArgs.Empty);
        }
        
        if (rightPlayer.PlayerHeight > minPlayerHeight) {
            rightPlayer.SetPlayerHeight(rightPlayer.PlayerHeight - .5f);
        }
    }

    public Player GetPlayer(PlayerSide playerSide) {
        return playerSide == PlayerSide.PlayerL ? leftPlayer : rightPlayer;
    }
    
    private void GoalLine_OnGoalScored(object sender, GoalLine.OnGoalScoredEventArgs e) {
        if (e.goalSide == GoalLine.GoalSide.Left) {
            playerScored = rightPlayer;
            playerLost = leftPlayer;
        } else {
            playerScored = leftPlayer;
            playerLost = rightPlayer;
        }
        
        playerLost.TakeDamage(BallManager.Instance.GetCurrentBall().CurrentDamage);
        OnBallScored?.Invoke(this, new OnBallScoredEventArgs {
            scoredPlayer = playerScored,
            lostPlayer = playerLost
        });
    }
}
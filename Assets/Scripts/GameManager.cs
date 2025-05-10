using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public Action<Ball> OnBallSpawned;
    public Action OnBallDestroyed;
    
    [Header("Players")]
    [SerializeField] private Player leftPlayer;
    [SerializeField] private Player rightPlayer;
    private Player playerScored;
    private Player playerLost;

    [Header("Ball")]
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Ball ballPrefab;
    public static GameManager Instance { get; private set; }

    private Ball currentBall;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        CreateBall(ballSpawnPoint);

        GoalLine.OnGoalScored += GoalLine_OnGoalScored;
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
        OnBallDestroyed?.Invoke();
    }

    public Ball GetCurrentBall() {
        return currentBall;
    }
}

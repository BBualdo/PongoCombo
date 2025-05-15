using System;
using UnityEngine;

public class BallManager : MonoBehaviour {
    public static BallManager Instance { get; private set; }
    
    public event EventHandler<OnBallSpawnedEventArgs> OnBallSpawned;
    public class OnBallSpawnedEventArgs : EventArgs {
        public Ball ball;
    }
    
    [Header("Ball")]
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private int ballDamageIncreaseValue = 1;
    private Ball currentBall;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start() {
        if (SFXManager.Instance != null) {
            SFXManager.Instance.RegisterBallEvents(this);
        }
    }

    private void OnDestroy() {
        if (SFXManager.Instance != null) {
            SFXManager.Instance.UnregisterBallEvents(this);
        }
    }
    
    public void CreateBall(Transform spawnPoint = null) {
        if (spawnPoint == null) 
            spawnPoint = ballSpawnPoint;
        
        DestroyCurrentBall();
        currentBall = Instantiate(ballPrefab, spawnPoint);
        OnBallSpawned?.Invoke(this, new OnBallSpawnedEventArgs {
            ball = currentBall
        });
    }

    private void DestroyCurrentBall() {
        if (currentBall == null) return;
        Destroy(currentBall.gameObject);
    }

    public Ball GetCurrentBall() {
        return currentBall;
    }
    
    public float GetBallDamageIncreaseValue() {
        return ballDamageIncreaseValue;
    }
    
    public void DoubleBallDamage() {
        ballDamageIncreaseValue *= 2;
    }
}
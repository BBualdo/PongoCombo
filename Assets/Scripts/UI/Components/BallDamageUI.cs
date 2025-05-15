using System;
using TMPro;
using UnityEngine;

public class BallDamageUI : MonoBehaviour {
    [SerializeField] private TMP_Text damageValueText;
    private Ball ball;

    private void Start() {
        BallManager.Instance.OnBallSpawned += BallManager_OnBallSpawned;
        GameManager.Instance.OnGameReset += GameManager_OnGameReset;
        
        UpdateVisual();
        ball = BallManager.Instance.GetCurrentBall();
        if (ball != null) {
            BallManager_OnBallSpawned(this, new BallManager.OnBallSpawnedEventArgs {
                ball = ball,
            });
        }
    }

    private void OnDestroy() {
        BallManager.Instance.OnBallSpawned -= BallManager_OnBallSpawned;
        GameManager.Instance.OnGameReset -= GameManager_OnGameReset;
    }

    private void GameManager_OnGameReset(object sender, EventArgs e) {
        //TODO: Fix setting ball damage to "-" during countdown
        UpdateVisual();
    }
    
    private void BallManager_OnBallSpawned(object sender, BallManager.OnBallSpawnedEventArgs e) {
        ball = e.ball;
        UpdateVisual();
        ball.OnDamageIncreased += Ball_OnDamageIncreased;
    }

    private void Ball_OnDamageIncreased(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        if (ball != null) {
            damageValueText.text = ball.CurrentDamage.ToString();
        } else {
            damageValueText.text = "-";
        }
    }
}

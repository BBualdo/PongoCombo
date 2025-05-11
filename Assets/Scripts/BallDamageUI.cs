using System;
using TMPro;
using UnityEngine;

public class BallDamageUI : MonoBehaviour {
    [SerializeField] private TMP_Text damageValueText;
    private Ball ball;

    private void Start() {
        GameManager.Instance.OnBallSpawned += GameManager_OnBallSpawned;
        
        UpdateVisual();
        ball = GameManager.Instance.GetCurrentBall();
        if (ball != null) {
            GameManager_OnBallSpawned(ball);
        }
    }

    private void OnDestroy() {
        GameManager.Instance.OnBallSpawned -= GameManager_OnBallSpawned;
    }

    private void GameManager_OnBallSpawned(Ball newBall) {
        ball = newBall;
        UpdateVisual();
        ball.OnDamageIncreased += Ball_OnDamageIncreased;
    }

    private void Ball_OnDamageIncreased(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        if (ball != null) {
            damageValueText.text = ball.GetBallDamage().ToString();
        }
    }
}

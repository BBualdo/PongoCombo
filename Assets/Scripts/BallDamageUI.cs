using System;
using TMPro;
using UnityEngine;

public class BallDamageUI : MonoBehaviour {
    [SerializeField] private TMP_Text damageValueText;
    private Ball ball;

    private void Start() {
        GameManager.Instance.OnBallSpawned += GameManager_OnBallSpawned;
        GameManager.Instance.OnBallDestroyed += GameManager_OnBallDestroyed;
    }

    private void GameManager_OnBallDestroyed() {
        ball.OnDamageIncreased -= Ball_OnDamageIncreased;
        ball = null;
    }

    private void GameManager_OnBallSpawned(Ball ball) {
        this.ball = ball;
        damageValueText.text = ball.GetBallDamage().ToString();
        ball.OnDamageIncreased += Ball_OnDamageIncreased;
    }

    private void Ball_OnDamageIncreased(object sender, EventArgs e) {
        damageValueText.text = ball.GetBallDamage().ToString();
    }
}

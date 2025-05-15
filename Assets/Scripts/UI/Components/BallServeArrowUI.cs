using System;
using UnityEngine;

public class BallServeArrowUI : MonoBehaviour {
    [SerializeField] private Transform arrowTransform;
    [SerializeField] private Player player;

    private void Start() {
        Hide();
        player.OnServeDirectionChanged += Player_OnServeDirectionChanged;
        player.OnBallGrabbed += Player_OnBallGrabbed;
        player.OnBallServed += Player_OnBallServed;
    }

    private void OnDestroy() {
        Hide();
        player.OnServeDirectionChanged -= Player_OnServeDirectionChanged;
        player.OnBallGrabbed -= Player_OnBallGrabbed;
        player.OnBallServed -= Player_OnBallServed;
    }

    private void Player_OnBallServed(object sender, EventArgs e) {
        Hide();
    }

    private void Player_OnBallGrabbed(object sender, EventArgs e) {
        Show();
    }
    
    private void Player_OnServeDirectionChanged(object sender, Player.OnServeDirectionChangedEventArgs e) {
        Vector2 dirNormalized = e.serveDirection.normalized;
        
        // Rotation
        float angle = Mathf.Atan2(dirNormalized.y, dirNormalized.x) * Mathf.Rad2Deg;
        if (player.PlayerSide == PlayerManager.PlayerSide.PlayerR) {
            angle += 180f;
        }
        arrowTransform.localRotation = Quaternion.Euler(0, 0, angle);
        
        // Position
        float radius = 1f;
        Vector2 offset = new Vector2(dirNormalized.x, dirNormalized.y).normalized * radius;
        arrowTransform.localPosition = offset;
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

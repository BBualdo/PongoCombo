using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour {
    [SerializeField] private Image barImage;
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text healthText;

    private void Start() {
        UpdateVisual();
        player.OnDamageTaken += Player_OnDamageTaken;
        GameManager.Instance.OnGameReset += GameManager_OnGameReset;
    }

    private void OnDestroy() {
        GameManager.Instance.OnGameReset -= GameManager_OnGameReset;
    }

    private void GameManager_OnGameReset(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void Player_OnDamageTaken(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        barImage.fillAmount = player.GetHealthPercent() / 100f;
        healthText.text = $"{player.GetRemainingHealth()}/{player.GetMaxHealth()}";
    }
}

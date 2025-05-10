using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour {
    [SerializeField] private Image barImage;
    [SerializeField] private Player player;
    [SerializeField] private TMP_Text healthText;

    private void Start() {
        barImage.fillAmount = player.GetHealthPercent() / 100f;
        player.OnDamageTaken += Player_OnDamageTaken;
    }

    private void Player_OnDamageTaken(object sender, EventArgs e) {
        barImage.fillAmount = player.GetHealthPercent() / 100f;
        healthText.text = $"{player.GetRemainingHealth()}/{player.GetMaxHealth()}";
    }
}

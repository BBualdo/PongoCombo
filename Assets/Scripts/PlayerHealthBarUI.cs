using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour {
    [SerializeField] private Image barImage;
    [SerializeField] private Player player;

    private void Start() {
        barImage.fillAmount = player.GetHealthPercent() / 100f;
        player.OnDamageTaken += Player_OnDamageTaken;
    }

    private void Player_OnDamageTaken(object sender, EventArgs e) {
        barImage.fillAmount = player.GetHealthPercent() / 100f;
    }
}

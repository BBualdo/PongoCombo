using System;
using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour {
    [SerializeField] private TMP_Text timerText;
    
    private void Start() {
        GameManager.Instance.OnGameTimeUpdate += GameManager_OnGameTimeUpdate;
        
        UpdateVisual();
    }

    private void GameManager_OnGameTimeUpdate(object sender, GameManager.OnGameTimeUpdateEventArgs e) {
        UpdateVisual(e.gameTime);
    }

    private void UpdateVisual(float gameTime = 0) {
        timerText.text = FormatTime(Mathf.FloorToInt(gameTime));
    }
    private string FormatTime(int timeInSeconds) {
        return TimeSpan.FromSeconds(timeInSeconds).ToString(@"mm\:ss");
    }
}
using System;
using TMPro;
using UnityEngine;

public class GameTimerUI : MonoBehaviour {
    [SerializeField] private TMP_Text timerText;
    
    private void Start() {
        GameTimeManager.Instance.OnGameTimeUpdate += GameTimeManager_OnGameTimeUpdate;
        
        UpdateVisual();
    }

    private void OnDestroy() {
        GameTimeManager.Instance.OnGameTimeUpdate -= GameTimeManager_OnGameTimeUpdate;
    }

    private void GameTimeManager_OnGameTimeUpdate(object sender, GameTimeManager.OnGameTimeUpdateEventArgs e) {
        UpdateVisual(e.gameTime);
    }

    private void UpdateVisual(float gameTime = 0) {
        timerText.text = FormatTime(Mathf.FloorToInt(gameTime));
    }
    private string FormatTime(int timeInSeconds) {
        return TimeSpan.FromSeconds(timeInSeconds).ToString(@"mm\:ss");
    }
}
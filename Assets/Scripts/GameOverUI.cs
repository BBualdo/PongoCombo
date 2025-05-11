using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private TMP_Text playerWinnerText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button exitGameButton;
    
    private void Start() {
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        Hide();
        
        playAgainButton.onClick.AddListener(() => {
            GameManager.Instance.RestartGame();
            Hide();
        });
        
        exitGameButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Scenes/MainMenuScene");
        });
    }

    private void OnDestroy() {
        GameManager.Instance.OnGameOver -= GameManager_OnGameOver;
    }

    private void GameManager_OnGameOver(object sender, GameManager.OnGameOverEventArgs e) {
        Show();
        playerWinnerText.text = $"{e.winner.playerName} wygrywa";
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

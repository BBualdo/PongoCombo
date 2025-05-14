using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour {
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;

    private void Start() {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        resumeButton.onClick.AddListener(() => TogglePause());
        exitButton.onClick.AddListener(() => {
            TogglePause();
            SceneManager.LoadScene(nameof(Loader.Scene.MainMenuScene));
        });
        
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, EventArgs e) {
        if (gameObject.activeSelf) {
            Hide();
        } else {
            Show();
        }
    }

    private void TogglePause() {
        GameManager.Instance.TogglePause();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

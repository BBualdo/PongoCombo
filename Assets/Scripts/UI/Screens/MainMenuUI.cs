using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private DifficultySelectUI difficultySelectUI;
    
    [SerializeField] private Button soloButton;
    [SerializeField] private Button pvpButton;
    [SerializeField] private Button quitButton;

    public event EventHandler OnSoloGameChosen;
    
    private void Start() {
        difficultySelectUI.OnBackToMenu += DifficultySelectUI_OnBackToMenu;
        
        soloButton.onClick.AddListener(() => {
            PlayerPrefs.SetInt(PlayerPrefsHelper.GAME_MODE, 1);
            OnSoloGameChosen?.Invoke(this, EventArgs.Empty);
            Hide();
        });
        pvpButton.onClick.AddListener(() => {
            PlayerPrefs.SetInt(PlayerPrefsHelper.GAME_MODE, 2);
            Loader.Instance.LoadScene(Loader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    private void DifficultySelectUI_OnBackToMenu(object sender, EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

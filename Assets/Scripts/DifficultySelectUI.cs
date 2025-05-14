using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultySelectUI : MonoBehaviour
{
    private enum Difficulty {
        Easy, Medium, Hard
    }
    
    [SerializeField] private MainMenuUI mainMenuUI;
    
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button backButton;

    public event EventHandler OnBackToMenu;
    
    private void Start() {
        mainMenuUI.OnSoloGameChosen += MainMenuUI_OnSoloGameChosen;
        
        easyButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Easy));
        mediumButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Medium));
        hardButton.onClick.AddListener(() => SelectDifficulty(Difficulty.Hard));
        backButton.onClick.AddListener(() => BackToMenu());
        
        Hide();
    }

    private void MainMenuUI_OnSoloGameChosen(object sender, EventArgs e) {
        Show();
    }

    private void BackToMenu() {
        OnBackToMenu?.Invoke(this, EventArgs.Empty);
        Hide();
    }

    private void SelectDifficulty(Difficulty difficulty) {
        PlayerPrefs.SetInt(PlayerPrefsHelper.GAME_DIFFICULTY, (int)difficulty);
        SceneManager.LoadScene("GameScene");
    }
    
    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

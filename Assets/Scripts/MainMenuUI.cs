using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button soloButton;
    [SerializeField] private Button pvpButton;
    [SerializeField] private Button quitButton;

    private void Start() {
        soloButton.onClick.AddListener(() => {
            PlayerPrefs.SetInt(PlayerPrefsHelper.GAME_MODE, 1);
            SceneManager.LoadScene("GameScene");
        });
        pvpButton.onClick.AddListener(() => {
            PlayerPrefs.SetInt(PlayerPrefsHelper.GAME_MODE, 2);
            SceneManager.LoadScene("GameScene");
        });
        quitButton.onClick.AddListener(() => Application.Quit());
    }
}

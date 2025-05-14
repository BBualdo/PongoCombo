using TMPro;
using UnityEngine;

public class GameCountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    
    private void Start()
    {
        GameManager.Instance.OnCountdownChanged += GameManager_OnCountdownChanged;
        Hide();
    }

    private void OnDestroy() {
        GameManager.Instance.OnCountdownChanged -= GameManager_OnCountdownChanged;
    }

    private void GameManager_OnCountdownChanged(object sender, GameManager.OnCountdownChangedEventArgs e) {
        if (e.countdownTimer > 0) {
            Show();
            countdownText.text = Mathf.CeilToInt(e.countdownTimer).ToString();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

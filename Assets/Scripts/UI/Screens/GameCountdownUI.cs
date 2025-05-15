using TMPro;
using UnityEngine;

public class GameCountdownUI : MonoBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    
    private void Start()
    {
        GameTimeManager.Instance.OnCountdownChanged += GameTimeManager_OnCountdownChanged;
        Hide();
    }

    private void OnDestroy() {
        GameTimeManager.Instance.OnCountdownChanged -= GameTimeManager_OnCountdownChanged;
    }

    private void GameTimeManager_OnCountdownChanged(object sender, GameTimeManager.OnCountdownChangedEventArgs e) {
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

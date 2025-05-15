using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour {
    [SerializeField] private Transform loadingBarContainer;
    [SerializeField] private Image loadingBar;
    [SerializeField] private TMP_Text pressToContinueText;
    
    public event EventHandler OnReadyToContinue;
    
    private void Start() {
        Loader.Instance.OnLoadProgressChanged += Loader_OnLoadProgressChanged;
        Loader.Instance.OnLoadComplete += Loader_OnLoadComplete;
        pressToContinueText.gameObject.SetActive(false);
        Hide();
    }

    private void OnDestroy() {
        Loader.Instance.OnLoadProgressChanged -= Loader_OnLoadProgressChanged;
        Loader.Instance.OnLoadComplete -= Loader_OnLoadComplete;
    }
    
    private void Loader_OnLoadComplete(object sender, EventArgs e) {
        loadingBarContainer.gameObject.SetActive(false);
        pressToContinueText.gameObject.SetActive(true);
    }

    private void Loader_OnLoadProgressChanged(object sender, Loader.OnLoadProgressChangedEventArgs e) {
        if (!gameObject.activeSelf) {
            Show();
        }
        
        loadingBar.fillAmount = e.progress;
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}

using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour {
    [SerializeField] private Image loadingBar;
    
    private void Start() {
        Loader.Instance.OnLoadProgressChanged += Loader_OnLoadProgressChanged;
        Hide();
    }

    private void OnDestroy() {
        Loader.Instance.OnLoadProgressChanged -= Loader_OnLoadProgressChanged;
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

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour {
    public enum Scene {
        GameScene,
        MainMenuScene,
    }
    
    public static Loader Instance { get; private set; }

    public event EventHandler<OnLoadProgressChangedEventArgs> OnLoadProgressChanged;
    public class OnLoadProgressChangedEventArgs : EventArgs {
        public float progress;
    }
    
    public event EventHandler OnLoadComplete;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(Scene scene) {
        StartCoroutine(LoadSceneAsync(scene));
    }

    private IEnumerator LoadSceneAsync(Scene scene) {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene.ToString());
        loadOperation.allowSceneActivation = false;
        
        while (loadOperation.progress < .9f) {
            float loadProgress = Mathf.Clamp01(loadOperation.progress / .9f);
            OnLoadProgressChanged?.Invoke(this, new OnLoadProgressChangedEventArgs {
                progress = loadProgress
            });
            yield return null;
        }
        
        OnLoadProgressChanged?.Invoke(this, new OnLoadProgressChangedEventArgs {
            progress = 1f
        });
        
        yield return new WaitForSeconds(1f);
        
        OnLoadComplete?.Invoke(this, EventArgs.Empty);

        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        
        loadOperation.allowSceneActivation = true;
    }
}
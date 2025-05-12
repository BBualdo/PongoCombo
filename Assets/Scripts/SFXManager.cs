using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [SerializeField] private SFXListSO sfxListSO;
    
    private AudioSource audioSource;
    private Ball ball;
    private int previousCountdownInt;
    
    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        GameManager.Instance.OnBallSpawned += GameManager_OnBallSpawned;
        GameManager.Instance.OnBallScored += GameManager_OnBallScored;
        GameManager.Instance.OnCountdownChanged += GameManager_OnCountdownChanged;
        
        ball = GameManager.Instance.GetCurrentBall();
        if (ball != null) {
            ball.OnPlayerHit += Ball_OnPlayerHit;
            ball.OnWallHit += Ball_OnWallHit;
        }
    }
    
    private void OnDestroy() {
        GameManager.Instance.OnBallSpawned -= GameManager_OnBallSpawned;
        GameManager.Instance.OnBallScored -= GameManager_OnBallScored;
    }
    
    private void GameManager_OnCountdownChanged(object sender, GameManager.OnCountdownChangedEventArgs e) {
        int currentCountdownInt = Mathf.CeilToInt(e.countdownTimer);
        
        if (currentCountdownInt == previousCountdownInt) return;
        
        previousCountdownInt = currentCountdownInt;
        switch (previousCountdownInt) {
            case 3:
            case 2:
            case 1:
                PlayOneShot(sfxListSO.countdownSoundShort);
                break;
            case 0:
                PlayOneShot(sfxListSO.countdownSoundLong);
                break;
        }
    }

    private void GameManager_OnBallSpawned(Ball newBall) {
        ball = newBall;
        ball.OnPlayerHit += Ball_OnPlayerHit;
        ball.OnWallHit += Ball_OnWallHit;
    }

    private void GameManager_OnBallScored(object sender, EventArgs e) {
        PlayOneShot(sfxListSO.floorHitSounds);
    }

    private void Ball_OnWallHit(object sender, EventArgs e) {
        PlayOneShot(sfxListSO.wallHitSounds);
    }

    private void Ball_OnPlayerHit(object sender, EventArgs e) {
        PlayOneShot(sfxListSO.paletteHitSounds);
    }

    private void PlayOneShot(AudioClip clip, float volume = 1f) {
        audioSource.PlayOneShot(clip, volume);
    }

    private void PlayOneShot(AudioClip[] clips, float volume = 1f) {
        PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    } 
}

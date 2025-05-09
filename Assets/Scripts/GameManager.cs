using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform ballSpawnPoint;
    [SerializeField] private Ball ballPrefab;
    public static GameManager Instance { get; private set; }

    private Ball currentBall;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Ball newBall = Instantiate(ballPrefab, ballSpawnPoint);
        RegisterBall(newBall);
    }

    public void RegisterBall(Ball ball) {
        currentBall = ball;
    }

    public void DestroyCurrentBall() {
        if (currentBall == null) return;
        Destroy(currentBall.gameObject);
    }

    public Ball GetCurrentBall() {
        return currentBall;
    }
}

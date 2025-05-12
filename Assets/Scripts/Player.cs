using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    private enum PlayerSide {
        PlayerL,
        PlayerR
    }
    public event EventHandler OnDamageTaken;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private Transform playerBallHoldPoint;

    [Header("Player Settings")] 
    public string playerName;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private PlayerSide playerSide;
    private float playerHeight;
    
    [Header("AI")]
    [SerializeField] private bool isAIControlled;
    [SerializeField] private float followSpeed = 5f;
    private float timeToServe = 0f;
    private float timeToServeMax = 2f;

    [Header("Health")] 
    [SerializeField] private float maxHealth = 100f;
    private float healthLeft;
    
    [Header("ServeDirectionDraw")]
    private float yServeDirectionMax = 3f;
    private float yServeDirection = 0f;
    private int yServeDirectionStep = 1; // 1 = up | -1 = down
    

    // The yMoveBound is max absolute position value for the player to hit the walls
    private float yMoveBound;
    private float yMoveBoundOffset = 0.1f;

    private void Awake() {
        playerHeight = playerVisual.localScale.y;
        healthLeft = maxHealth;
    }

    private void Start() {
        yMoveBound = (Field.Instance.GetFieldHeight() - playerHeight) / 2;
        isAIControlled = playerSide == PlayerSide.PlayerR && GameManager.Instance.GetGameMode() == 1;
    }

    private void Update() {
        if (GameManager.Instance.GetState() != GameManager.State.GameOver) {
            if (!isAIControlled) {
                HandleMovement();
            } else {
                HandleAIMovement();
            }
        
            ApplyPlayerMoveBounds();
            HandleServing();
        }
    }

    private void HandleMovement() {
        KeyCode up = playerSide == PlayerSide.PlayerL ? KeyCode.W : KeyCode.UpArrow;
        KeyCode down = playerSide == PlayerSide.PlayerL ? KeyCode.S : KeyCode.DownArrow;
        
        if (Input.GetKey(up)) {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        } else if (Input.GetKey(down)) {
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        }
    }

    private void HandleServing() {
        if (TryGetBall(out Ball ball)) {
            if (!isAIControlled) {
                // Start direction draw
                Vector2 serveDirection = DrawServeDirection();

                if (Input.GetKeyDown(KeyCode.Space)) {
                    ball.PerformServe(serveDirection);
                }
            } else {
                timeToServe += Time.deltaTime;
                Vector2 serveDirection = DrawServeDirection();
                
                if (timeToServe >= timeToServeMax) {
                    timeToServe = 0f;
                    ball.PerformServe(serveDirection);
                }
            }
        }
    }

    private void HandleAIMovement() {
        Ball ball = GameManager.Instance.GetCurrentBall();
        float targetY;
        
        if (ball != null) {
            targetY = Mathf.Lerp(transform.position.y, ball.transform.position.y, followSpeed * Time.deltaTime);
        } else {
            targetY = Mathf.Lerp(transform.position.y, 0, followSpeed * Time.deltaTime);
        }
        
        transform.position = new Vector2(transform.position.x, targetY);
    }

    private void ApplyPlayerMoveBounds() {
        Vector3 position = transform.position;
        float posY = Mathf.Clamp(position.y, -yMoveBound, yMoveBound);
        transform.position = new Vector3(position.x, posY, position.z);
    }

    public float GetHitOffsetNormalized(ContactPoint2D contact) {
        float offset = contact.point.y - transform.position.y;
        return offset / (playerHeight / 2);
    }

    public Transform GetPlayerBallHoldPoint() {
        return playerBallHoldPoint;
    }

    private bool TryGetBall(out Ball ball) {
        if (playerBallHoldPoint.childCount == 0) {
            ball = null;
            return false;
        }
        
        if (playerBallHoldPoint.GetChild(0).TryGetComponent<Ball>(out ball)) {
            return true;
        } else {
            ball = null;
            return false;
        }
    }

    public void TakeDamage(float damage) {
        healthLeft -= damage;
        OnDamageTaken?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPercent() {
        return (healthLeft / maxHealth) * 100;
    }

    public float GetRemainingHealth() {
        return healthLeft;
    }

    public float GetMaxHealth() {
        return maxHealth;
    }

    public bool IsDead() {
        return GetHealthPercent() <= 0;
    }

    public void Reset() {
        transform.position = new Vector2(transform.position.x, 0);
        healthLeft = maxHealth;
    }

    private Vector2 DrawServeDirection() {
        // Player should serve to an opposite direction of his field position
        float xServeDirection = playerSide == PlayerSide.PlayerL ? 1f : -1f;
        float drawSpeed = 2f;

        yServeDirection += yServeDirectionStep * (drawSpeed * Time.deltaTime);
        if (Math.Abs(yServeDirection) >= yServeDirectionMax) {
            yServeDirectionStep *= -1;
        }
        
        return new Vector2(xServeDirection, yServeDirection);
    }
}

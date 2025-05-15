using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MonoBehaviour {
    public event EventHandler OnDamageTaken;
    public event EventHandler<OnServeDirectionChangedEventArgs> OnServeDirectionChanged;
    public class OnServeDirectionChangedEventArgs {
        public Vector2 serveDirection;
    }

    public event EventHandler OnBallGrabbed;
    public event EventHandler OnBallServed;

    public string PlayerName { get; private set; }
    public float PlayerHeight { get; private set; } = 2.5f;
    public PlayerManager.PlayerSide PlayerSide => playerSide;
    public float MaxHealth => 100f;
    public float HealthLeft { get; private set; }
    
    [Header("Player Components")]
    [SerializeField] private Transform playerVisual;
    [SerializeField] private Transform playerBallHoldPoint;
    private BoxCollider2D playerCollider;

    [Header("Player Settings")] 
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private PlayerManager.PlayerSide playerSide;
    
    [Header("AI")]
    [SerializeField] private bool isAIControlled;
    private float followSpeed;
    private float timeToServe = 0f;
    private float timeToServeMax = 2f;
    
    [Header("ServeDirectionDraw")]
    private float yServeDirectionMax = 2f;
    private float yServeDirection = 0f;
    private int yServeDirectionStep = 1; // 1 = up | -1 = down
    
    // The yMoveBound is max absolute position value for the player to hit the walls
    private float yMoveBound;

    private void Awake() {
        playerCollider = GetComponent<BoxCollider2D>();
        SetPlayerHeight();
        HealthLeft = MaxHealth;
        followSpeed = CalculateFollowSpeed();
        PlayerName = PlayerSide == PlayerManager.PlayerSide.PlayerL ? "Player 1" : "Player 2";
    }

    private void Start() {
        UpdateYMoveBound();
        isAIControlled = playerSide == PlayerManager.PlayerSide.PlayerR && GameManager.Instance.GetGameMode() == 1;
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

    private float CalculateFollowSpeed() {
        int difficulty = PlayerPrefs.GetInt(PlayerPrefsHelper.GAME_DIFFICULTY, 1);

        switch (difficulty) {
            case 0:
                return 3f;
            case 1:
                return 5f;
            case 2:
                return 7f;
        }

        return 0f;
    }

    private void UpdateYMoveBound() {
        yMoveBound = (Field.Instance.GetFieldHeight() - PlayerHeight) / 2;
    }

    private void HandleMovement() {
        KeyCode up = playerSide == PlayerManager.PlayerSide.PlayerL ? KeyCode.W : KeyCode.UpArrow;
        KeyCode down = playerSide == PlayerManager.PlayerSide.PlayerL ? KeyCode.S : KeyCode.DownArrow;
        
        if (Input.GetKey(up)) {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        } else if (Input.GetKey(down)) {
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        }
    }

    private void HandleServing() {
        if (!TryGetBall(out Ball ball)) return; 
        
        OnBallGrabbed?.Invoke(this, EventArgs.Empty);    
            
        if (!isAIControlled) 
            HandlePlayerServing(ball);
        else
            HandleAIServing(ball);
    }

    private void HandlePlayerServing(Ball ball) {
        Vector2 serveDirection = DrawServeDirection();

        if (Input.GetKeyDown(KeyCode.Space)) {
            ball.PerformServe(serveDirection);
            OnBallServed?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HandleAIServing(Ball ball) {
        timeToServe += Time.deltaTime;
        Vector2 serveDirection = DrawServeDirection();
            
        if (timeToServe >= timeToServeMax) {
            timeToServe = 0f;
            ball.PerformServe(serveDirection);
            OnBallServed?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private void HandleAIMovement() {
        Ball ball = BallManager.Instance.GetCurrentBall();
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
        return offset / (PlayerHeight / 2);
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
        HealthLeft -= damage;
        OnDamageTaken?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthPercent() {
        return (HealthLeft / MaxHealth) * 100;
    }

    public bool IsDead() {
        return GetHealthPercent() <= 0;
    }

    public void Reset() {
        transform.position = new Vector2(transform.position.x, 0);
        HealthLeft = MaxHealth;
    }

    private Vector2 DrawServeDirection() {
        // Player should serve to an opposite direction of his field position
        float xServeDirection = playerSide == PlayerManager.PlayerSide.PlayerL ? 1f : -1f;
        float drawSpeed = 2f;

        yServeDirection += yServeDirectionStep * (drawSpeed * Time.deltaTime);
        if (Math.Abs(yServeDirection) >= yServeDirectionMax) {
            yServeDirectionStep *= -1;
        }

        Vector2 serveDirection = new Vector2(xServeDirection, yServeDirection);
        
        OnServeDirectionChanged?.Invoke(this, new OnServeDirectionChangedEventArgs {
            serveDirection = serveDirection
        });
        
        return serveDirection;
    }

    public void SetPlayerHeight(float newHeight = 2.5f) {
        playerVisual.localScale = new Vector3(playerVisual.localScale.x, newHeight, playerVisual.localScale.z);
        PlayerHeight = playerVisual.localScale.y;

        Vector2 colliderSize = playerCollider.size;
        playerCollider.size = new Vector2(colliderSize.x, newHeight);
        
        UpdateYMoveBound();
    }
}

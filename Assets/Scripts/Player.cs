using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private Transform playerVisual;
    [SerializeField] private Transform playerBallHoldPoint;
    [SerializeField] private bool isAIControlled;
    [SerializeField] private float moveSpeed = 12f;
    private float playerHeight;

    [Header("Health")] 
    private float maxHealth = 100f;
    private float healthLeft;
    

    // The yMoveBound is max absolute position value for the player to hit the walls
    private float yMoveBound;

    private void Awake() {
        playerHeight = playerVisual.localScale.y;
        healthLeft = maxHealth;
    }

    private void Start() {
        yMoveBound = (Field.Instance.GetFieldHeight() - playerHeight) / 2;
    }

    private void Update() {
        if (!isAIControlled) {
            HandleMovement();
        } else {
            HandleAIMovement();
        }
        
        ApplyPlayerMoveBounds();
        HandleServing();
    }

    private void HandleMovement() {
        if (Input.GetKey(KeyCode.W)) {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.S)) {
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        }
    }

    private void HandleServing() {
        if (TryGetBall(out Ball ball) && Input.GetKeyDown(KeyCode.Space)) {
            var sign = Mathf.Sign(playerBallHoldPoint.transform.localPosition.x);
            ball.PerformServe(new Vector2(sign, 0));
        }
    }

    private void HandleAIMovement() {
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, GameManager.Instance.GetCurrentBall().transform.position.y, position.z);
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
    }

    public float GetHealthPercent() {
        return (healthLeft / maxHealth) * 100;
    }
}

using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] private Transform playerBallHoldPoint;
    [SerializeField] private bool isAIControlled;
    [SerializeField] private float moveSpeed = 5f;
    private float playerHeight;

    // The yMoveBound is max absolute position value for the player to hit the walls
    private float yMoveBound;

    private void Awake() {
        playerHeight = transform.localScale.y;
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
    }

    private void HandleMovement() {
        if (Input.GetKey(KeyCode.W)) {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.S)) {
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
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

    public void GiveBall(Ball ball) {
        ball.StopMoving();
        ball.transform.position = playerBallHoldPoint.position;
    }
}

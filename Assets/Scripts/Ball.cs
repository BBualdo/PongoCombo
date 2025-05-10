using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    private Vector2 moveDirection;
    private bool canMove = true;
    private float currentDamage;

    private void Start() {
        moveDirection = new Vector2(-1, 0);
    }

    private void Update() {
        if (canMove) {
            transform.position += (Vector3)moveDirection.normalized * (moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        moveDirection = Vector2.Reflect(moveDirection, other.GetContact(0).normal);
        if (other.gameObject.TryGetComponent<Player>(out Player player)) {
            float offset = player.GetHitOffsetNormalized(other.GetContact(0));
            moveDirection.y += offset;
            CorrectDirectionX();

            IncreaseBallDamage();
        }
    }

    private void CorrectDirectionX() {
        float minX = 1f;

        if (Mathf.Abs(moveDirection.x) < minX) {
            moveDirection.x = Mathf.Sign(moveDirection.x) * minX;
        }
    }

    public void StopMoving() {
        canMove = false;
        moveDirection = Vector2.zero;
    }

    public void PerformServe(Vector2 serveDirection) {
        transform.parent = null;
        moveDirection = serveDirection;
        canMove = true;
    }

    private void IncreaseBallDamage() {
        currentDamage++;
    }

    public float GetBallDamage() {
        return currentDamage;
    }
}

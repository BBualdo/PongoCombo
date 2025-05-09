using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    private Vector2 moveDirection;

    private bool isMoving;

    private void Start() {
        isMoving = true;
        moveDirection = new Vector2(-1, 0);
    }

    private void Update() {
        if (isMoving) {
            transform.position += (Vector3)moveDirection.normalized * (moveSpeed * Time.deltaTime);
        } else {
            transform.position = GetBallParent().position;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        moveDirection = Vector2.Reflect(moveDirection, other.GetContact(0).normal);
        if (other.gameObject.TryGetComponent<Player>(out Player player)) {
            float offset = player.GetHitOffsetNormalized(other.GetContact(0));
            moveDirection.y += offset;
            CorrectDirectionX();
        }
    }

    private void CorrectDirectionX() {
        float minX = 1f;

        if (Mathf.Abs(moveDirection.x) < minX) {
            moveDirection.x = Mathf.Sign(moveDirection.x) * minX;
        }
    }

    private Transform GetBallParent() {
        return transform.parent;
    }

    public void StopMoving() {
        moveDirection = Vector2.zero;
        isMoving = false;
    }
}

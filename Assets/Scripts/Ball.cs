using UnityEngine;

public class Ball : MonoBehaviour
{
    public static Ball Instance { get; private set; }

    [SerializeField] private float moveSpeed = 7f;
    private Vector2 moveDirection;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        moveDirection = new Vector2(-1, 0);
    }

    private void Update() {
        transform.position += (Vector3)moveDirection.normalized * (moveSpeed * Time.deltaTime);
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
}

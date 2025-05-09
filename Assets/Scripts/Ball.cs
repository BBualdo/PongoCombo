using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    private Vector2 moveDirection;

    private void Start() {
        moveDirection = new Vector2(-1, 0);
    }

    private void Update() {
        transform.position += (Vector3)moveDirection * (moveSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        moveDirection = Vector2.Reflect(moveDirection, other.GetContact(0).normal);
    }
}

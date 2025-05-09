using UnityEngine;

public class Player : MonoBehaviour {
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
        if (Input.GetKey(KeyCode.W)) {
            transform.position += Vector3.up * (moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S)) {
            transform.position += Vector3.down * (moveSpeed * Time.deltaTime);
        }

        Vector3 position = transform.position;
        float posY = Mathf.Clamp(position.y, -yMoveBound, yMoveBound);
        transform.position = new Vector3(position.x, posY, position.z);
    }

    public float GetHitOffsetNormalized(ContactPoint2D contact) {
        float offset = contact.point.y - transform.position.y;
        return offset / (playerHeight / 2);
    }
}

using UnityEngine;

public class GoalLine : MonoBehaviour
{
    [SerializeField] Player goalLineOwner;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<Ball>(out Ball ball)) {
            Debug.Log("Goal!");
            goalLineOwner.GiveBall(ball);
        }
    }
}

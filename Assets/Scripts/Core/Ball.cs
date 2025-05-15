using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour {
    public event EventHandler OnDamageIncreased;
    public event EventHandler OnPlayerHit;
    public event EventHandler OnWallHit;
    
    public float CurrentDamage { get; private set; } = 1;
    
    [SerializeField] private float moveSpeed = 10f;
    private Vector2 moveDirection;
    private bool canMove = true;

    private void Start() {
        moveDirection = GetRandomDirection();
    }

    private void Update() {
        if (canMove) {
            transform.position += (Vector3)moveDirection.normalized * (moveSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!canMove) {
            // The ball can't move, so it's in player's hands - don't apply collision 
            return;
        }
        
        moveDirection = Vector2.Reflect(moveDirection, other.GetContact(0).normal);
        if (other.gameObject.TryGetComponent<Player>(out Player player)) {
            OnPlayerHit?.Invoke(this, EventArgs.Empty);
            
            float offset = player.GetHitOffsetNormalized(other.GetContact(0));
            moveDirection.y += offset;
            CorrectDirectionX();

            IncreaseBallDamage();
        } else {
            OnWallHit?.Invoke(this, EventArgs.Empty);
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
        OnPlayerHit?.Invoke(this, EventArgs.Empty);
        moveDirection = serveDirection;
        canMove = true;
    }

    private void IncreaseBallDamage() {
        CurrentDamage += BallManager.Instance.GetBallDamageIncreaseValue();
        OnDamageIncreased?.Invoke(this, EventArgs.Empty);
    }

    private Vector2 GetRandomDirection() {
        float randomX = Random.Range(-1f, 1.01f);
        if (randomX < 0) randomX = -1f;
        else randomX = 1f;
        
        float randomY = Random.Range(-3f, 3.01f);
        
        return new Vector2(randomX, randomY);
    }
}

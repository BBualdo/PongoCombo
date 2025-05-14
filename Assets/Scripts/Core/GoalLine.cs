using System;
using UnityEngine;

public class GoalLine : MonoBehaviour
{
    public enum GoalSide {
        Left, Right
    }

    public static event EventHandler<OnGoalScoredEventArgs> OnGoalScored;
    public class OnGoalScoredEventArgs : EventArgs {
        public GoalSide goalSide;
    }

    [SerializeField] private GoalSide side;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Ball>() != null) {
            OnGoalScored?.Invoke(this, new OnGoalScoredEventArgs {
                goalSide = side
            });
        }
    }
}

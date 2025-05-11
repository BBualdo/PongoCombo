using UnityEngine;

public class Field : MonoBehaviour {
    public static Field Instance { get; private set; }

    [SerializeField] private Transform topWall;
    [SerializeField] private Transform bottomWall;

    private LineRenderer topWallLine;
    private LineRenderer bottomWallLine;

    private void Awake() {
        Instance = this;

        topWallLine = topWall.GetComponent<LineRenderer>();
        bottomWallLine = bottomWall.GetComponent<LineRenderer>();
    }

    public float GetFieldHeight() {
        float topLinePosY = topWallLine.GetPosition(0).y - topWallLine.startWidth / 2;
        float bottomLinePosY = bottomWallLine.GetPosition(0).y + bottomWallLine.startWidth / 2;
        
        return Mathf.Abs(topLinePosY - bottomLinePosY);
    }
}
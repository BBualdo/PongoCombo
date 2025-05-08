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

    public int GetFieldHeight() {
        return Mathf.Abs((int)(topWallLine.GetPosition(0).y - bottomWallLine.GetPosition(0).y));
    }
}
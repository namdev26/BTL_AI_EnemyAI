using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("A* Info")]
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;
    public Node parent;

    [Header("Grid Info")]
    public int gridX;
    public int gridY;
    public List<Node> neighbors = new();
    public bool isObstacle = false;
    public Color originalColor = Color.white;

    private NodeVisual visual;

    private void Awake()
    {
        visual = GetComponent<NodeVisual>();
        visual?.Init(originalColor);
    }

    // Khởi tạo thông tin cho node
    public void Init(Vector3Int gridPosition)
    {
        gridX = gridPosition.x;
        gridY = gridPosition.y;
        name = $"Node ({gridX},{gridY})";
        UpdateColor();
    }

    // Kích hoạt bằng chuột khi đánh dấu ô không thể đi qua và ngược lại hủy
    public void ToggleObstacle()
    {
        isObstacle = !isObstacle;
        UpdateColor();
    }

    // Cập nhật màu ô cản
    public void UpdateColor()
    {
        if (visual == null) visual = GetComponent<NodeVisual>();
        if (isObstacle)
            visual?.SetAsObstacle();
        else
            visual?.SetColor(originalColor);
    }

    // Reset màu all
    public void ResetVisual()
    {
        if (!isObstacle)
            visual?.ResetVisual(originalColor);
    }


    //Set màu
    public void SetExploredVisual(Color color)
    {
        visual?.SetColor(color);
    }
}

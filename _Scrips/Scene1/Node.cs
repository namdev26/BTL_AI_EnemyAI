using System.Collections.Generic;
using System.Xml.Linq;
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

    [SerializeField] private NodeVisual visual;

    private void Awake()
    {
        visual = GetComponent<NodeVisual>();
        visual?.Init(originalColor);
    }

    public void Init(Vector3Int gridPosition)
    {
        gridX = gridPosition.x;
        gridY = gridPosition.y;
        name = $"Node ({gridX},{gridY})";
        UpdateColor();
    }

    public void ToggleObstacle()
    {
        isObstacle = !isObstacle;
        UpdateColor();
    }

    public void UpdateColor()
    {
        if (visual == null) visual = GetComponent<NodeVisual>();
        if (isObstacle)
            visual?.SetAsObstacle();
        else
            visual?.SetColor(originalColor);
    }

    public void ResetVisual()
    {
        if (!isObstacle)
            visual?.ResetVisual(originalColor);
    }

    public void SetExploredVisual(Color color)
    {
        visual?.SetColor(color);
    }
}

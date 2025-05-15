using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("A* Info")]
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost; public Node parent;
    [Header("Grid Info")]
    public int gridX;
    public int gridY;
    public List<Node> neighbors = new();
    public bool isObstacle = false;

    private NodeVisual visual;

    private void Awake()
    {
        visual = GetComponent<NodeVisual>();
        visual?.Init();
    }

    public void Init(Vector3Int gridPosition)
    {
        gridX = gridPosition.x;
        gridY = gridPosition.y;
        name = $"Node ({gridX},{gridY})";
        UpdateVisual();
    }

    public void ToggleObstacle()
    {
        isObstacle = !isObstacle;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (visual == null) visual = GetComponent<NodeVisual>();
        if (isObstacle)
            visual?.SetAsObstacle();
        else
            visual?.ResetVisual();
    }

    public void ResetVisual()
    {
        if (!isObstacle)
            visual?.ResetVisual();
    }

    public void SetFrontierVisual()
    {
        visual?.SetAsFrontier();
    }

    public void SetExploredVisual()
    {
        visual?.SetAsExplored();
    }
}
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
    public List<Node> neighbors = new List<Node>();
    public bool isObstacle = false;
    public Color originalColor = Color.white;

    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        UpdateColor();
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
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        sr.color = isObstacle ? Color.black : Color.white;
    }

    public void ResetVisual()
    {
        if (!isObstacle)
            sr.color = originalColor;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isObstacle ? Color.black : Color.green;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.9f);
    }

    public void SetExploredVisual(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

}

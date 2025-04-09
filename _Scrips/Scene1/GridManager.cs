using UnityEngine;

[DefaultExecutionOrder(-10)]
public class GridManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public int width = 10;
    public int height = 10;
    public float spacing = 1f;
    public Node[,] grid;

    void Start()
    {
        GenerateGrid();
        AssignNeighbors();
        CenterCameraOnGrid();
    }

    void GenerateGrid()
    {
        grid = new Node[width, height];

        // Center the grid around (0, 0)
        Vector3 origin = Vector3.zero;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = origin + new Vector3(x * spacing, y * spacing, 0);
                GameObject nodeObj = Instantiate(nodePrefab, pos, Quaternion.identity, transform);
                nodeObj.name = $"Node ({x},{y})";

                Node node = nodeObj.GetComponent<Node>();
                node.gridX = x;
                node.gridY = y;
                grid[x, y] = node;
            }
        }
    }

    void AssignNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = grid[x, y];
                node.neighbors.Clear();

                TryAddNeighbor(node, x - 1, y); // Left
                TryAddNeighbor(node, x + 1, y); // Right
                TryAddNeighbor(node, x, y + 1); // Up
                TryAddNeighbor(node, x, y - 1); // Down
            }
        }
    }

    void TryAddNeighbor(Node node, int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            Node neighbor = grid[x, y];
            if (!neighbor.isObstacle)
            {
                node.neighbors.Add(neighbor);
            }
        }
    }

    void CenterCameraOnGrid()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 centerPosition = new Vector3(0, 0, -10); // Z = -10 for 2D
        cam.transform.position = centerPosition;

        float aspectRatio = (float)cam.pixelWidth / cam.pixelHeight;
        float gridWidth = width * spacing;
        float gridHeight = height * spacing;

        float sizeX = gridWidth / (2f * aspectRatio);
        float sizeY = gridHeight / 2f;

        cam.orthographicSize = Mathf.Max(sizeX, sizeY) + 1f; // +1 for padding
    }
}

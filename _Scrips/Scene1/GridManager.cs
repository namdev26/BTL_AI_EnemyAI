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

        // Tính toán vị trí trung tâm của grid
        float gridWidth = width * spacing;
        float gridHeight = height * spacing;
        Vector3 center = new Vector3((width - 1) * spacing / 2f, (height - 1) * spacing / 2f, -10f);

        cam.transform.position = center;

        // Tính aspect ratio
        float aspect = (float)cam.pixelWidth / cam.pixelHeight;

        // Tính toán kích thước camera sao cho bao trùm toàn bộ grid
        float sizeX = gridWidth / (2f * aspect);
        float sizeY = gridHeight / 2f;

        cam.orthographicSize = Mathf.Max(sizeX, sizeY) + 0.5f; // Thêm chút padding
    }


    public Node GetNodeFromWorldPosition(Vector3 worldPos)
    {
        float spacing = this.spacing;
        int x = Mathf.RoundToInt(worldPos.x / spacing);
        int y = Mathf.RoundToInt(worldPos.y / spacing);

        if (x >= 0 && y >= 0 && x < width && y < height)
            return grid[x, y];

        return null;
    }
}

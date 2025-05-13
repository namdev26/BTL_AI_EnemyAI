using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(-10)]
public class GridManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public int width = 10;
    public int height = 10;
    public float spacing = 1f;
    public Node[,] grid;

    // Tham chiếu đến ObstacleGenerator
    private ObstacleGenerator obstacleGenerator;
    public ObstacleGenerator ObstacleGenerator => obstacleGenerator;

    private void Awake()
    {
        // Kiểm tra nếu đã có ObstacleGenerator
        obstacleGenerator = GetComponent<ObstacleGenerator>();

        // Nếu chưa có, tạo mới
        if (obstacleGenerator == null)
        {
            obstacleGenerator = gameObject.AddComponent<ObstacleGenerator>();
        }
    }

    void Start()
    {
        GenerateGrid(); // khởi tạo grid
        AssignNeighbors(); // tìm các node lân cận
        CenterCameraOnGrid();
    }

    // Đồng bộ các thông số từ GridManager sang ObstacleGenerator


    void GenerateGrid()
    {
        grid = new Node[width, height];
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

    // Phương thức này được public để có thể gọi lại sau khi tạo obstacle
    public void AssignNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = grid[x, y];
                node.neighbors.Clear();
                TryAddNeighbor(node, x - 1, y); // Trái
                TryAddNeighbor(node, x + 1, y); // Phải
                TryAddNeighbor(node, x, y + 1); // Trên
                TryAddNeighbor(node, x, y - 1); // Dưới
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

        float gridWidth = width * spacing;
        float gridHeight = height * spacing;
        Vector3 center = new Vector3((width - 1) * spacing / 2f, (height - 1) * spacing / 2f, -10f);
        cam.transform.position = center;

        float aspect = (float)cam.pixelWidth / cam.pixelHeight;
        float sizeX = gridWidth / (2f * aspect);
        float sizeY = gridHeight / 2f;
        cam.orthographicSize = Mathf.Max(sizeX, sizeY) + 0.5f;
    }

    // Phương thức public để tạo vật cản từ bên ngoài
    public void GenerateRandomObstacles()
    {
        if (obstacleGenerator != null)
        {
            obstacleGenerator.GenerateRandomObstacles();
        }
        else
        {
            Debug.LogError("ObstacleGenerator không khả dụng");
        }
    }
}
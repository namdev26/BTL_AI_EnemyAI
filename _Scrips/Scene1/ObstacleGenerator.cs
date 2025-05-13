using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class ObstacleGenerator : MonoBehaviour
{
    [Header("Random Obstacles")]
    [Range(0, 50)]
    public int obstaclePercentage = 20; // Phần trăm vật cản trên lưới
    public bool generateRandomObstacles = false; // Bật/tắt sinh vật cản ngẫu nhiên

    private GridManager gridManager;

    public event Action<string> OnError;
    public event Action OnObstaclesGenerated;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("Không tìm thấy GridManager");
            OnError?.Invoke("Không tìm thấy GridManager");
            return;
        }

        StartCoroutine(WaitUntilGridIsReady());
    }

    IEnumerator WaitUntilGridIsReady()
    {
        while (gridManager.grid == null || gridManager.grid.Length == 0)
            yield return null;

        if (generateRandomObstacles)
        {
            GenerateRandomObstacles();
        }
    }

    // Tạo vật cản ngẫu nhiên trên lưới
    public void GenerateRandomObstacles()
    {
        // Đảm bảo lưới đã được khởi tạo
        if (gridManager.grid == null || gridManager.grid.Length == 0)
        {
            string errorMsg = "Không thể tạo vật cản: lưới chưa được khởi tạo";
            Debug.LogError(errorMsg);
            OnError?.Invoke(errorMsg);
            return;
        }

        // Reset tất cả nút thành không phải vật cản
        foreach (var node in gridManager.grid)
        {
            if (node != null)
            {
                node.isObstacle = false;
                node.ResetVisual();
            }
        }

        // Lấy vị trí hiện tại của người chơi và quái vật từ AStarPathfinding
        AStarPathfinding pathfinder = FindObjectOfType<AStarPathfinding>();
        if (pathfinder == null || pathfinder.playerGameObject == null || pathfinder.monsterGameObject == null)
        {
            string errorMsg = "Không tìm thấy AStarPathfinding hoặc đối tượng player/monster";
            Debug.LogError(errorMsg);
            OnError?.Invoke(errorMsg);
            return;
        }

        Vector3 playerPos = pathfinder.playerGameObject.transform.position;
        Vector3 monsterPos = pathfinder.monsterGameObject.transform.position;

        Node playerNode = GetNodeFromWorldPos(playerPos);
        Node monsterNode = GetNodeFromWorldPos(monsterPos);

        if (playerNode == null || monsterNode == null)
        {
            string errorMsg = "Không thể xác định vị trí của Player hoặc Monster trên lưới";
            Debug.LogError(errorMsg);
            OnError?.Invoke(errorMsg);
            return;
        }

        // Tổng số nút trong lưới
        int totalNodes = gridManager.width * gridManager.height;
        // Số lượng vật cản cần tạo
        int obstacleCount = Mathf.RoundToInt(totalNodes * obstaclePercentage / 100f);

        // Tạo một danh sách các nút có thể được chọn làm vật cản
        List<Node> availableNodes = new List<Node>();
        foreach (var node in gridManager.grid)
        {
            if (node != null && node != playerNode && node != monsterNode)
            {
                availableNodes.Add(node);
            }
        }

        // Trộn ngẫu nhiên danh sách
        System.Random random = new System.Random();
        availableNodes = availableNodes.OrderBy(x => random.Next()).ToList();

        // Chọn số lượng nút cần thiết làm vật cản
        for (int i = 0; i < obstacleCount && i < availableNodes.Count; i++)
        {
            availableNodes[i].isObstacle = true;
            availableNodes[i].UpdateVisual();
        }

        // Kiểm tra xem có đường đi giữa người chơi và quái vật không
        bool pathExists = CheckIfPathExists(monsterNode, playerNode);
        if (!pathExists)
        {
            // Nếu không có đường đi, giảm bớt một số vật cản
            OptimizeObstacles(monsterNode, playerNode);
        }

        // Cập nhật các neighbors sau khi đã tạo vật cản
        gridManager.AssignNeighbors();

        // Thông báo đã tạo xong vật cản
        OnObstaclesGenerated?.Invoke();
    }

    // Kiểm tra xem có đường đi giữa hai nút hay không
    private bool CheckIfPathExists(Node start, Node end)
    {
        // Sử dụng BFS để kiểm tra
        HashSet<Node> visited = new HashSet<Node>();
        Queue<Node> queue = new Queue<Node>();

        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();

            if (current == end)
                return true;

            foreach (var neighbor in GetNeighbors(current))
            {
                if (!neighbor.isObstacle && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return false;
    }

    // Lấy danh sách các nút lân cận (không cần qua Node.neighbors vì đang trong quá trình tạo vật cản)
    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        int x = node.gridX;
        int y = node.gridY;

        // Kiểm tra 4 hướng (trái, phải, trên, dưới)
        TryAddNeighborToList(neighbors, x - 1, y);
        TryAddNeighborToList(neighbors, x + 1, y);
        TryAddNeighborToList(neighbors, x, y + 1);
        TryAddNeighborToList(neighbors, x, y - 1);

        return neighbors;
    }

    private void TryAddNeighborToList(List<Node> neighbors, int x, int y)
    {
        if (x >= 0 && y >= 0 && x < gridManager.width && y < gridManager.height)
        {
            neighbors.Add(gridManager.grid[x, y]);
        }
    }

    // Giảm bớt vật cản để đảm bảo có đường đi
    private void OptimizeObstacles(Node start, Node end)
    {
        // Tìm các vật cản gần người chơi và quái vật để xóa chúng
        List<Node> obstaclesToRemove = new List<Node>();

        // Thu thập tất cả các vật cản
        foreach (var node in gridManager.grid)
        {
            if (node != null && node.isObstacle)
            {
                obstaclesToRemove.Add(node);
            }
        }

        // Sắp xếp dựa trên khoảng cách với người chơi và quái vật
        obstaclesToRemove = obstaclesToRemove.OrderBy(node =>
            Vector3.Distance(node.transform.position, start.transform.position) +
            Vector3.Distance(node.transform.position, end.transform.position)).ToList();

        // Gỡ bỏ vật cản cho đến khi có đường đi
        foreach (var obstacle in obstaclesToRemove)
        {
            obstacle.isObstacle = false;
            obstacle.ResetVisual();

            if (CheckIfPathExists(start, end))
                break;
        }
    }

    // Lấy node từ vị trí thế giới
    public Node GetNodeFromWorldPos(Vector3 pos)
    {
        float spacing = gridManager.spacing;
        int x = Mathf.RoundToInt(pos.x / spacing);
        int y = Mathf.RoundToInt(pos.y / spacing);

        if (x >= 0 && y >= 0 && x < gridManager.width && y < gridManager.height)
            return gridManager.grid[x, y];

        return null;
    }
}
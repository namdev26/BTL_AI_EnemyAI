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
    public int obstaclePercentage = 20; 
    public bool generateRandomObstacles = false; 

    private GridManager gridManager;

    public event Action<string> OnError;
    public event Action OnObstaclesGenerated;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
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

    public void GenerateRandomObstacles()
    {
        if (gridManager.grid == null || gridManager.grid.Length == 0)
        {
            string errorMsg = "Không thể tạo vật cản: lưới chưa được khởi tạo";
            Debug.LogError(errorMsg);
            OnError?.Invoke(errorMsg);
            return;
        }

        foreach (var node in gridManager.grid)
        {
            if (node != null)
            {
                node.isObstacle = false;
                node.ResetVisual();
            }
        }

        AStarPathfinding pathfinder = FindObjectOfType<AStarPathfinding>();

        Vector3 playerPos = pathfinder.playerGameObject.transform.position;
        Vector3 monsterPos = pathfinder.monsterGameObject.transform.position;

        Node playerNode = GetNodeFromWorldPos(playerPos);
        Node monsterNode = GetNodeFromWorldPos(monsterPos);

        int totalNodes = gridManager.width * gridManager.height;
        int obstacleCount = Mathf.RoundToInt(totalNodes * obstaclePercentage / 100f);

        List<Node> availableNodes = new List<Node>();
        foreach (var node in gridManager.grid)
        {
            if (node != null && node != playerNode && node != monsterNode)
            {
                availableNodes.Add(node);
            }
        }
        System.Random random = new System.Random();
        availableNodes = availableNodes.OrderBy(x => random.Next()).ToList();

        for (int i = 0; i < obstacleCount && i < availableNodes.Count; i++)
        {
            availableNodes[i].isObstacle = true;
            availableNodes[i].UpdateVisual();
        }

        gridManager.AssignNeighbors();

        OnObstaclesGenerated?.Invoke();
    }

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
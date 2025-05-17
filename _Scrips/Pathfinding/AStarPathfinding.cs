using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class AStarPathfinding : MonoBehaviour
{
    [Header("Path Preview")]
    public GameObject pathEffectPrefab;

    public static AStarPathfinding Instance;

    [Header("References")]
    public GameObject monsterGameObject;
    public GameObject playerGameObject;
    public GridManager gridManager;
    public HeuristicSelector heuristicSelector;

    [Header("Nodes")]
    public Node monster;
    public Node player;
    public Node currentNode;
    public List<Node> resultPath = new List<Node>();
    public List<Node> frontierNodes = new List<Node>();
    public List<Node> exploredNodes = new List<Node>();

    public event Action<int, int, int, string> OnStateUpdated;
    public event Action<long> OnPathFound;
    public event Action<string> OnError;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        StartCoroutine(WaitUntilGridIsReady());
    }

    public void UpdatePath()
    {
        StopAllCoroutines();
        StartCoroutine(RecalculatePath());
    }

    IEnumerator RecalculatePath()
    {
        player = GetNodeFromWorldPos(playerGameObject.transform.position);
        monster = GetNodeFromWorldPos(monsterGameObject.transform.position);

        if (player == null || monster == null)
        {
            OnError?.Invoke("Không tìm được node player hoặc monster");
            yield break;
        }
        StartCoroutine(FindPathCoroutine());
    }

    IEnumerator WaitUntilGridIsReady()
    {
        while (gridManager.grid == null || gridManager.grid.Length == 0)
            yield return null;

        player = GetNodeFromWorldPos(playerGameObject.transform.position);
        monster = GetNodeFromWorldPos(monsterGameObject.transform.position);

        StartCoroutine(FindPathCoroutine());
    }

    IEnumerator ShowPathWithAnimation()
    {
        CleanupPathEffects();

        foreach (Node node in resultPath)
        {
            if (pathEffectPrefab != null)
            {
                GameObject effect = Instantiate(pathEffectPrefab, node.transform.position, Quaternion.identity);
                effect.tag = "PathEffect";
            }
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(MoveMonster());
    }
    IEnumerator MoveMonster()
    {
        foreach (Node node in resultPath)
        {
            monsterGameObject.transform.position = node.transform.position;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void CleanupPathEffects()
    {
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PathEffect"))
        {
            Destroy(dot);
        }
    }

    public IEnumerator FindPathCoroutine()
    {
        ResetNodeData();
        ResetGridVisuals();
        frontierNodes.Clear();
        exploredNodes.Clear();
        resultPath.Clear();

        monster.gCost = 0;
        monster.parent = null;
        currentNode = monster;
        frontierNodes.Add(monster);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        OnStateUpdated?.Invoke(frontierNodes.Count, exploredNodes.Count, resultPath.Count, "Bắt đầu tìm đường");

        if (monster == player)
        {
            OnStateUpdated?.Invoke(frontierNodes.Count, exploredNodes.Count, resultPath.Count, "Quái vật và người chơi đang cùng 1 vị trí rồi");
            stopwatch.Stop();
            OnPathFound?.Invoke(stopwatch.ElapsedMilliseconds);
            yield break;
        }

        while (frontierNodes.Count > 0)
        {
            currentNode = BestNodeCostFrontier();
            frontierNodes.Remove(currentNode);
            if (currentNode == null)
            {
                OnStateUpdated?.Invoke(frontierNodes.Count, exploredNodes.Count, resultPath.Count, "Không có đường đi");
                stopwatch.Stop();
                OnPathFound?.Invoke(stopwatch.ElapsedMilliseconds);
                yield break;
            }

            if (IsNodeTarget(currentNode))
            {
                BuildPath();
                stopwatch.Stop();
                OnStateUpdated?.Invoke(frontierNodes.Count, exploredNodes.Count, resultPath.Count, "Đã tìm thấy đường đi!");
                OnPathFound?.Invoke(stopwatch.ElapsedMilliseconds);
                StartCoroutine(ShowPathWithAnimation());
                yield break;
            }

            if (AddExplored(currentNode))
            {
                yield return new WaitForSeconds(0.03f);
                AddNeighborsFrontier(currentNode);
                OnStateUpdated?.Invoke(frontierNodes.Count, exploredNodes.Count, resultPath.Count, $"Sau node {currentNode.name}");
            }
        }

        OnStateUpdated?.Invoke(frontierNodes.Count, exploredNodes.Count, resultPath.Count, "Không có đường đi");
        stopwatch.Stop();
        OnPathFound?.Invoke(stopwatch.ElapsedMilliseconds);
    }

    Node BestNodeCostFrontier(bool bestSpeed = false)
    {
        if (frontierNodes.Count <= 0) return null;

        if (bestSpeed) return

        frontierNodes.OrderBy(node => node.hCost).First();

        else
        {
            frontierNodes = frontierNodes.OrderBy(node => node.fCost).ToList();
            return frontierNodes.Where(node => node.fCost == frontierNodes.First().fCost)
                .OrderBy(node => node.hCost).First();
        }
    }

    void AddNeighborsFrontier(Node node)
    {
        foreach (var neighbor in node.neighbors)
        {
            if (neighbor.isObstacle || exploredNodes.Contains(neighbor))
                continue;

            float tentativeG = node.gCost + Vector2.Distance(node.transform.position, neighbor.transform.position);

            if (!frontierNodes.Contains(neighbor))
            {
                neighbor.parent = node;
                neighbor.gCost = tentativeG;
                neighbor.hCost = heuristicSelector.CalculateHCost(neighbor.transform.position, player.transform.position);
                frontierNodes.Add(neighbor);
                neighbor.SetFrontierVisual();
            }
            else if (tentativeG < neighbor.gCost)
            {
                neighbor.parent = node;
                neighbor.gCost = tentativeG;
            }
        }
    }

    bool IsNodeTarget(Node node) => node == player;

    bool AddExplored(Node node)
    {
        if (exploredNodes.Contains(node)) return false;
        exploredNodes.Add(node);
        frontierNodes.Remove(node);
        node.SetExploredVisual();
        return true;
    }

    void BuildPath()
    {
        Node node = player;
        while (node != null)
        {
            resultPath.Insert(0, node);
            node = node.parent;
        }
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

    void ResetGridVisuals()
    {
        foreach (var node in gridManager.grid)
        {
            if (node != null)
                node.ResetVisual();
        }
    }
    void ResetNodeData()
    {
        foreach (var node in gridManager.grid)
        {
            if (node != null)
            {
                node.gCost = 0f;
                node.hCost = 0f;
                node.parent = null;
                node.ResetVisual();
            }
        }
    }

    public void CreateNewRandomObstacles()
    {
        StopAllCoroutines();
        CleanupPathEffects();
        ResetGridVisuals();

        if (gridManager != null && gridManager.ObstacleGenerator != null)
        {
            gridManager.ObstacleGenerator.OnObstaclesGenerated += OnObstaclesGenerated;
            gridManager.GenerateRandomObstacles();
        }
    }

    private void OnObstaclesGenerated()
    {
        if (gridManager.ObstacleGenerator != null)
        {
            gridManager.ObstacleGenerator.OnObstaclesGenerated -= OnObstaclesGenerated;
        }
        StartCoroutine(RecalculatePath());
    }
}
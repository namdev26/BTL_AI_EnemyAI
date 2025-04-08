using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public static AStarPathfinding Instance;

    [Header("References")]
    public GameObject monsterGameObject;
    public GameObject playerGameObject;
    public GridManager gridManager;

    [Header("Nodes")]
    public Node monster;
    public Node player;
    public Node currentNode;
    public List<Node> resultPath = new List<Node>();
    public List<Node> frontierNodes = new List<Node>();
    public List<Node> exploredNodes = new List<Node>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        monster = GetNodeFromWorldPos(monsterGameObject.transform.position);
        player = GetNodeFromWorldPos(playerGameObject.transform.position);

        if (FindPath())
            StartCoroutine(MoveMonster());
    }

    public bool FindPath()
    {
        frontierNodes.Clear();
        exploredNodes.Clear();
        resultPath.Clear();

        monster.gCost = 0;
        monster.parent = null;
        currentNode = monster;
        frontierNodes.Add(monster);

        if (monster == player)
        {
            Debug.Log("Monster is already at the player.");
            return true;
        }

        while (frontierNodes.Count > 0)
        {
            currentNode = BestNodeCostFrontier();
            if (currentNode == null)
            {
                Debug.Log("No path found.");
                return false;
            }

            if (IsNodeTarget(currentNode))
            {
                BuildPath();
                return true;
            }

            if (AddExplored(currentNode))
            {
                AddNeighborsFrontier(currentNode);
            }
        }

        Debug.Log("Path not found.");
        return false;
    }

    Node BestNodeCostFrontier()
    {
        if (frontierNodes.Count <= 0) return null;

        frontierNodes = frontierNodes.OrderBy(n => n.fCost).ThenBy(n => n.hCost).ToList();
        return frontierNodes.First();
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
                frontierNodes.Add(neighbor);
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
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);
        if (x >= 0 && y >= 0 && x < gridManager.width && y < gridManager.height)
            return gridManager.grid[x, y];
        return null;
    }

    IEnumerator MoveMonster()
    {
        foreach (Node node in resultPath)
        {
            monsterGameObject.transform.position = node.transform.position;
            yield return new WaitForSeconds(0.2f);
        }
    }
}

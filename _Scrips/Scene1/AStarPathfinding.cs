using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Header("Nodes")]
    public Node monster;
    public Node player;
    public Node currentNode;
    public List<Node> resultPath = new List<Node>(); // lưu node kết quả đường đi
    public List<Node> frontierNodes = new List<Node>(); // lưu các node trong hàng đợi
    public List<Node> exploredNodes = new List<Node>(); // lưu các node đã duyệt

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
        StopAllCoroutines(); // Dừng các coroutine cũ
        StartCoroutine(RecalculatePath());
    }

    IEnumerator RecalculatePath()
    {
        player = GetNodeFromWorldPos(playerGameObject.transform.position);
        monster = GetNodeFromWorldPos(monsterGameObject.transform.position);

        if (player == null || monster == null)
        {
            Debug.LogError("Không tìm được node player hoặc monster");
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

        if (player == null || monster == null)
        {
            Debug.LogError("Player hoặc Monster node là null. Kiểm tra vị trí hoặc GridManager.");
            yield break;
        }
        StartCoroutine(FindPathCoroutine());
    }

    IEnumerator ShowPathWithAnimation()
    {
        CleanupPathEffects(); // xóa dấu chân

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

        //CleanupPathEffects();
    }

    void CleanupPathEffects()
    {
        foreach (GameObject dot in GameObject.FindGameObjectsWithTag("PathEffect"))
        {
            Destroy(dot);
        }
    }

    public IEnumerator FindPathCoroutine()
    {
        ResetGridVisuals();

        frontierNodes.Clear();
        exploredNodes.Clear();
        resultPath.Clear();

        monster.gCost = 0;
        monster.parent = null;
        currentNode = monster;
        frontierNodes.Add(monster);

        if (monster == player)
        {
            Debug.Log("Quái vật và người chơi đang cùng 1 vị trí rồi.");
            yield break;
        }

        while (frontierNodes.Count > 0)
        {
            currentNode = BestNodeCostFrontier();
            frontierNodes.Remove(currentNode);
            if (currentNode == null)
            {
                Debug.Log("Không có đường đi.");
                yield break;
            }

            if (IsNodeTarget(currentNode)) // đã đến đích
            {
                BuildPath();
                StartCoroutine(ShowPathWithAnimation());
                yield break;
            }

            if (AddExplored(currentNode))
            {
                //currentNode.SetExploredVisual(Color.cyan); // tô màu node duyệt qua
                yield return new WaitForSeconds(0.03f);     // delay giữa các node
                AddNeighborsFrontier(currentNode);
            }
        }

        Debug.Log("Không có đường đi.");
    }


    Node BestNodeCostFrontier(bool bestSpeed = false)
    {
        if (frontierNodes.Count <= 0) return null;

        if (bestSpeed) return frontierNodes.OrderBy(node => node.hCost).First();
        else
        {
            frontierNodes = frontierNodes.OrderBy(node => node.fCost).ToList();
            return frontierNodes.Where(node => node.fCost == frontierNodes.First().fCost)
                .OrderBy(node => node.hCost).First();
        }
    }

    void AddNeighborsFrontier(Node node)
    {
        foreach (var neighbor in node.neighbors) // duyệt các lớp hàng xóm với node hàng xóm hiện tại 
        {
            if (neighbor.isObstacle || exploredNodes.Contains(neighbor)) // và đã duyệt rồi
                continue;

            float tentativeG = node.gCost + Vector2.Distance(node.transform.position, neighbor.transform.position);

            if (!frontierNodes.Contains(neighbor))
            {
                neighbor.parent = node;
                neighbor.gCost = tentativeG;
                neighbor.hCost = Vector2.Distance(neighbor.transform.position, player.transform.position); // tính hCost
                frontierNodes.Add(neighbor);

                // Tô màu node biên
                neighbor.SetFrontierVisual();
            }
            else if (tentativeG < neighbor.gCost)
            {
                neighbor.parent = node;
                neighbor.gCost = tentativeG;
                // không cần gán lại hCost vì vị trí player không đổi
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
            resultPath.Insert(0, node); // đảo ngược đường đi từ player
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
}

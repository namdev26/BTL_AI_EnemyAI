using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PathfindingDebugger : MonoBehaviour
{
    [Header("UI References")]
    // Nếu dùng TextMeshPro:
    public TMP_Text frontierText;
    public TMP_Text exploredText;
    public TMP_Text resultPathText;
    public TMP_Text executionTimeText;

    [Header("Pathfinding Reference")]
    public AStarPathfinding pathfinding;

    private void Awake()
    {
        if (pathfinding == null)
        {
            Debug.LogError("Pathfinding reference is not assigned in PathfindingDebugger!");
            return;
        }

        // Đăng ký các sự kiện từ AStarPathfinding
        pathfinding.OnStateUpdated += UpdateUI;
        pathfinding.OnPathFound += UpdateExecutionTime;
        pathfinding.OnError += ShowError;
    }

    private void UpdateUI(int frontierCount, int exploredCount, int pathCount, string status)
    {
        if (frontierText != null)
            frontierText.text = $"Frontier Nodes: {frontierCount}";
        if (exploredText != null)
            exploredText.text = $"Explored Nodes: {exploredCount}";
        if (resultPathText != null)
            resultPathText.text = $"Result Path: {pathCount}";
        //if (executionTimeText != null)
        //    executionTimeText.text = $"Status: {status}";
    }

    private void UpdateExecutionTime(long milliseconds)
    {
        if (executionTimeText != null)
            executionTimeText.text = $"Execution Time: {milliseconds} ms";
    }

    private void ShowError(string errorMessage)
    {
        if (executionTimeText != null)
            executionTimeText.text = $"Error: {errorMessage}";
    }
}
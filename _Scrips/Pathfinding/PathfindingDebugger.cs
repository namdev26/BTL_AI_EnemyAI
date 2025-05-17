using UnityEngine;
using TMPro;

public class PathfindingDebugger : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text frontierText;
    public TMP_Text exploredText;
    public TMP_Text resultPathText;
    public TMP_Text executionTimeText;

    [Header("Pathfinding References")]
    public AStarPathfinding pathfinding;

    private void Awake()
    {
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
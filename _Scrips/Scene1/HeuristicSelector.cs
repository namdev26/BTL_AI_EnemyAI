using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeuristicSelector : MonoBehaviour
{
    public enum HeuristicType
    {
        Euclidean,
        Manhattan,
        Diagonal
    }

    public static HeuristicSelector Instance;
    public GridManager gridManager;


    [Header("References")]
    public AStarPathfinding pathfinding;

    [Header("UI")]
    public Button euclideanButton;
    public Button manhattanButton;
    public Button diagonalButton;
    public TextMeshProUGUI currentHeuristicText;

    private HeuristicType currentHeuristic = HeuristicType.Euclidean;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (euclideanButton != null)
        {
            euclideanButton.onClick.AddListener(() => ChangeHeuristic(HeuristicType.Euclidean));
        }

        if (manhattanButton != null)
        {
            manhattanButton.onClick.AddListener(() => ChangeHeuristic(HeuristicType.Manhattan));
        }

        if (diagonalButton != null)
        {
            diagonalButton.onClick.AddListener(() => ChangeHeuristic(HeuristicType.Diagonal));
        }

        UpdateHeuristicText();
    }

    public void ChangeHeuristic(HeuristicType type)
    {
        currentHeuristic = type;
        UpdateHeuristicText();

        if (pathfinding != null)
        {
            pathfinding.CleanupPathEffects();
            pathfinding.UpdatePath();
        }
    }

    private void UpdateHeuristicText()
    {
        if (currentHeuristicText != null)
        {
            currentHeuristicText.text = "Current: " + currentHeuristic.ToString();
        }
    }

    public float CalculateHCost(Vector3 start, Vector3 end)
    {
        switch (currentHeuristic)
        {
            case HeuristicType.Euclidean:
                return Mathf.Sqrt(Mathf.Pow(start.x - end.x, 2) + Mathf.Pow(start.y - end.y, 2));

            case HeuristicType.Manhattan:
                return Mathf.Abs(start.x - end.x) + Mathf.Abs(start.y - end.y);

            case HeuristicType.Diagonal:
                float dx = Mathf.Abs(start.x - end.x);
                float dy = Mathf.Abs(start.y - end.y);
                float D = gridManager.spacing;
                float D2 = Mathf.Sqrt(gridManager.spacing + gridManager.spacing);
                return D * (dx + dy) + (D2 - 2 * D) * Mathf.Min(dx, dy);

            default:
                Debug.Log("Lỗi");
                return -1;
        }
    }
}
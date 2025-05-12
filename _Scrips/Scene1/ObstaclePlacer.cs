using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    public GridManager gridManager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click trái
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPos.z = 0;

            Node node = AStarPathfinding.Instance.GetNodeFromWorldPos(worldPos);
            if (node != null)
            {
                node.isObstacle = !node.isObstacle;
                node.UpdateVisual(); // Hàm này bạn dùng để đổi màu node
            }
        }
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Node lastNode;

    void Update()
    {
        Node currentNode = AStarPathfinding.Instance.GetNodeFromWorldPos(transform.position);
        if (currentNode != lastNode)
        {
            lastNode = currentNode;
            AStarPathfinding.Instance.UpdatePath();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float gCost;
    public float hCost;

    public float fCost => gCost + hCost;

    public Node parent;
    public List<Node> neighbors = new List<Node>();

    public bool isObstacle;

    private void OnDrawGizmos()
    {
        if (isObstacle)
            Gizmos.color = Color.black;
        else
            Gizmos.color = Color.white;

        Gizmos.DrawCube(transform.position, Vector3.one * 0.9f);
    }
}

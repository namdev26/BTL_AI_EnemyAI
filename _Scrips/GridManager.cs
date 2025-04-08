using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject nodePrefab;
    public int width = 10;
    public int height = 10;
    public float spacing = 1f;
    public Node[,] grid;

    void Start()
    {
        GenerateGrid();
        AssignNeighbors();
    }

    void GenerateGrid()
    {
        grid = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject nodeObj = Instantiate(nodePrefab, new Vector3(x * spacing, y * spacing, 0), Quaternion.identity);
                nodeObj.name = $"Node ({x},{y})";
                nodeObj.transform.parent = transform;

                Node node = nodeObj.GetComponent<Node>();
                grid[x, y] = node;
            }
        }
    }

    void AssignNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = grid[x, y];
                node.neighbors.Clear();

                // 4 hướng cơ bản (lên, xuống, trái, phải)
                TryAddNeighbor(node, x - 1, y); // trái
                TryAddNeighbor(node, x + 1, y); // phải
                TryAddNeighbor(node, x, y + 1); // lên
                TryAddNeighbor(node, x, y - 1); // xuống
            }
        }
    }

    void TryAddNeighbor(Node node, int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            node.neighbors.Add(grid[x, y]);
        }
    }
}

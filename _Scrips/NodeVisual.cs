using UnityEngine;

public class NodeVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer; // Renderer cho lớp nền cỏ
    [SerializeField] private SpriteRenderer obstacleRenderer; // Renderer cho chướng ngại vật, frontier, explored
    [SerializeField] private Sprite grassSprite; // Sprite cho cỏ
    [SerializeField] private Sprite obstacleSprite; // Sprite cho chướng ngại vật
    [SerializeField] private Sprite frontierSprite; // Sprite cho frontier nodes
    [SerializeField] private Sprite exploredSprite; // Sprite cho explored nodes
    public void Init()
    {
        // Khởi tạo lớp nền cỏ
        if (backgroundRenderer == null) backgroundRenderer = GetComponent<SpriteRenderer>();
        if (backgroundRenderer != null && grassSprite != null)
        {
            backgroundRenderer.sprite = grassSprite;
            backgroundRenderer.sortingOrder = 0; // Đảm bảo lớp cỏ ở dưới
        }

        // Khởi tạo lớp chướng ngại vật/frontier/explored
        if (obstacleRenderer == null)
        {
            GameObject overlayObj = new GameObject("OverlayRenderer");
            overlayObj.transform.SetParent(transform, false);
            overlayObj.transform.localPosition = Vector3.zero;
            obstacleRenderer = overlayObj.AddComponent<SpriteRenderer>();
            obstacleRenderer.sortingOrder = 1; // Đảm bảo lớp trên ở trên lớp cỏ
        }

        // Ẩn lớp trên ban đầu
        SetOverlayVisible(false);
    }

    public void SetAsObstacle()
    {
        if (obstacleRenderer != null && obstacleSprite != null)
        {
            obstacleRenderer.sprite = obstacleSprite;
            obstacleRenderer.enabled = true; // Hiển thị chướng ngại vật
        }
    }

    public void SetAsFrontier()
    {
        if (obstacleRenderer != null && frontierSprite != null)
        {
            obstacleRenderer.sprite = frontierSprite;
            obstacleRenderer.enabled = true; // Hiển thị frontier
        }
    }

    public void SetAsExplored()
    {
        if (obstacleRenderer != null && exploredSprite != null)
        {
            obstacleRenderer.sprite = exploredSprite;
            obstacleRenderer.enabled = true; // Hiển thị explored
        }
    }

    public void ResetVisual()
    {
        SetOverlayVisible(false); // Ẩn lớp trên, giữ lớp cỏ
    }

    private void SetOverlayVisible(bool isVisible)
    {
        if (obstacleRenderer != null)
        {
            obstacleRenderer.enabled = isVisible;
        }
    }
}

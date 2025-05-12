using UnityEngine;
using System.Collections.Generic;

public class NodeVisual : MonoBehaviour
{
    [System.Serializable]
    public class SpriteEntry
    {
        public Sprite sprite;
        [Range(0f, 1f)]
        public float probability = 1f;
    }

    [SerializeField] private SpriteRenderer backgroundRenderer; 
    [SerializeField] private SpriteRenderer obstacleRenderer; 

    [SerializeField] private List<SpriteEntry> grassSprites = new List<SpriteEntry>(); 
    [SerializeField] private List<SpriteEntry> obstacleSprites = new List<SpriteEntry>(); 
    [SerializeField] private List<SpriteEntry> frontierSprites = new List<SpriteEntry>(); 
    [SerializeField] private List<SpriteEntry> exploredSprites = new List<SpriteEntry>(); 

    public void Init()
    {
        if (backgroundRenderer == null) backgroundRenderer = GetComponent<SpriteRenderer>();

        if (backgroundRenderer != null && grassSprites.Count > 0)
        {
            backgroundRenderer.sprite = GetRandomSpriteFromList(grassSprites);
            backgroundRenderer.sortingOrder = 0;
        }

        if (obstacleRenderer == null)
        {
            GameObject overlayObj = new GameObject("OverlayRenderer");
            overlayObj.transform.SetParent(transform, false);
            overlayObj.transform.localPosition = Vector3.zero;
            obstacleRenderer = overlayObj.AddComponent<SpriteRenderer>();
            obstacleRenderer.sortingOrder = 1;
        }

        SetOverlayVisible(false);
    }

    private Sprite GetRandomSpriteFromList(List<SpriteEntry> spriteList)
    {
        if (spriteList == null || spriteList.Count == 0)
            return null;

        float totalProbability = 0f;
        foreach (var entry in spriteList)
        {
            totalProbability += entry.probability;
        }

        float randomValue = Random.Range(0f, totalProbability);
        float cumulativeProbability = 0f;

        foreach (var entry in spriteList)
        {
            cumulativeProbability += entry.probability;
            if (randomValue <= cumulativeProbability)
            {
                return entry.sprite;
            }
        }

        return spriteList[0].sprite;
    }

    public void SetAsObstacle()
    {
        if (obstacleRenderer != null && obstacleSprites.Count > 0)
        {
            obstacleRenderer.sprite = GetRandomSpriteFromList(obstacleSprites);
            obstacleRenderer.enabled = true;
        }
    }

    public void SetAsFrontier()
    {
        if (obstacleRenderer != null && frontierSprites.Count > 0)
        {
            obstacleRenderer.sprite = GetRandomSpriteFromList(frontierSprites);
            obstacleRenderer.enabled = true; 
        }
    }

    public void SetAsExplored()
    {
        if (obstacleRenderer != null && exploredSprites.Count > 0)
        {
            obstacleRenderer.sprite = GetRandomSpriteFromList(exploredSprites);
            obstacleRenderer.enabled = true;
        }
    }

    public void ResetVisual()
    {
        SetOverlayVisible(false);
    }

    private void SetOverlayVisible(bool isVisible)
    {
        if (obstacleRenderer != null)
        {
            obstacleRenderer.enabled = isVisible;
        }
    }
}
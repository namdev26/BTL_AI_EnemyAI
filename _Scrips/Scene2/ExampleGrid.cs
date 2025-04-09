﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class ExampleGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int _size;
    [SerializeField] private Vector2 _gap;
    [SerializeField, Range(0, 0.8f)] private float _skipAmount = 0.1f;
    [SerializeField, Range(0, 1)] private float _forestAmount = 0.3f;
    [SerializeField] private ScriptableGridConfig _config;

    private bool _requiresGeneration = true;
    private Camera _cam;
    private Grid _grid;

    private Vector3 _cameraPositionTarget;
    private float _cameraSizeTarget;
    private Vector3 _moveVel;
    private float _cameraSizeVel;

    private Vector2 _currentGap;
    private Vector2 _gapVel;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
        _cam = Camera.main;
        _currentGap = _gap;
    }

    private void OnValidate() => _requiresGeneration = true;

    private void LateUpdate()
    {
        if (Vector2.Distance(_currentGap, _gap) > 0.01f)
        {
            _currentGap = Vector2.SmoothDamp(_currentGap, _gap, ref _gapVel, 0.1f);
            _requiresGeneration = true;
        }

        if (_requiresGeneration) Generate();

        _cam.transform.position = Vector3.SmoothDamp(_cam.transform.position, _cameraPositionTarget, ref _moveVel, 0.5f);
        _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, _cameraSizeTarget, ref _cameraSizeVel, 0.5f);
    }

    private void Generate()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        _grid.cellLayout = GridLayout.CellLayout.Rectangle;
        _grid.cellSize = _config.CellSize;
        _grid.cellGap = _currentGap;
        _grid.cellSwizzle = _config.GridSwizzle;

        var coordinates = new List<Vector3Int>();

        for (int x = 0; x < _size.x; x++)
            for (int y = 0; y < _size.y; y++)
                coordinates.Add(new Vector3Int(x, y));

        var bounds = new Bounds();
        var skipCount = Mathf.FloorToInt(coordinates.Count * _skipAmount);
        var forestCount = Mathf.FloorToInt(coordinates.Count * _forestAmount);
        var index = 0;
        var rand = new Random(420);

        foreach (var coordinate in coordinates.OrderBy(_ => rand.Next()).Take(coordinates.Count - skipCount))
        {
            var isForest = index++ < forestCount;
            var prefab = isForest ? _config.ForestPrefab : _config.GrassPrefab;
            var position = _grid.GetCellCenterWorld(coordinate);
            var spawned = Instantiate(prefab, position, Quaternion.identity, transform);
            spawned.Init(coordinate);
            bounds.Encapsulate(position);
        }

        SetCamera(bounds);
        _requiresGeneration = false;
    }

    private void SetCamera(Bounds bounds)
    {
        bounds.Expand(2);
        var vertical = bounds.size.y;
        var horizontal = bounds.size.x * _cam.pixelHeight / _cam.pixelWidth;

        _cameraPositionTarget = bounds.center + Vector3.back;
        _cameraSizeTarget = Mathf.Max(horizontal, vertical) * 0.5f;
    }
}

[CreateAssetMenu]
public class ScriptableGridConfig : ScriptableObject
{
    [Header("Grid Config - Rectangle Only")]
    public GridTileBase GrassPrefab, ForestPrefab;
    public Vector3 CellSize = Vector3.one;
    public GridLayout.CellSwizzle GridSwizzle = GridLayout.CellSwizzle.XYZ;
}

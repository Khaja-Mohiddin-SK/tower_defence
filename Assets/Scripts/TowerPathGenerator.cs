using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPathGenerator : MonoBehaviour
{
    [Header("Scene References")]
    public Transform pathStartPoint;
    public Transform pathEndPoint;
    public GameObject pathTilePrefab;
    public Renderer planeRenderer;

    [Header("Enemy")]
    public GameObject enemyPrefab;

    [Header("Grid Settings")]
    public float tileSize = 1f;
    public float tileHeight = 0.05f;

    [Header("Path Shape Settings")]
    public int startStraightTiles = 2;
    public Vector2Int checkpointCountRange = new Vector2Int(1, 3);

    [Header("Generated Path")]
    public List<Vector3> pathPoints = new List<Vector3>();

    [Header("Enemy Wave Settings")]
    public int enemyCount = 15;
    public float spawnInterval = 1.5f;

    private int minGridX;
    private int maxGridX;
    private int minGridZ;
    private int maxGridZ;

    private HashSet<Vector2Int> usedCells = new HashSet<Vector2Int>();

    public bool IsPointOnPath(Vector3 worldPosition)
    {
        float checkDistance = tileSize * 0.6f;
        foreach (Vector3 pathPoint in pathPoints)
        {
            float distance = Vector3.Distance(
                new Vector3(worldPosition.x, 0f, worldPosition.z),
                new Vector3(pathPoint.x, 0f, pathPoint.z)
            );
            if (distance < checkDistance)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsNearPath(Vector3 worldPosition, float blockedDistance)
    {
        foreach (Vector3 pathPoint in pathPoints)
        {
            Vector3 flatWorldPosition = new Vector3(worldPosition.x,0f,worldPosition.z);
            Vector3 flatPathPoint = new Vector3(pathPoint.x,0f,pathPoint.z);
            float distance = Vector3.Distance(flatWorldPosition, flatPathPoint);
            if (distance <= blockedDistance)
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        CalculateGridBoundsFromPlane();
        GeneratePath();
        SpawnPathTiles();
        StartCoroutine(SpawnEnemyWave());
    }

    void CalculateGridBoundsFromPlane()
    {
        Bounds bounds = planeRenderer.bounds;

        minGridX = Mathf.CeilToInt(bounds.min.x / tileSize) + 1;
        maxGridX = Mathf.FloorToInt(bounds.max.x / tileSize) - 1;

        minGridZ = Mathf.CeilToInt(bounds.min.z / tileSize) + 1;
        maxGridZ = Mathf.FloorToInt(bounds.max.z / tileSize) - 1;
    }

    void GeneratePath()
    {
        pathPoints.Clear();
        usedCells.Clear();

        Vector2Int start = ClampToGrid(WorldToGrid(pathStartPoint.position));
        Vector2Int end = ClampToGrid(WorldToGrid(pathEndPoint.position));

        Vector2Int current = start;
        AddPoint(current);

        // Force the path to move away from the enemy tower first.
        int xDirection = current.x < end.x ? 1 : -1;

        for (int i = 0; i < startStraightTiles; i++)
        {
            Vector2Int next = current;
            next.x += xDirection;
            next = ClampToGrid(next);

            if (next == current)
                break;

            current = next;
            AddPoint(current);
        }

        // Create random middle checkpoints.
        List<Vector2Int> targets = new List<Vector2Int>();

        int checkpointCount = Random.Range(
            checkpointCountRange.x,
            checkpointCountRange.y + 1
        );

        for (int i = 0; i < checkpointCount; i++)
        {
            float t = (i + 1f) / (checkpointCount + 1f);

            int checkpointX = Mathf.RoundToInt(Mathf.Lerp(current.x, end.x, t));
            int checkpointZ = Random.Range(minGridZ + 1, maxGridZ);

            Vector2Int checkpoint = new Vector2Int(checkpointX, checkpointZ);
            checkpoint = ClampToGrid(checkpoint);

            targets.Add(checkpoint);
        }

        targets.Add(end);

        foreach (Vector2Int target in targets)
        {
            current = GenerateSegment(current, target);
        }
    }

    Vector2Int GenerateSegment(Vector2Int current, Vector2Int target)
    {
        int safetyCounter = 0;
        while (current != target && safetyCounter < 1000)
        {
            safetyCounter++;
            List<Vector2Int> possibleMoves = new List<Vector2Int>();
            
            if (current.x != target.x)
            {
                Vector2Int moveX = current;
                moveX.x += current.x < target.x ? 1 : -1;
                moveX = ClampToGrid(moveX);
                if (!usedCells.Contains(moveX) || moveX == target)
                {
                    possibleMoves.Add(moveX);
                }
            }
            
            if (current.y != target.y)
            {
                Vector2Int moveZ = current;
                moveZ.y += current.y < target.y ? 1 : -1;
                moveZ = ClampToGrid(moveZ);
                if (!usedCells.Contains(moveZ) || moveZ == target)
                {
                    possibleMoves.Add(moveZ);
                }
            }
            
            if (possibleMoves.Count == 0)
            {
                Debug.LogWarning("Path got blocked. Stopping segment.");
                break;
            }
            Vector2Int next = possibleMoves[Random.Range(0, possibleMoves.Count)];
            if (next == current)
                break;
            current = next;
            AddPoint(current);
        }
        return current;
    }
    void AddPoint(Vector2Int gridPosition)
    {
        if (usedCells.Contains(gridPosition))
            return;

        usedCells.Add(gridPosition);
        pathPoints.Add(GridToWorld(gridPosition));
    }

    void SpawnPathTiles()
    {
        foreach (Vector3 point in pathPoints)
        {
            Instantiate(pathTilePrefab, point, Quaternion.identity);
        }
    }

    IEnumerator SpawnEnemyWave()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            SpawnSingleEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnSingleEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab is not assigned.");
            return;
        }
        if (pathPoints == null || pathPoints.Count == 0)
        {
            Debug.LogError("No path points found.");
            return;
        }
        
        GameObject enemyObject = Instantiate(
            enemyPrefab,
            pathPoints[0],
            Quaternion.identity
        );
            
        EnemyMovement enemyMovement = enemyObject.GetComponent<EnemyMovement>();
        
        if (enemyMovement == null)
        {
            Debug.LogError("Enemy prefab does not have EnemyMovement script.");
            return;
        }
        enemyMovement.SetPath(new List<Vector3>(pathPoints));
    }
    Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / tileSize);
        int z = Mathf.RoundToInt(worldPosition.z / tileSize);

        return new Vector2Int(x, z);
    }

    Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * tileSize,
            tileHeight,
            gridPosition.y * tileSize
        );
    }

    Vector2Int ClampToGrid(Vector2Int gridPosition)
    {
        gridPosition.x = Mathf.Clamp(gridPosition.x, minGridX, maxGridX);
        gridPosition.y = Mathf.Clamp(gridPosition.y, minGridZ, maxGridZ);

        return gridPosition;
    }
}
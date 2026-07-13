using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 3f;
    public float reachDistance = 0.05f;
    public float heightOffset = 0.35f;

    private List<Vector3> pathPoints;
    private int targetIndex = 1;
    public int damageToProtectTower = 1;

    public void SetPath(List<Vector3> newPath)
    {
        pathPoints = newPath;

        if (pathPoints == null || pathPoints.Count == 0)
            return;

        transform.position = GetPathPosition(0);
        targetIndex = 1;
    }

    void Update()
    {
        if (pathPoints == null || pathPoints.Count == 0)
            return;

        if (targetIndex >= pathPoints.Count)
            return;

        Vector3 targetPosition = GetPathPosition(targetIndex);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < reachDistance)
        {
            targetIndex++;

            if (targetIndex >= pathPoints.Count)
            {
                ReachEnd();
            }
        }
    }

    Vector3 GetPathPosition(int index)
    {
        Vector3 point = pathPoints[index];
        return new Vector3(point.x, point.y + heightOffset, point.z);
    }

    void ReachEnd()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DamageProtectTower(damageToProtectTower);
        }
        
        Destroy(gameObject);
    }
}
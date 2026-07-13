using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlacementPreviewRing : MonoBehaviour
{
    public float radius = 0.7f;
    public int segments = 64;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawRing();
    }

    void DrawRing()
    {
        lineRenderer.positionCount = segments;
        lineRenderer.loop = true;
        lineRenderer.useWorldSpace = false;

        for (int i = 0; i < segments; i++)
        {
            float angle = ((float)i / segments) * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, 0.05f, z));
        }
    }
}
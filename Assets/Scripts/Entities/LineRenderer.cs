using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    private static Material cachedMaterial;

    public static void Draw(Vector2 start, Vector2 end)
    {
        if (cachedMaterial == null)
        {
            cachedMaterial = new Material(Shader.Find("Sprites/Default"));
        }

        GameObject go = new GameObject("LightningLine");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = cachedMaterial;
        lr.sortingOrder = 100;

        lr.startWidth = 12f;
        lr.endWidth = 5f;
        lr.startColor = new Color(0.5f, 1f, 1f, 1f);
        lr.endColor = new Color(0.2f, 0.5f, 1f, 0.8f);

        int pointCount = 8;
        lr.positionCount = pointCount;
        Vector3[] points = new Vector3[pointCount];

        points[0] = start;
        points[pointCount - 1] = end;

        Vector2 dir = (end - start).normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        for (int i = 1; i < pointCount - 1; i++)
        {
            float t = i / (float)(pointCount - 1);
            Vector2 midPoint = Vector2.Lerp(start, end, t);

            float noise = Random.Range(-25f, 25f);
            points[i] = midPoint + perp * noise;
        }

        lr.SetPositions(points);

        GameObject innerGo = new GameObject("LightningInner");
        LineRenderer innerLr = innerGo.AddComponent<LineRenderer>();
        innerLr.material = cachedMaterial;
        innerLr.sortingOrder = 101;
        innerLr.startWidth = 2f;
        innerLr.endWidth = 0.5f;
        innerLr.startColor = Color.white;
        innerLr.endColor = Color.white;
        innerLr.positionCount = pointCount;
        innerLr.SetPositions(points);

        Destroy(go, 0.08f);
        Destroy(innerGo, 0.08f);
    }
}
using UnityEngine;

public class VisualParticle : MonoBehaviour
{
    public Vector2 velocity;
    public float friction = 0.9f;
    public float life = 0.5f;
    public float startScale = 10f;
    public float endScale = 0f;
    public Color startColor = Color.white;
    public Color endColor = new Color(1, 1, 1, 0);

    private SpriteRenderer sr;
    private float maxLife;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        maxLife = life;
        sr.color = startColor;
        transform.localScale = Vector3.one * startScale;
    }

    void Update()
    {
        // 移动与减速
        transform.position += (Vector3)(velocity * Time.deltaTime);
        velocity *= Mathf.Pow(friction, Time.deltaTime * 60f);

        // 寿命与渐变
        life -= Time.deltaTime;
        float t = 1f - Mathf.Clamp01(life / maxLife);

        // 缩放过渡
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);

        // 颜色过渡
        if (sr != null) sr.color = Color.Lerp(startColor, endColor, t);

        if (life <= 0f) Destroy(gameObject);
    }
}
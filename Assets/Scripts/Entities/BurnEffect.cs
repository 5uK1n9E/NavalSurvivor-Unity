using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    private Enemy enemy;
    private float spawnTimer = 0f;

    void Start() { enemy = GetComponent<Enemy>(); }

    void Update()
    {
        if (enemy == null || !enemy.alive || enemy.burnTimer <= 0f) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = 0.06f;

            GameObject fire = new GameObject("FireParticle");
            fire.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * enemy.radius);
            SpriteRenderer sr = fire.AddComponent<SpriteRenderer>();

            sr.sprite = Player.GetCircleSprite();
            sr.sortingOrder = 50;

            VisualParticle vp = fire.AddComponent<VisualParticle>();
            vp.velocity = new Vector2(Random.Range(-20f, 20f), Random.Range(50f, 100f));
            vp.life = Random.Range(0.3f, 0.6f);
            vp.startScale = Random.Range(10f, 20f);
            vp.endScale = 0f;
            vp.startColor = new Color(1f, Random.Range(0.4f, 0.8f), 0f, 1f);
            vp.endColor = new Color(1f, 0.1f, 0f, 0f);
        }
    }
}
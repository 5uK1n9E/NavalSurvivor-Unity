using UnityEngine;
using System.Collections.Generic;

public class MissileStrike : MonoBehaviour
{
    public float radius = 80f;
    public float countdown = 2f;   // 预警时间
    public float explodeTime = 3f; // 爆炸持续时间
    public int damageToPlayer = 25;
    public int damageToEnemy = 10;

    private float timer;
    private bool isExploding = false;
    private SpriteRenderer sr;
    private GameObject textObj;
    private TextMesh textMesh;
    private float damageTickTimer = 0f;

    void Start()
    {
        // 红色半透明预警圈
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = Player.GetCircleSprite();
        sr.color = new Color(1f, 0f, 0f, 0.3f); // 红色半透明
        sr.sortingOrder = 5;

        // 缩放适配半径
        transform.localScale = Vector3.one * (radius * 2.2f);

        // 倒计时文字
        textObj = new GameObject("CountdownText");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 0, -0.1f);
        textMesh = textObj.AddComponent<TextMesh>();
        textMesh.fontSize = 64;
        textMesh.characterSize = 0.2f; // 调整 TextMesh 的显示大小
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = new Color(1f, 0.2f, 0.2f, 1f);

        timer = countdown;
    }

    void Update()
    {
        if (!isExploding)
        {
            // 倒计时阶段
            timer -= Time.deltaTime;

            // 预警圈脉动动画
            float pulse = 1f + Mathf.Sin(Time.time * 15f) * 0.05f;
            transform.localScale = Vector3.one * (radius * 2.2f) * pulse;

            textMesh.text = Mathf.CeilToInt(Mathf.Max(0, timer)).ToString();

            if (timer <= 0f)
            {
                StartExplosion();
            }
        }
        else
        {
            // 爆炸阶段
            timer -= Time.deltaTime;

            // 爆炸圈视觉反馈
            sr.color = new Color(1f, 0.5f, 0f, Mathf.Clamp01(timer / explodeTime)); // 从橙红变成透明
            transform.localScale = Vector3.one * (radius * 2.5f) * (1f + (explodeTime - timer) * 0.2f);

            // 每 0.15 秒结算一次范围伤害
            damageTickTimer -= Time.deltaTime;
            if (damageTickTimer <= 0f)
            {
                damageTickTimer = 0.15f;
                ApplyAreaDamage();
            }

            if (timer <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    void StartExplosion()
    {
        isExploding = true;
        timer = explodeTime;
        textObj.SetActive(false);

        // 播放爆炸音效
        if (AudioManager.I != null) AudioManager.I.PlayBoom();

        // 生成爆炸粒子
        for (int i = 0; i < 20; i++)
        {
            GameObject p = new GameObject("ExplosionSpark");
            p.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * radius);
            SpriteRenderer sparkSr = p.AddComponent<SpriteRenderer>();
            sparkSr.sprite = Player.GetCircleSprite();
            sparkSr.sortingOrder = 20;

            VisualParticle vp = p.AddComponent<VisualParticle>();
            vp.velocity = Random.insideUnitCircle * 200f;
            vp.startScale = 20f;
            vp.endScale = 50f;
            vp.life = 0.5f;
            vp.startColor = new Color(1f, 0.4f, 0f, 1f);
            vp.endColor = new Color(1f, 0f, 0f, 0f);
        }
    }

    void ApplyAreaDamage()
    {
        Player p = GameManager.I.player;
        if (p == null) return;

        // 对玩家造成伤害（是否开启轰炸协议，爆破偏转）
        if (p.bombardmentModeStack == 0)
        {
            if (Vector2.Distance(transform.position, p.transform.position) < radius + p.radius)
            {
                float pMult = 1f;
                if (p.missileShuntStack > 0)
                {
                    pMult = Mathf.Max(0.1f, 1f - p.missileShuntStack * 0.15f);
                }
                p.Hurt(damageToPlayer * pMult);
            }
        }

        // 对敌人造成伤害（受强化轰炸、爆破偏转、轰炸协议加成）
        float eMult = 1f;
        if (p.missileShuntStack > 0) eMult *= (1f + p.missileShuntStack * 0.2f);
        if (p.bombardmentStack > 0) eMult *= (1f + p.bombardmentStack * 0.3f);
        if (p.bombardmentModeStack > 0) eMult *= 3.0f; // 轰炸协议 3 倍伤害

        foreach (var e in new List<Enemy>(Enemy.All))
        {
            if (Vector2.Distance(transform.position, e.transform.position) < radius + e.radius)
            {
                e.Hurt(Mathf.RoundToInt(damageToEnemy * eMult));
            }
        }
    }
}
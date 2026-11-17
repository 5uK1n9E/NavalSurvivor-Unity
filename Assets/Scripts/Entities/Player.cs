// Player.cs
using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("耐久 / 分数（运行时只读）")]
    public float dur;
    public float invincibleTimer = 0f;
    public int kills = 0;
    public int score = 0;

    [Header("碰撞半径")]
    public float radius = 36f;

    [Header("贴图朝向偏移（度）")]
    public float spriteForwardOffsetDegrees = 90f;

    [Header("燃料")]
    public float fuel;

    [Header("临时道具状态")]
    public float strengthenTimer = 0f;   // 双发持续时间
    public float fireRateBoostTimer = 0f; // 射速提升持续时间
    public float jammingTimer = 0f; // 减速时间

    [Header("强化属性")]
    public int maxDurBonus = 0;
    public int dmgBonus = 0;
    public float fireRateBonus = 0f;
    public float fuelSaveBonus = 0f;
    public float moveSpdBonus = 0f;
    public int armorBonus = 0;
    public int bulletSizeBonus = 0;
    public int playerSizeBonus = 0;
    public float enemyBulletSlowBonus = 0f;
    public int pierceStack = 0;
    public int burnStack = 0;
    public int chainStack = 0;
    public int heavyArmorStack = 0;
    public float heavyArmorSpdPenalty = 0f;
    public int collisionDamageStack = 0;
    public int deathRingStack = 0;
    public int missileShuntStack = 0;
    public int counterStrikeStack = 0;
    public int lifestealStack = 0;
    public int fullHpPowerStack = 0;
    public int autoAimStack = 0;
    public int bombardmentStack = 0;
    public bool bloodOathPending = false;
    public int bloodOathKills = 0;
    public int bloodOathCompleted = 0;
    public int overloadReactionStack = 0;
    public int bombardmentModeStack = 0;
    public int volleyProtocolStack = 0;
    public int bloodlustStack = 0;
    public int shieldChargeStack = 0;
    public float shieldChargeTimer = 0f;
    public int reviveStack = 0;
    public bool reviveUsed = false;
    public int fireBreathStack = 0;
    public float fireBreathCd = 0f;
    public bool shieldActive = false;

    private static Sprite cachedCircleSprite;
    private GameObject shieldVisual;
    public float fireBreathVisualTimer = 0f; // 喷射计时器
    // 特效用圆
    public static Sprite GetCircleSprite()
    {
        if (cachedCircleSprite == null)
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size);
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    if (dist < radius)
                    {
                        float alpha = Mathf.Clamp01((radius - dist) / 4f);
                        tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                    }
                    else tex.SetPixel(x, y, Color.clear);
                }
            }
            tex.Apply();
            cachedCircleSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
        return cachedCircleSprite;
    }

    // 玩家朝向
    private float angle = Mathf.PI / 2f;

    // 调试 HUD 显示
    private bool sprinting = false;
    private bool moving = false;

    // 射击相关
    private float shootCd = 0f;      // 射击冷却（秒）
    private int bcount = 1;          // 基础弹数
    private Camera cam;

    void Start()
    {
        fuel = GameConfig.MaxFuel;
        AutoScaleSprite();
        cam = Camera.main;
        dur = GameConfig.MaxDur;
        if (GameManager.I != null) GameManager.I.player = this;
        radius += playerSizeBonus;

        shieldVisual = new GameObject("ShieldVisual");
        shieldVisual.transform.SetParent(transform);
        shieldVisual.transform.localPosition = Vector3.zero;
        SpriteRenderer shieldSr = shieldVisual.AddComponent<SpriteRenderer>();
        shieldSr.sprite = GetCircleSprite();
        shieldSr.color = new Color(0.3f, 0.7f, 1f, 0.4f);
        shieldSr.sortingOrder = 15;
        shieldVisual.SetActive(false);
    }

    void Update()
    {
        // 移动输入
        float dx = 0f, dy = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) dy += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) dy -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) dx -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) dx += 1f;

        moving = (dx != 0f || dy != 0f);
        sprinting = Input.GetKey(KeyCode.Space) && moving;

        // 移动
        if (moving)
        {
            Vector2 dir = new Vector2(dx, dy).normalized;
            float baseSpeed = sprinting ? GameConfig.PlayerSprintSpeed : GameConfig.PlayerSpeed;
            float speed = Mathf.Max(10f, baseSpeed + moveSpdBonus * 60f - heavyArmorSpdPenalty * 60f);
            transform.position += (Vector3)(dir * speed * Time.deltaTime);
            angle = Mathf.Atan2(dir.y, dir.x);
        }

        // 燃料
        bool fuelFree = (shieldActive && shieldChargeStack > 0);
        if (!fuelFree)
        {
            float rate = sprinting ? GameConfig.FuelCostSprintPerSecond : GameConfig.FuelCostPerSecond;
            rate = rate * (1f - (fuelSaveBonus / 100f));
            fuel = Mathf.Max(0f, fuel - rate * Time.deltaTime);
        }

        // 临时道具计时
        if (strengthenTimer > 0f) strengthenTimer -= Time.deltaTime;
        if (fireRateBoostTimer > 0f) fireRateBoostTimer -= Time.deltaTime;


        // 特效生成

        // 护盾显示与脉动
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(shieldActive);
            if (shieldActive)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 6f) * 0.05f;
                float targetDiameter = radius * 0.5f;
                shieldVisual.transform.localScale = Vector3.one * targetDiameter * pulse;
            }
        }

        // 双发
        if (strengthenTimer > 0f && Random.value < 0.5f)
        {
            GameObject p = new GameObject("PurpleSpark");
            p.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * radius);
            SpriteRenderer ssr = p.AddComponent<SpriteRenderer>();
            ssr.sprite = GetCircleSprite();
            ssr.sortingOrder = 60;

            VisualParticle vp = p.AddComponent<VisualParticle>();
            vp.velocity = Random.insideUnitCircle * 80f; // 往外飞
            vp.friction = 0.9f;
            vp.startScale = 8f;
            vp.endScale = 0f;
            vp.life = 0.4f;
            vp.startColor = new Color(0.8f, 0.2f, 1f, 0.8f);
            vp.endColor = new Color(0.8f, 0.2f, 1f, 0f);
        }

        // 射速
        if (fireRateBoostTimer > 0f && Random.value < 0.5f)
        {
            GameObject p = new GameObject("YellowSpark");
            p.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * radius);
            SpriteRenderer frsr = p.AddComponent<SpriteRenderer>();
            frsr.sprite = GetCircleSprite();
            frsr.sortingOrder = 60;

            VisualParticle vp = p.AddComponent<VisualParticle>();
            vp.velocity = Random.insideUnitCircle * 100f;
            vp.friction = 0.9f;
            vp.startScale = 6f;
            vp.endScale = 0f;
            vp.life = 0.3f;
            vp.startColor = new Color(1f, 0.9f, 0.1f, 1f);
            vp.endColor = new Color(1f, 0.9f, 0.1f, 0f);
        }

        // 烈焰喷射
        if (fireBreathStack > 0)
        {
            fireBreathCd -= Time.deltaTime;

            // 触发伤害和视觉计时器
            if (fireBreathCd <= 0f && Enemy.All.Count > 0)
            {
                fireBreathCd = 3f;
                fireBreathVisualTimer = 0.4f; // ★ 视觉持续喷 0.4 秒

                Vector2 forward = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                // 伤害判定（一次）
                foreach (var e in new List<Enemy>(Enemy.All))
                {
                    Vector2 toEnemy = (Vector2)e.transform.position - (Vector2)transform.position;
                    if (toEnemy.magnitude <= 280f + e.radius && Vector2.Angle(forward, toEnemy.normalized) <= 35f)
                        e.Hurt(100);
                }
            }

            // 持续生成粒子
            if (fireBreathVisualTimer > 0f)
            {
                fireBreathVisualTimer -= Time.deltaTime;
                Vector2 forward = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                for (int i = 0; i < 4; i++)
                {
                    float spread = Random.Range(-35f, 35f) * Mathf.Deg2Rad;
                    float rot = Mathf.Atan2(forward.y, forward.x) + spread;
                    Vector2 dir = new Vector2(Mathf.Cos(rot), Mathf.Sin(rot));

                    GameObject fire = new GameObject("BreathFire");
                    fire.transform.position = transform.position + (Vector3)(dir * Random.Range(10f, 40f));

                    SpriteRenderer fireSr = fire.AddComponent<SpriteRenderer>();
                    fireSr.sprite = GetCircleSprite();
                    fireSr.sortingOrder = 90;

                    VisualParticle vp = fire.AddComponent<VisualParticle>();
                    vp.velocity = dir * Random.Range(600f, 900f);
                    vp.friction = 0.95f;
                    vp.life = Random.Range(0.2f, 0.4f);
                    vp.startScale = Random.Range(15f, 25f);
                    vp.endScale = Random.Range(50f, 80f);

                    vp.startColor = new Color(1f, 0.9f, 0.3f, 1f);
                    vp.endColor = new Color(1f, 0.1f, 0f, 0f);
                }
            }
        }
        // 信号干扰减速特效
        if (jammingTimer > 0f) jammingTimer -= Time.deltaTime;
        if (jammingTimer > 0f && Random.value < 0.4f)
        {
            GameObject p = new GameObject("PlayerJammingSpark");
            p.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * radius);
            SpriteRenderer jsr = p.AddComponent<SpriteRenderer>();
            jsr.sprite = GetCircleSprite();
            jsr.sortingOrder = 60;
            VisualParticle vp = p.AddComponent<VisualParticle>();
            vp.velocity = Random.insideUnitCircle * 100f;
            vp.startScale = 8f; vp.endScale = 0f; vp.life = 0.3f;
            vp.startColor = new Color(0.2f, 0.8f, 1f, 1f);
            vp.endColor = new Color(0.2f, 0.8f, 1f, 0f);
        }

        // 无敌帧计时
        invincibleTimer = Mathf.Max(0f, invincibleTimer - Time.deltaTime);

        // 边界
        ClampToScreen();

        // 贴图旋转
        float rotZ = angle * Mathf.Rad2Deg + spriteForwardOffsetDegrees;
        transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

        // 自动射击
        shootCd -= Time.deltaTime;
        if (shootCd <= 0f)
        {
            Fire();
            float interval = GameConfig.ShootInterval + fireRateBonus;
            if (fireRateBoostTimer > 0f) interval *= 0.6f;

            // 轰炸协议下，射击间隔变为 3 倍
            if (bombardmentModeStack > 0) interval *= 3f;

            shootCd = Mathf.Max(0.05f, interval);
        }
        // 无敌帧闪烁：每 4 帧切换一次半透明
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            if (invincibleTimer > 0f)
                c.a = (Mathf.FloorToInt(Time.time * 15f) % 2 == 0) ? 0.5f : 1f;
            else
                c.a = 1f;
            sr.color = c;
        }

        // 静海协议
        if (shieldChargeStack > 0)
        {
            if (shieldActive) shieldChargeTimer = 0f;
            else
            {
                shieldChargeTimer += Time.deltaTime;
                if (shieldChargeTimer >= 8f) { shieldActive = true; shieldChargeTimer = 0f; }
            }
        }

    }

    public int GetFinalDamage()
    {
        int baseDmg = GameConfig.BulletDmg + dmgBonus;

        // 满盈之力
        if (fullHpPowerStack > 0 && dur >= GameConfig.MaxDur + maxDurBonus)
            baseDmg += fullHpPowerStack * 4;

        // 自动瞄准
        if (autoAimStack > 0)
        {
            float mult = Mathf.Max(0.4f, 1f - autoAimStack * 0.12f);
            baseDmg = Mathf.RoundToInt(baseDmg * mult);
        }

        // 齐射协议
        if (volleyProtocolStack > 0) baseDmg = Mathf.RoundToInt(baseDmg * 1.8f);

        // 血怒
        if (bloodlustStack > 0)
        {
            float maxHp = GameConfig.MaxDur + maxDurBonus;
            float ratio = Mathf.Clamp01(dur / maxHp);
            float bonus = ratio <= 0.1f ? 1.0f : 0.1f + (1f - ratio) / 0.9f * 0.9f;
            baseDmg = Mathf.RoundToInt(baseDmg * (1f + bonus));
        }

        // 轰炸协议下，子弹伤害减半
        if (bombardmentModeStack > 0)
        {
            baseDmg = Mathf.RoundToInt(baseDmg * 0.5f);
        }
        return Mathf.Max(1, baseDmg);
    }

    void Fire()
    {
        if (cam == null || BulletPool.I == null) return;

        Vector3 mouseWorld = cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 playerPos = transform.position;
        Vector2 aimDir = (new Vector2(mouseWorld.x, mouseWorld.y) - playerPos);

        // 自动瞄准
        if (autoAimStack > 0 && Enemy.All.Count > 0)
        {
            Enemy nearest = Enemy.All[0];
            float minD = float.MaxValue;
            foreach (var e in Enemy.All)
            {
                float d = Vector2.Distance(playerPos, e.transform.position);
                if (d < minD) { minD = d; nearest = e; }
            }
            aimDir = ((Vector2)nearest.transform.position - playerPos).normalized;
        }
        else
        {
            if (aimDir.sqrMagnitude < 0.0001f) aimDir = Vector2.up;
            aimDir.Normalize();
        }

        float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x);

        // 弹数计算
        int totalBullets = bcount + (volleyProtocolStack > 0 ? 1 : 0) + (strengthenTimer > 0f ? 1 : 0);

        int finalDmg = GetFinalDamage();
        float finalRadius = 5f + bulletSizeBonus;

        for (int i = 0; i < totalBullets; i++)
        {
            float off = (i - (totalBullets - 1) / 2f) * GameConfig.Spread;
            float a = baseAngle + off;
            Vector2 bulletDir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));

            // 无限穿透：齐射协议传 -1 表示无限
            int pCount = volleyProtocolStack > 0 ? -1 : pierceStack;

            BulletPool.I.Spawn(playerPos, bulletDir, GameConfig.BulletSpeed, finalDmg, true, finalRadius, pCount);
        }
    }

    public void Hurt(float dmg)
    {
        // 护盾优先吸收
        if (shieldActive)
        {
            shieldActive = false;
            shieldChargeTimer = 0f;
            return;
        }
        // 如果处于无敌帧，直接无视伤害
        if (invincibleTimer > 0f) return;
        // 扣血计算
        float final = Mathf.Max(1f, dmg + armorBonus);
        dur = Mathf.Max(0f, dur - final);
        invincibleTimer = GameConfig.InvincibleDuration;

        // 不屈意志（复活机制）
        if (dur <= 0f && reviveStack > 0 && !reviveUsed)
        {
            reviveUsed = true;
            dur = (GameConfig.MaxDur + maxDurBonus) * 0.5f; // 恢复50%
            invincibleTimer = 2f; // 2秒无敌

            // 特效
            for (int i = 0; i < 20; i++)
            {
                float a = i * (Mathf.PI * 2f / 20f);
                Vector2 dir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                LightningEffect.Draw(transform.position, (Vector2)transform.position + dir * 100f);
            }
        }

        // 反击冲击
        if (counterStrikeStack > 0)
        {
            for (int i = 0; i < counterStrikeStack; i++)
            {
                if (Enemy.All.Count > 0)
                {
                    Enemy target = Enemy.All[Random.Range(0, Enemy.All.Count)];
                    target.Hurt(18);
                    LightningEffect.Draw(transform.position, target.transform.position);
                }
            }
        }

        // 钢铁碰撞
        if (collisionDamageStack > 0)
        {
            int cdm = 10 * collisionDamageStack;
            var enemies = new List<Enemy>(Enemy.All);
            foreach (var e in new List<Enemy>(Enemy.All))
            {
                if (e.alive && Vector2.Distance(transform.position, e.transform.position) < radius + e.radius)
                    e.Hurt(cdm);
            }
        }
    }

    public void Pickup(string type)
    {
        if (type == "fuel")
        {
            fuel = Mathf.Min(GameConfig.MaxFuel, fuel + GameConfig.DropFuel);
            score += GameConfig.ScoreFuel;
        }
        else if (type == "repair")
        {
            float maxDur = GameConfig.MaxDur + maxDurBonus;
            dur = Mathf.Min(maxDur, dur + GameConfig.DropRepair);
            score += GameConfig.ScoreRepair;
        }
        else if (type == "upgrade") // 双发
        {
            strengthenTimer = 10f; // 持续 10 秒
            score += GameConfig.ScoreUpgrade;
        }
        else if (type == "shield") // 护盾
        {
            shieldActive = true;
            score += GameConfig.ScoreUpgrade;
        }
        else if (type == "fire_rate") // 射速提升
        {
            fireRateBoostTimer = 15f; // 持续 15 秒
            score += GameConfig.ScoreUpgrade;
        }
        else if (type == "jamming")
        {
            jammingTimer = GameConfig.SignalJammingDuration; // 6 秒
            score += GameConfig.ScoreUpgrade;
        }
    }
    void ClampToScreen()
    {
        float minX = -GameConfig.W / 2f + radius;
        float maxX = GameConfig.W / 2f - radius;
        float minY = -GameConfig.H / 2f + radius;
        float maxY = GameConfig.H / 2f - radius;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void AutoScaleSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        Vector2 size = sr.sprite.bounds.size;
        float maxSide = Mathf.Max(size.x, size.y);
        if (maxSide > 0.001f)
        {
            float s = GameConfig.PlayerSize / maxSide;
            transform.localScale = new Vector3(s, s, 1f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
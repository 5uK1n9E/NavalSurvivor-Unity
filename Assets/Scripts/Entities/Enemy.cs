// Enemy.cs
// 对应原 entities.py 的敌人字典 + update_enemy + draw_enemy
using UnityEngine;


public class Enemy : MonoBehaviour
{
    // 所有当前活跃的敌人（CollisionSystem 遍历用）
    public static readonly System.Collections.Generic.List<Enemy> All = new();
    [Header("类型 key")]
    public string typeKey = "scout";
    public GameObject boomPrefab;

    [Header("自动配置")]
    public int hp, maxHp;
    public float speed;         // 像素/秒
    public int dmg;
    public float fireRate;      // 秒
    public float radius = 26f;  // 碰撞半径（原 sz）
    public int bulletSize;      // 原 bsz
    public float bulletSpeed;   // 像素/秒
    public float visualMaxSize = 85f;

    [Header("贴图朝向 offset（度）")]
    public float spriteForwardOffsetDegrees = 90f;

    [Header("运行时状态")]
    public int uid;
    public bool alive = true;

    private float shootCd = 0f;
    private Player player;
    private Camera cam;

    private static int _nextUid = 1;
    [Header("掉落物贴图")]
    public Sprite fuelSprite;
    public Sprite repairSprite;
    public Sprite upgradeSprite;
    public GameObject dropPrefab;
    public Sprite shieldSprite;
    public Sprite fireRateSprite;
    public Sprite jammingSprite;

    public float burnTimer = 0f;      // 燃烧剩余时间
    public float burnTickAcc = 0f;    // 伤害结算累积器
    public bool killedByRing = false;

    void Start()
    {
        uid = _nextUid++;
        player = FindObjectOfType<Player>();
        cam = Camera.main;
        shootCd = Random.Range(0f, fireRate);
        AutoScaleSprite();
    }

    void OnValidate()
    {
        if (Application.isPlaying) return;
        if (string.IsNullOrEmpty(typeKey)) return;
        if (!GameConfig.ETYPES.ContainsKey(typeKey)) return;

        var s = GameConfig.ETYPES[typeKey];
        hp = maxHp = s.hp;
        speed = s.speed;
        dmg = s.dmg;
        fireRate = s.fireRate;
        radius = s.sz;
        bulletSize = s.bulletSize;
        bulletSpeed = s.bulletSpeed;
        visualMaxSize = typeKey switch
        {
            "scout" => 85f,
            "cruiser" => 110f,
            "battleship" => 145f,
            _ => 85f
        };
    }

    // 供 WaveManager 生成时调用，根据波次计算属性
    public void SetupStats(int waveNum)
    {
        if (!GameConfig.ETYPES.ContainsKey(typeKey)) return;
        var s = GameConfig.ETYPES[typeKey];

        // 计算血量倍率
        int stage = (waveNum - 1) / GameConfig.WaveStageLength;
        float hpMult = 1.0f + waveNum * GameConfig.WaveHpPerWave + stage * GameConfig.WaveHpStageBonus;

        // 计算伤害倍率
        float dmgMult = 1.0f + waveNum * 0.05f;

        // 赋值
        hp = Mathf.RoundToInt(s.hp * hpMult);
        maxHp = hp;
        dmg = Mathf.RoundToInt(s.dmg * dmgMult);

        speed = s.speed;
        fireRate = s.fireRate;
        radius = s.sz;
        bulletSize = s.bulletSize;
        bulletSpeed = s.bulletSpeed;

        visualMaxSize = typeKey switch
        {
            "scout" => 85f,
            "cruiser" => 110f,
            "battleship" => 145f,
            _ => 85f
        };

        // 更新贴图缩放
        AutoScaleSprite();
    }

    void Update()
    {
        if (!alive || player == null) return;

        Vector2 pos = transform.position;
        Vector2 playerPos = player.transform.position;
        float dist = Vector2.Distance(pos, playerPos);

        // 朝向玩家移动
        Vector2 dir = (playerPos - pos);
        if (dir.sqrMagnitude > 0.0001f) dir.Normalize();

        float finalSpeed = speed;
        if (GameManager.I.player.jammingTimer > 0f)
            finalSpeed *= GameConfig.JammingSpeedMult; // 0.45倍速
        transform.position += (Vector3)(dir * finalSpeed * Time.deltaTime);

        // 边界 clamp
        Vector3 p = transform.position;
        p.x = Mathf.Clamp(p.x, -GameConfig.W / 2f + radius, GameConfig.W / 2f - radius);
        p.y = Mathf.Clamp(p.y, -GameConfig.H / 2f + radius, GameConfig.H / 2f - radius);
        transform.position = p;

        // 贴图旋转朝玩家
        float angleRad = Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0f, 0f,
            angleRad * Mathf.Rad2Deg + spriteForwardOffsetDegrees);

        // 距离 < 350 时按间隔射击
        if (dist < 350f)
        {
            shootCd -= Time.deltaTime;
            if (shootCd <= 0f)
            {
                if (BulletPool.I != null)
                {
                    float finalBspd = bulletSpeed;
                    if (GameManager.I.player.jammingTimer > 0f)
                        finalBspd *= GameConfig.JammingSpeedMult;
                    BulletPool.I.Spawn(
                        transform.position,
                        dir,
                        bulletSpeed,
                        dmg,
                        isPlayer: false,
                        radius: bulletSize
                    );
                }
                shootCd = fireRate;
            }
        }
        // 燃烧 DoT 结算
        if (burnTimer > 0f)
        {
            burnTimer -= Time.deltaTime;
            burnTickAcc += Time.deltaTime;

            // 每 0.5 秒结算一次燃烧伤害
            if (burnTickAcc >= 0.5f)
            {
                burnTickAcc = 0f;
                int burnDps = (int)(GameConfig.BurnBaseDps + player.burnStack * GameConfig.BurnDpsPerStack);
                Hurt(burnDps * 1);
                Hurt((int)(burnDps * 0.5f));
            }
        }
        // 信号干扰时的电弧特效
        if (GameManager.I.player.jammingTimer > 0f && Random.value < 0.15f)
        {
            GameObject jp = new GameObject("JammingSpark");
            jp.transform.position = transform.position + (Vector3)(Random.insideUnitCircle * radius);
            SpriteRenderer sr = jp.AddComponent<SpriteRenderer>();
            sr.sprite = Player.GetCircleSprite();
            sr.sortingOrder = 50;
            VisualParticle vp = jp.AddComponent<VisualParticle>();
            vp.velocity = Random.insideUnitCircle * 50f;
            vp.startScale = 6f; vp.endScale = 0f; vp.life = 0.2f;
            vp.startColor = new Color(0.2f, 0.8f, 1f, 1f); // 蓝色
            vp.endColor = new Color(0.2f, 0.8f, 1f, 0f);
        }
    }

    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    // 受到伤害 hp归零死亡
    public void Hurt(int dmg)
    {
        if (!alive) return;
        hp -= dmg;
        if (hp <= 0)
            Die();
    }

    void Die()
    {
        alive = false;

        // 统计得分
        if (player != null)
        {
            player.kills++;
            player.score += (typeKey == "battleship")
                ? GameConfig.ScoreBoss
                : GameConfig.ScoreEnemy;
        }
        if (boomPrefab != null) Instantiate(boomPrefab, transform.position, Quaternion.identity);

        // 死亡回响
        Player p = GameManager.I.player;
        if (p != null && p.deathRingStack > 0 && !killedByRing) //如果是环弹击杀就不触发
        {
            int ringCount = 12;
            int ringDmg = Mathf.Max(1, (int)((GameConfig.BulletDmg + p.dmgBonus) * 0.5f)); // 一半伤害
            for (int i = 0; i < ringCount; i++)
            {
                float angle = i * (Mathf.PI * 2f / ringCount);
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                // 生成环形子弹（传入速度、伤害、半径）
                Bullet ringB = BulletPool.I.Spawn(transform.position, dir, 6f * 60f, ringDmg, true, 4f);
                ringB.isRingBullet = true; // 标记为环弹
            }
        }
        if (p != null)
        {
            // 吸血
            if (p.lifestealStack > 0)
                p.dur = Mathf.Min(GameConfig.MaxDur + p.maxDurBonus, p.dur + (3 + p.lifestealStack * 2));

            // 杀戮契约
            if (p.bloodOathPending)
            {
                p.bloodOathKills++;
                if (p.bloodOathKills >= 15)
                {
                    p.dmgBonus += 20;
                    p.bloodOathPending = false;
                    p.bloodOathCompleted++;
                }
            }
        }
        // 掉落判定
        float r = Random.Range(0f, 100f);
        float fuelDropRate = (typeKey == "battleship") ? 25f : 100f; // 战舰掉燃料概率低，普通怪必掉

        string dropType = null;
        Sprite dropSprite = null;

        // 护盾
        if (r < GameConfig.DropShieldP)
        {
            dropType = "shield"; dropSprite = shieldSprite;
        }
        // 射速
        else if (r < GameConfig.DropShieldP + GameConfig.DropFireRateP)
        {
            dropType = "fire_rate"; dropSprite = fireRateSprite;
        }
        // 减速
        else if (r < GameConfig.DropShieldP + GameConfig.DropFireRateP + GameConfig.DropJammingP) // ★ 新增 5% 减速
        {
            dropType = "jamming"; dropSprite = jammingSprite;
        }
        // 双发
        else if (r < GameConfig.DropShieldP + GameConfig.DropFireRateP + GameConfig.DropJammingP + GameConfig.DropUp)
        {
            dropType = "upgrade"; dropSprite = upgradeSprite;
        }
        // 修复
        else if (r < GameConfig.DropShieldP + GameConfig.DropFireRateP + GameConfig.DropUp + GameConfig.DropRepP)
        {
            dropType = "repair";
            dropSprite = repairSprite;
        }
        // 燃料
        else if (r < fuelDropRate)
        {
            dropType = "fuel";
            dropSprite = fuelSprite;
        }

        if (dropType != null && dropPrefab != null)
        {
            GameObject dropObj = Instantiate(dropPrefab, transform.position, Quaternion.identity);
            dropObj.GetComponent<Drop>().Init(dropType, dropSprite);
        }

        Destroy(gameObject);
        if (AudioManager.I != null) AudioManager.I.PlayBoom();
    }

    void AutoScaleSprite()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;
        Vector2 size = sr.sprite.bounds.size;
        float maxSide = Mathf.Max(size.x, size.y);
        if (maxSide > 0.001f)
        {
            float s = visualMaxSize / maxSide;
            transform.localScale = new Vector3(s, s, 1f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
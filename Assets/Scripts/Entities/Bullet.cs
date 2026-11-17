// Bullet.cs
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 所有当前活跃的子弹（CollisionSystem 遍历用）
    public static readonly System.Collections.Generic.List<Bullet> All = new();
    [Header("碰撞半径（默认 r=5）")]
    public float radius = 5f;

    // 运行时常量
    public float damage;
    public bool isPlayerBullet = true;

    // 运动状态
    private Vector2 velocity;
    private float life;                     // 剩余寿命（秒）
    private const float MaxLife = 200f / 60f;

    // 穿透
    public int pierce = 0;                  // 剩余穿透次数
    public bool infinitePierce = false;     // 无限穿透标记
    public HashSet<int> hitIds = new();     // 已命中敌人的 UID（防重复伤害）
    public bool isRingBullet = false;

    public void Init(Vector2 pos, Vector2 dir, float speed, int dmg, bool isPlayer = true, float radius = 5f, int pierceCount = 0)
    {
        transform.position = new Vector3(pos.x, pos.y, 0f);
        velocity = dir.normalized * speed;
        damage = dmg;
        isPlayerBullet = isPlayer;
        this.radius = radius;   // 碰撞半径
        life = MaxLife;
        hitIds.Clear();
        isRingBullet = false;
        if (pierceCount == -1) infinitePierce = true;
        else { infinitePierce = false; pierce = pierceCount; }

        transform.localScale = new Vector3(radius * 2f, radius * 2f, 1f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = isPlayer
                ? new Color(255f / 255f, 240f / 255f, 70f / 255f)    // C_BULLET
                : new Color(255f / 255f, 90f / 255f, 90f / 255f);    // C_ENEMY_B
        }
    }

    void Update()
    {
        // 移动（秒）
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // 寿命
        life -= Time.deltaTime;

        // 出屏 / 超时 → 回收
        Vector3 p = transform.position;
        float halfW = GameConfig.W / 2f + 20f;
        float halfH = GameConfig.H / 2f + 20f;
        if (life <= 0f ||
            p.x < -halfW || p.x > halfW ||
            p.y < -halfH || p.y > halfH)
        {
            ReturnToPool();
        }
    }
    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    void ReturnToPool()
    {
        if (BulletPool.I != null)
            BulletPool.I.Recycle(this);
        else
            Destroy(gameObject);
    }
}
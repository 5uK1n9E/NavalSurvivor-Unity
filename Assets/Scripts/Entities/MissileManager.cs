using UnityEngine;

public class MissileManager : MonoBehaviour
{
    public static MissileManager I { get; private set; }

    public GameObject missilePrefab;

    private float spawnTimer = 0f;
    private float nextSpawnTime = 15f;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Update()
    {
        if (GameManager.I == null || GameManager.I.state != GameState.Playing) return;
        if (Time.timeScale == 0f) return;

        Player p = GameManager.I.player;
        if (p == null) return;

        // 轰炸协议：无视波次，开局就有
        bool isBombardmentMode = p.bombardmentModeStack > 0;
        // 普通模式：第 6 波后才有
        bool canSpawn = isBombardmentMode || WaveManager.I.wave >= 6;

        if (!canSpawn) return;

        // 计算间隔
        if (isBombardmentMode)
        {
            nextSpawnTime = 7.5f; // 协议专属固定间隔
        }
        else
        {
            // 受强化轰炸影响（每层减 15% 间隔，最低 3 秒）
            float spacingMult = Mathf.Max(0.2f, 1f - p.bombardmentStack * 0.15f);
            nextSpawnTime = Mathf.Max(3f, 15f * spacingMult);
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= nextSpawnTime)
        {
            spawnTimer = 0f;
            SpawnMissiles(p);
        }
    }

    void SpawnMissiles(Player p)
    {
        // 计算数量
        int count = Random.Range(2, 5); // 基础 2~4 个
        if (p.bombardmentStack > 0) count += p.bombardmentStack; // 强化轰炸加数量
        if (p.bombardmentModeStack > 0) count += 5; // 轰炸协议额外加 5 个
        count = Mathf.Min(count, 12); // 上限 12 个

        for (int i = 0; i < count; i++)
        {
            // 在屏幕内随机位置生成
            float x = Random.Range(-GameConfig.W / 2f + 80f, GameConfig.W / 2f - 80f);
            float y = Random.Range(-GameConfig.H / 2f + 80f, GameConfig.H / 2f - 80f);

            if (missilePrefab != null)
            {
                Instantiate(missilePrefab, new Vector3(x, y, 0f), Quaternion.identity);
            }
            else
            {
                // 如果没有预制体，直接动态生成
                GameObject go = new GameObject("MissileStrike");
                go.transform.position = new Vector3(x, y, 0f);
                go.AddComponent<MissileStrike>();
            }
        }
    }
}
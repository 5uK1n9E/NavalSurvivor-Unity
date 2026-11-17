// WaveManager.cs
// 对应原 waves.py 的 WaveManager
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    public static WaveManager I { get; private set; }
    public bool isStoryMode = false;
    public int storyBattle = 0;
    private bool isTransitioning = false;

    [Header("敌人预制体")]
    public GameObject scoutPrefab;
    public GameObject cruiserPrefab;
    public GameObject battleshipPrefab;

    [Header("状态")]
    public int wave = 1;
    public float waveTimer = 0f;
    public int targetWaves = 9999; // 生存模式无限

    // 待生成队列
    private Queue<(Vector2 pos, string type)> queue = new();
    private float spawnTimer = 0f;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Start()
    {
        // 故事模式
        if (StoryProgress.IsStoryMode)
        {
            isStoryMode = true;
            storyBattle = StoryProgress.CurrentBattle;
            // 第一战 3 波，第二战 3 波，第三战 5 波
            if (storyBattle == 0) targetWaves = 3;
            else if (storyBattle == 1) targetWaves = 3;
            else targetWaves = 5;
        }
        else // 生存模式
        {
            isStoryMode = false;
            targetWaves = 9999;
        }
        StartWave(wave);
    }

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null || gm.state != GameState.Playing) return;
        if (Time.timeScale == 0f) return;

        // 波次推进
        if (Enemy.All.Count == 0 && queue.Count == 0)
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= 3f)
            {
                if (isStoryMode && wave >= targetWaves)
                {
                    isTransitioning = true;
                    // 故事模式战斗胜利
                    StoryProgress.CurrentBattle++;
                    if (StoryProgress.CurrentBattle >= 3)
                    {
                        // 三战全胜，进入胜利结局
                        StoryProgress.IsEnding = true;
                        StoryProgress.IsVictory = true;
                    }
                    else
                    {
                        // 进入下一幕
                        StoryProgress.IsEnding = false;
                    }
                    SceneManager.LoadScene("Story");
                    return;
                }

                wave++;
                StartWave(wave);
                waveTimer = 0f;

                if (!isStoryMode)
                {
                    GameManager.I.state = GameState.CardSelect;
                    Time.timeScale = 0f;
                    FindObjectOfType<CardSelectionUI>().ShowCards();
                }
            }
        }

        // 按间隔生成敌人
        if (queue.Count > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                var item = queue.Dequeue();
                SpawnEnemy(item.pos, item.type);
                spawnTimer = 0.25f;
            }
        }
    }

    public void StartWave(int waveNum)
    {
        int count = CalcEnemyCount(waveNum);

        for (int i = 0; i < count; i++)
        {
            // 以玩家为中心，350~580 像素随机位置
            Player p = GameManager.I.player;
            Vector2 center = p != null ? (Vector2)p.transform.position : Vector2.zero;

            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(350f, 580f);
            Vector2 pos = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            // 限制在屏幕内
            pos.x = Mathf.Clamp(pos.x, -GameConfig.W / 2f + 40f, GameConfig.W / 2f - 40f);
            pos.y = Mathf.Clamp(pos.y, -GameConfig.H / 2f + 40f, GameConfig.H / 2f - 40f);

            // 按概率选敌人
            string type = PickEnemyType(waveNum);
            queue.Enqueue((pos, type));
        }
    }

    void SpawnEnemy(Vector2 pos, string type)
    {
        GameObject prefab = type switch
        {
            "scout" => scoutPrefab,
            "cruiser" => cruiserPrefab,
            "battleship" => battleshipPrefab,
            _ => scoutPrefab
        };

        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, new Vector3(pos.x, pos.y, 0f), Quaternion.identity);
            Enemy e = obj.GetComponent<Enemy>();
            if (e != null)
            {
                e.SetupStats(wave);
            }
        }
    }

    int CalcEnemyCount(int waveNum)
    {
        int stage = (waveNum - 1) / GameConfig.WaveStageLength;
        return 3 + waveNum * GameConfig.WaveCountPerWave + stage * GameConfig.WaveCountStageBonus;
    }

    string PickEnemyType(int waveNum)
    {
        int stage = (waveNum - 1) / GameConfig.WaveStageLength;
        float r = Random.value; // 0.0 ~ 1.0

        if (stage == 0)
        {
            if (waveNum < 3) return "scout";
            if (waveNum < 6) return r < 0.6f ? "scout" : "cruiser";
            return r < 0.4f ? "scout" : (r < 0.75f ? "cruiser" : "battleship");
        }
        else if (stage == 1)
        {
            return r < 0.30f ? "scout" : (r < 0.70f ? "cruiser" : "battleship");
        }
        else if (stage == 2)
        {
            return r < 0.20f ? "scout" : (r < 0.60f ? "cruiser" : "battleship");
        }
        else
        {
            return r < 0.15f ? "scout" : (r < 0.55f ? "cruiser" : "battleship");
        }
    }

    // 强制进入下一波（供调试面板使用）
    public void ForceNextWave()
    {
        // 清空场上所有敌人
        var enemiesToKill = new List<Enemy>(Enemy.All);
        foreach (var e in enemiesToKill)
        {
            Destroy(e.gameObject);
        }

        // 清空还未生成的队列，重置计时器
        queue.Clear();
        spawnTimer = 0f;
        waveTimer = 0f;

        // 波次加 1，并立刻生成新波次
        wave++;
        StartWave(wave);
    }
}
// CollisionSystem.cs
using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem : MonoBehaviour
{
    void Update()
    {
        if (GameManager.I == null) return;
        if (GameManager.I.state != GameState.Playing) return;
        if (Time.timeScale == 0f) return;

        HandlePlayerBulletVsEnemy();
        HandleEnemyBulletVsPlayer();
        HandleEnemyContactVsPlayer();
        HandleDrops();
    }

    void HandleDrops()
    {
        Player p = GameManager.I.player;
        if (p == null) return;

        var drops = new List<Drop>(Drop.All);
        Vector2 ppos = p.transform.position;

        foreach (var d in drops)
        {
            if (!d.gameObject.activeInHierarchy) continue;

            Vector2 dpos = d.transform.position;
            float rr = d.radius + p.radius;
            if ((dpos - ppos).sqrMagnitude < rr * rr)
            {
                // 拾取
                p.Pickup(d.dropType);
                Destroy(d.gameObject); // 从场景销毁（或者做对象池）
            }
        }
    }

    // 玩家子弹 → 敌人（保持不变）
    void HandlePlayerBulletVsEnemy()
    {
        var bullets = new List<Bullet>(Bullet.All);
        var enemies = new List<Enemy>(Enemy.All);
        Player p = GameManager.I.player;

        foreach (var b in bullets)
        {
            if (!b.isPlayerBullet || !b.gameObject.activeInHierarchy) continue;

            Vector2 bpos = b.transform.position;
            float br = b.radius;

            foreach (var e in enemies)
            {
                if (!e.alive || !e.gameObject.activeInHierarchy) continue;

                Vector2 epos = e.transform.position;
                float rr = br + e.radius;

                if ((bpos - epos).sqrMagnitude < rr * rr)
                {
                    // 如果这颗子弹已经打过这个敌人了，直接跳过（穿透防重复伤害）
                    if (b.hitIds.Contains(e.uid)) continue;
                    b.hitIds.Add(e.uid);

                    // 计算超载反应
                    int finalDmg = (int)b.damage;
                    if (p.overloadReactionStack > 0 && p.chainStack > 0 && e.burnTimer > 0f)
                    {
                        float mult = 1.5f + p.chainStack * 0.2f + p.burnStack * 0.2f;
                        finalDmg = Mathf.RoundToInt(b.damage * mult);

                        // 超载的特效
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 randomOffset = Random.insideUnitCircle * 30f;
                            LightningEffect.Draw(e.transform.position, (Vector2)e.transform.position + randomOffset);
                        }
                    }

                    // 造成基础伤害
                    e.killedByRing = b.isRingBullet;
                    e.Hurt(finalDmg);

                    // 附加燃烧状态
                    if (p != null && p.burnStack > 0)
                    {
                        float dur = GameConfig.BurnBaseDuration + p.burnStack * GameConfig.BurnDurPerStack;
                        e.burnTimer = dur;
                        e.burnTickAcc = 0f; // 刷新燃烧时间并重置计时
                    }

                    // 触发连锁闪电
                    if (p != null && p.chainStack > 0)
                    {
                        int chainRadius = GameConfig.ChainLightningBaseRadius + p.chainStack * GameConfig.ChainLightningRadiusPerStack;
                        int chainDmg = GameConfig.ChainLightningBaseDamage + p.chainStack * GameConfig.ChainLightningDmgPerStack;

                        int chainedCount = 0;
                        foreach (var other in new List<Enemy>(Enemy.All))
                        {
                            if (other == e || !other.alive) continue;
                            if (chainedCount >= GameConfig.ChainLightningMaxTarget) break;

                            if (Vector2.Distance(epos, (Vector2)other.transform.position) <= chainRadius)
                            {
                                other.Hurt(chainDmg);
                                chainedCount++;
                                // 画一条闪电
                                LightningEffect.Draw(epos, (Vector2)other.transform.position);
                            }
                        }
                    }

                    // 穿透处理
                    if (b.infinitePierce) { }
                    else if (b.pierce > 0)
                    {
                        b.pierce--;
                    }
                    else
                    {
                        if (BulletPool.I != null) BulletPool.I.Recycle(b);
                        break;
                    }
                }
            }
        }
    }

    void HandleEnemyBulletVsPlayer()
    {
        Player p = GameManager.I.player;
        if (p == null) return;
        if (p.dur <= 0f) return;

        var bullets = new List<Bullet>(Bullet.All);
        Vector2 ppos = p.transform.position;

        foreach (var b in bullets)
        {
            if (b.isPlayerBullet) continue;
            if (!b.gameObject.activeInHierarchy) continue;

            Vector2 bpos = b.transform.position;
            float rr = b.radius + p.radius;
            if ((bpos - ppos).sqrMagnitude < rr * rr)
            {
                if (p.invincibleTimer <= 0f)
                {
                    p.Hurt(b.damage);
                    if (BulletPool.I != null) BulletPool.I.Recycle(b);
                }
                // 无敌帧内：敌弹不销毁，继续飞
            }
        }
    }

    // 敌人接触 → 玩家（20% 基础伤害）
        void HandleEnemyContactVsPlayer()
    {
        Player p = GameManager.I.player;
        if (p == null) return;
        if (p.invincibleTimer > 0f) return; // 如果正在无敌，直接跳过整个遍历

        var enemies = new List<Enemy>(Enemy.All);
        Vector2 ppos = p.transform.position;

        foreach (var e in enemies)
        {
            if (!e.alive) continue;

            Vector2 epos = e.transform.position;
            float rr = e.radius + p.radius;
            if ((epos - ppos).sqrMagnitude < rr * rr)
            {
                // 受到一次伤害立刻 break
                p.Hurt(e.dmg * 0.2f);
                break; 
            }
        }
    }
}
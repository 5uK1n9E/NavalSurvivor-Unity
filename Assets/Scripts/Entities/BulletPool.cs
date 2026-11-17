// BulletPool.cs
// 子弹对象池：避免频繁 Instantiate / Destroy 造成的卡顿
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool I { get; private set; }

    [Header("BulletPlayer预制体")]
    public Bullet bulletPrefab;

    private Queue<Bullet> pool = new();

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    public Bullet Spawn(Vector2 pos, Vector2 dir, float speed, int dmg, bool isPlayer = true, float radius = 5f, int pierceCount = 0)
    {
        Bullet b = pool.Count > 0 ? pool.Dequeue() : Instantiate(bulletPrefab, transform);
        b.gameObject.SetActive(true);
        b.Init(pos, dir, speed, dmg, isPlayer, radius, pierceCount);
        return b;
    }

    // 回收子弹到池里
    public void Recycle(Bullet b)
    {
        b.gameObject.SetActive(false);
        pool.Enqueue(b);
    }
}
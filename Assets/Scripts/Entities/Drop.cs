using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    public static readonly List<Drop> All = new List<Drop>();

    public string dropType; // "fuel", "repair", "upgrade"
    public float radius = 12f; // 拾取半径

    private SpriteRenderer sr;
    private float floatPhase;
    private Vector3 basePos;

    void OnEnable() => All.Add(this);
    void OnDisable() => All.Remove(this);

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        floatPhase = Random.Range(0f, 6.28f);
        basePos = transform.position;
    }

    void Update()
    {
        // 上下浮动动画
        floatPhase += 0.07f * 60f * Time.deltaTime;
        float yOffset = Mathf.Sin(floatPhase) * 3f;

        transform.position = new Vector3(basePos.x, basePos.y + yOffset, basePos.z);
    }

    // 根据掉落类型设置贴图
    public void Init(string type, Sprite sprite)
    {
        dropType = type;
        if (sprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            Vector2 size = sprite.bounds.size;
            float maxSide = Mathf.Max(size.x, size.y);
            if (maxSide > 0.001f)
            {
                float s = 24f / maxSide;
                transform.localScale = new Vector3(s, s, 1f);
            }
        }
    }
}
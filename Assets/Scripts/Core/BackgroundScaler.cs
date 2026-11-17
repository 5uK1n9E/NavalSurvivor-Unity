// BackgroundScaler.cs
// 让背景 Sprite 自动铺满 1200×800 的游戏区域
using UnityEngine;

[ExecuteAlways]
public class BackgroundScaler : MonoBehaviour
{
    void Start() => Fit();
    void OnValidate() => Fit();

    void Fit()
    {
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        Vector2 size = sr.sprite.bounds.size;
        if (size.x < 0.0001f || size.y < 0.0001f) return;

        transform.localScale = new Vector3(
            GameConfig.W / size.x,
            GameConfig.H / size.y,
            1f);
    }
}
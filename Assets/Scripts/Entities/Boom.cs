using UnityEngine;

public class Boom : MonoBehaviour
{
    public Sprite[] frames;
    public float maxRadius = 100f;
    public float frameRate = 10f;

    private SpriteRenderer sr;
    private float timer = 0f;
    private int currentFrame = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.one * 0.1f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= (1f / frameRate))
        {
            timer = 0f;
            currentFrame++;

            // 动画播放完，销毁自己
            if (currentFrame >= frames.Length)
            {
                Destroy(gameObject);
                return;
            }

            // 切换图片并放大
            sr.sprite = frames[currentFrame];
            float progress = (float)currentFrame / frames.Length;
            float size = Mathf.Lerp(0.1f, maxRadius * 2f, progress); // maxRadius*2 是因为Scale是直径
            transform.localScale = new Vector3(size, size, 1f);
        }
    }
}
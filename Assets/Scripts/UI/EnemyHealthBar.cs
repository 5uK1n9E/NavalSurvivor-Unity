using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Image fillImage;
    private Enemy enemy;
    private Vector3 offset = new Vector3(0, 60f, 0);

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            // 根据敌人半径自动计算头顶高度
            offset = new Vector3(0, enemy.radius + 20f, 0);
        }
    }

    void LateUpdate()
    {
        if (enemy == null) return;

        if (!enemy.alive)
        {
            if (fillImage != null) fillImage.fillAmount = 0;
            return;
        }

        if (fillImage != null)
            fillImage.fillAmount = (float)enemy.hp / enemy.maxHp;

        transform.position = enemy.transform.position + offset;
        transform.rotation = Quaternion.identity;
    }
}
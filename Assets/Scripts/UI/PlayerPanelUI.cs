using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelUI : MonoBehaviour
{
    public GameObject panel;
    public Text contentText;
    public Button tab0, tab1, tab2, btnClose;

    private int currentPage = 0;

    void Start()
    {
        tab0.onClick.AddListener(() => SwitchPage(0));
        tab1.onClick.AddListener(() => SwitchPage(1));
        tab2.onClick.AddListener(() => SwitchPage(2));
        btnClose.onClick.AddListener(() => panel.SetActive(false));

        panel.SetActive(false);
    }

    void Update()
    {
        // 按 Tab 键开关面板
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf) RefreshUI();
        }
    }

    void SwitchPage(int page)
    {
        currentPage = page;
        RefreshUI();
    }

    void RefreshUI()
    {
        if (!panel.activeSelf) return;
        Player p = GameManager.I.player;
        if (p == null) return;

        contentText.color = Color.white;
        string text = "";

        if (currentPage == 0) // 基础属性
        {
            text += "【基础属性】\n\n";
            text += $"耐久: {p.dur:F0} / {GameConfig.MaxDur + p.maxDurBonus}\n";
            text += $"燃料: {p.fuel:F0} / {GameConfig.MaxFuel}\n\n";
            text += $"子弹伤害: {p.GetFinalDamage()} (基础 {GameConfig.BulletDmg} + 额外 {p.dmgBonus})\n";
            text += $"射击间隔: {GameConfig.ShootInterval + p.fireRateBonus:F2} 秒\n";
            text += $"移动速度: {GameConfig.PlayerSpeed + p.moveSpdBonus * 60f:F1} 像素/秒\n";
            text += $"伤害减免: {-p.armorBonus}\n";
            text += $"子弹半径: {5f + p.bulletSizeBonus}\n";
        }
        else if (currentPage == 1) // 强化状态
        {
            text += "【强化实时状态】\n\n";
            if (p.burnStack > 0) text += $"燃烧弹药: {p.burnStack} 层\n";
            if (p.chainStack > 0) text += $"闪电弹药: {p.chainStack} 层\n";
            if (p.pierceStack > 0) text += $"破甲弹穿透: {p.pierceStack} 次\n";
            if (p.heavyArmorStack > 0) text += $"重型装甲: {p.heavyArmorStack} 层\n";
            if (p.shieldActive) text += $"护盾: 激活中\n";
            if (p.reviveStack > 0) text += $"不屈意志: {(p.reviveUsed ? "已用尽" : "待触发")}\n";
            if (p.bloodlustStack > 0) text += $"血怒: 已激活\n";
            if (text.Length < 40) text += "暂无特殊强化状态。";
        }
        else if (currentPage == 2) // 已获强化列表
        {
            text += "【已获取强化列表】\n\n";
            text += "（详细记录需要后续扩展 UpgradeManager）\n";
        }

        contentText.text = text;
    }
}
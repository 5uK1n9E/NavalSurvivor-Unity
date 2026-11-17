using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    [Header("基础 UI 组件")]
    public GameObject panel;
    public Button btnClose;
    public Button btnAddDmg;
    public Button btnGodMode;
    public Button btnNextWave;

    [Header("强化列表相关")]
    public Transform content;
    public GameObject upgradeBtnPrefab;

    void Start()
    {
        // 绑定基础按钮事件
        if (btnAddDmg != null) btnAddDmg.onClick.AddListener(() => GameManager.I.player.dmgBonus += 10);

        if (btnGodMode != null)
            btnGodMode.onClick.AddListener(() =>
            {
                GameManager.I.player.dur = 9999;
                GameManager.I.player.fuel = 9999;
            });

        if (btnNextWave != null)
            btnNextWave.onClick.AddListener(() =>
            {
                WaveManager.I.ForceNextWave();
            });

        if (btnClose != null) btnClose.onClick.AddListener(() => panel.SetActive(false));

        // 动态生成所有强化的按钮
        GenerateUpgradeButtons();
    }

    void GenerateUpgradeButtons()
    {
        if (content == null || upgradeBtnPrefab == null) return;

        // 清空测试内容
        foreach (Transform child in content) Destroy(child.gameObject);

        // 遍历 Enum 里的所有强化类型
        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            GameObject btnObj = Instantiate(upgradeBtnPrefab, content);
            btnObj.name = "Btn_" + type.ToString();

            // 设置文字
            Text txt = btnObj.GetComponentInChildren<Text>();
            if (txt != null) txt.text = type.ToString();

            // 绑定点击事件（直接调用 UpgradeManager）
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                UpgradeManager.ApplyUpgrade(GameManager.I.player, type);
                Debug.Log($"通过调试面板获得强化：{type}");
            });
        }
    }

    void Update()
    {
        // 按 F1 打开/关闭面板
        if (Input.GetKeyDown(KeyCode.F1) && panel != null)
        {
            panel.SetActive(!panel.activeSelf);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CardSelectionUI : MonoBehaviour
{
    [Header("UI 面板引用")]
    public GameObject cardPanel;
    public Transform cardContainer;
    public GameObject cardTemplatePrefab;
    private List<GameObject> currentCards = new List<GameObject>();

    public void ShowCards()
    {
        cardPanel.SetActive(true);

        // 清空之前的卡牌
        foreach (var card in currentCards) Destroy(card);
        currentCards.Clear();

        // 从管理器抽取 3 张卡
        List<UpgradeData> cards = UpgradeManager.GetRandomCards(GameManager.I.player);

        foreach (var data in cards)
        {
            GameObject card = Instantiate(cardTemplatePrefab, cardContainer);

            Text titleText = card.transform.Find("Title").GetComponent<Text>();
            Text descText = card.transform.Find("Desc").GetComponent<Text>();

            // 根据品质决定颜色和前缀
            string hexColor = "#FFFFFF";
            string qualityTag = "";
            switch (data.quality)
            {
                case UpgradeQuality.Common: hexColor = "#4CAF50"; qualityTag = "[普通]"; break; // 绿
                case UpgradeQuality.Rare: hexColor = "#2196F3"; qualityTag = "[高级]"; break; // 蓝
                case UpgradeQuality.Epic: hexColor = "#9C27B0"; qualityTag = "[史诗]"; break; // 紫
                case UpgradeQuality.Gold: hexColor = "#FFC107"; qualityTag = "[传说]"; break; // 金
            }

            // 设置文本
            titleText.text = $"<color={hexColor}>{qualityTag} {data.name}</color>";
            descText.text = data.description;


            Button btn = card.GetComponent<Button>();
            UpgradeData capturedData = data;
            btn.onClick.AddListener(() => OnCardClicked(capturedData));

            currentCards.Add(card);
        }
    }

    void OnCardClicked(UpgradeData data)
    {
        // 应用强化效果
        UpgradeManager.ApplyUpgrade(GameManager.I.player, data.type);

        // 清空卡片，隐藏面板
        foreach (var card in currentCards) Destroy(card);
        currentCards.Clear();
        cardPanel.SetActive(false);

        // 恢复游戏状态
        GameManager.I.state = GameState.Playing;
        Time.timeScale = 1f;
    }
}
// HUDController.cs
// 用 uGUI 显示 HUD：耐久条、燃料条、击杀/得分、鼠标准星
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("拖入对应 UI 元素")]
    public Image durFill;
    public Image fuelFill;
    public Text killScoreText;
    public RectTransform crosshair;
    public Text durText;
    public Text fuelText;

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null) return;

        var p = gm.player;
        if (p == null)
        {
            if (durFill != null) durFill.fillAmount = 0f;
            if (fuelFill != null) fuelFill.fillAmount = 0f;
            if (killScoreText != null) killScoreText.text = "击杀: 0   得分: 0";
        }
        else
        {
            if (durFill != null) durFill.fillAmount = Mathf.Clamp01(p.dur / (GameConfig.MaxDur + p.maxDurBonus));
            if (fuelFill != null) fuelFill.fillAmount = Mathf.Clamp01(p.fuel / GameConfig.MaxFuel);
            if (durText != null) durText.text = $"{Mathf.CeilToInt(p.dur)} / {GameConfig.MaxDur + p.maxDurBonus}";
            if (fuelText != null) fuelText.text = $"{Mathf.FloorToInt(p.fuel)} / {GameConfig.MaxFuel}";
            if (killScoreText != null)
            {
                int waveNum = WaveManager.I != null ? WaveManager.I.wave : 1;
                int target = WaveManager.I != null ? WaveManager.I.targetWaves : 9999;

                if (StoryProgress.IsStoryMode)
                    killScoreText.text = $"故事模式 第{StoryProgress.CurrentBattle + 1}战 | 波次: {waveNum}/{target}";
                else
                    killScoreText.text = $"波次: {waveNum}   击杀: {p.kills}   得分: {p.score}";
            }
        }

        // 准星跟随鼠标
        if (crosshair != null)
            crosshair.position = Input.mousePosition;

        // 游戏结束时隐藏准星
        if (crosshair != null)
            crosshair.gameObject.SetActive(gm.state == GameState.Playing);
    }
}
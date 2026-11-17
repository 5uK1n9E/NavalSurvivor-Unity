// UIManager.cs
// 统一管理 GameOver 和 Pause 两个面板的显隐、按钮回调
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("GameOver 面板")]
    public GameObject gameOverPanel;
    public Text gameOverReason;
    public Button overRestartBtn;
    public Button overMenuBtn;

    [Header("暂停面板")]
    public GameObject pausePanel;
    public Button btnContinue;
    public Button btnRestart;
    public Button btnQuit;

    [Header("右上角按钮")]
    public Button btnPause;
    public Button btnMute;
    public Text muteText;

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;

        // 绑定按钮回调
        if (btnContinue != null) btnContinue.onClick.AddListener(OnContinue);
        if (btnRestart != null) btnRestart.onClick.AddListener(OnRestart);
        if (btnQuit != null) btnQuit.onClick.AddListener(OnQuit);
    }

    void Start()
    {
        // 启动时全部隐藏
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (btnPause != null) btnPause.onClick.AddListener(() => GameManager.I.TogglePause());
        if (btnMute != null) btnMute.onClick.AddListener(OnMuteClicked);
        if (overRestartBtn != null) overRestartBtn.onClick.AddListener(OnRestart);
        if (overMenuBtn != null) overMenuBtn.onClick.AddListener(OnQuit);
    }

    void OnMuteClicked()
    {
        if (GameManager.I != null)
        {
            GameManager.I.muted = !GameManager.I.muted;
            AudioListener.volume = GameManager.I.muted ? 0f : 1f;
            if (muteText != null) muteText.text = GameManager.I.muted ? "静" : "音";
        }
    }

    void Update()
    {
        var gm = GameManager.I;
        if (gm == null) return;

        // ── GameOver 面板显隐 ──
        if (gameOverPanel != null)
        {
            bool shouldShow = (gm.state == GameState.Lose || gm.state == GameState.Win);
            if (gameOverPanel.activeSelf != shouldShow)
            {
                gameOverPanel.SetActive(shouldShow);
                if (shouldShow && gameOverReason != null)
                    gameOverReason.text = gm.winWhy;
            }
        }

        // ── 暂停面板显隐 ──
        if (pausePanel != null)
        {
            bool shouldShowPause = (gm.state == GameState.Paused);
            if (pausePanel.activeSelf != shouldShowPause)
                pausePanel.SetActive(shouldShowPause);
        }

        // ── ESC 切换暂停 ──
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gm.state == GameState.Playing)
                gm.TogglePause();
            else if (gm.state == GameState.Paused)
                gm.TogglePause();
        }

        // ── 按 R 重开（GameOver 状态下）──
        if ((gm.state == GameState.Lose || gm.state == GameState.Win) &&
            Input.GetKeyDown(KeyCode.R))
        {
            gm.Restart();
        }
    }

    void OnContinue()
    {
        if (GameManager.I != null) GameManager.I.TogglePause();
    }

    void OnRestart()
    {
        Time.timeScale = 1f;
        GameManager.I.Restart();
    }

    void OnQuit()
    {
        // 恢复时间流速，否则回到主菜单后，按钮会卡死无法点击
        Time.timeScale = 1f;
        // 加载 MainMenu 场景
        SceneManager.LoadScene("MainMenu");
    }
}
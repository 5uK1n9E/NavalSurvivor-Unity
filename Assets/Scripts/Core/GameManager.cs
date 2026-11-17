// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing, Paused, Win, Lose, StoryAct, CardSelect
}

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("游戏状态（运行时只读）")]
    public GameState state = GameState.Playing;

    public int frame = 0;
    public bool menuMode = false;
    public bool muted = false;
    public bool storyMode = false;
    public int storyBattle = 0;
    public int storyActIdx = 0;

    public Player player;
    public string winWhy = "";   // 失败原因："耐久归零" / "燃料耗尽"

    void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    void Update()
    {
        if (menuMode) return;

        // 死亡 / 胜利后：等玩家按 R 重开
        if (state == GameState.Lose || state == GameState.Win)
        {
            if (Input.GetKeyDown(KeyCode.R))
                Restart();
            return;
        }

        if (state == GameState.Playing)
        {
            frame++;
            CheckEndCondition();
        }
    }

    // 胜负判定
    void CheckEndCondition()
    {
        if (player == null) return;

        if (player.dur <= 0f)
        {
            if (StoryProgress.IsStoryMode)
            {
                // 故事模式失败，直接播失败结局
                StoryProgress.IsEnding = true;
                StoryProgress.IsVictory = false;
                Time.timeScale = 1f;
                SceneManager.LoadScene("Story");
                return;
            }
            state = GameState.Lose;
            winWhy = "耐久归零";
            Time.timeScale = 0f;
        }
        else if (player.fuel <= 0f)
        {
            if (StoryProgress.IsStoryMode)
            {
                StoryProgress.IsEnding = true;
                StoryProgress.IsVictory = false;
                Time.timeScale = 1f;
                SceneManager.LoadScene("Story");
                return;
            }
            state = GameState.Lose;
            winWhy = "燃料耗尽";
            Time.timeScale = 0f;
        }
    }

    // 重开
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        if (state == GameState.Playing)
        {
            state = GameState.Paused;
            Time.timeScale = 0f;
        }
        else if (state == GameState.Paused)
        {
            state = GameState.Playing;
            Time.timeScale = 1f;
        }
    }
}
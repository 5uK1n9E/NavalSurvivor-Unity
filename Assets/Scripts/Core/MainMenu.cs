using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("拖入按钮")]
    public Button btnStart;
    public Button btnQuit;
    public Button btnStory;

    void Start()
    {
        // 绑定点击事件
        if (btnStart != null)
            btnStart.onClick.AddListener(OnStartClicked);
        if (btnQuit != null)
            btnQuit.onClick.AddListener(OnQuitClicked);
        if (btnStory != null)
        {
            btnStory.onClick.AddListener(() =>
            {
                StoryProgress.IsStoryMode = true;
                StoryProgress.NextActIndex = 0; // 从第一幕开始
                StoryProgress.CurrentBattle = 0;
                StoryProgress.IsEnding = false;
                SceneManager.LoadScene("Story");
            });
        }
    }

    void OnStartClicked()
    {
        SceneManager.LoadScene("Game");
    }

    void OnQuitClicked()
    {
        // 退出游戏
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
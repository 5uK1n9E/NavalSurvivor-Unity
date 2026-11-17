using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StoryManager : MonoBehaviour
{
    public Image bgImage;
    public Text nameText;
    public Text contentText;
    public Button clickButton;

    [Header("剧情 CG（按顺序放 4 张过场，1 张胜利，1 张失败）")]
    public Sprite[] actCGs;
    public Sprite winCG;
    public Sprite loseCG;

    private struct StoryLine
    {
        public string speaker;
        public string text;
        public bool isNarration;
    }

    private List<StoryLine> lines = new List<StoryLine>();
    private int currentIndex = 0;
    private bool isTyping = false;
    private bool skipTyping = false;

    void Start()
    {
        // 1. 设置背景 CG
        if (StoryProgress.IsEnding)
        {
            if (StoryProgress.IsVictory && winCG != null) bgImage.sprite = winCG;
            else if (!StoryProgress.IsVictory && loseCG != null) bgImage.sprite = loseCG;
        }
        else
        {
            if (StoryProgress.NextActIndex < actCGs.Length)
                bgImage.sprite = actCGs[StoryProgress.NextActIndex];
        }

        // 2. 解析并播放剧情
        LoadStory();
        if (lines.Count > 0)
            StartCoroutine(PlayStory());
        else
            GoToNextPhase(); // 如果没解析到，直接进战斗

        clickButton.onClick.AddListener(OnClickNext);
    }

    void LoadStory()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("story");
        if (textAsset == null)
        {
            Debug.LogError("找不到 story.txt！");
            return;
        }

        string[] rawLines = textAsset.text.Split('\n');
        bool inTargetAct = false;
        int actCounter = 0;

        foreach (var rawLine in rawLines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            bool isHeader = false;
            bool isEndingHeader = false;

            // 识别标题
            if (line.StartsWith("###"))
            {
                isHeader = true;
                if (line.Contains("结局")) isEndingHeader = true;
            }
            else if (line.StartsWith("结局") && line.Contains("："))
            {
                isHeader = true;
                isEndingHeader = true;
            }

            if (isHeader)
            {
                // 如果已经读到了我们需要的那段内容，遇到下一个标题立刻停止
                if (inTargetAct) break;

                if (StoryProgress.IsEnding)
                {
                    if (isEndingHeader)
                    {
                        // ★ 核心修复：精确判定是胜利还是失败
                        if (StoryProgress.IsVictory)
                        {
                            if (line.Contains("胜利") || line.Contains("结局二"))
                                inTargetAct = true;
                        }
                        else
                        {
                            if (line.Contains("失败") || line.Contains("结局一") || line.Contains("耗尽"))
                                inTargetAct = true;
                        }
                    }
                }
                else
                {
                    // 读取普通幕
                    if (!isEndingHeader)
                    {
                        if (actCounter == StoryProgress.NextActIndex) inTargetAct = true;
                        actCounter++;
                    }
                }
                continue;
            }

            if (!inTargetAct) continue;

            // 把 "失败：" 和 "胜利：" 当旁白处理，不显示为说话人
            if (line.Contains("：") && !line.StartsWith("**"))
            {
                string[] parts = line.Split(new char[] { '：' }, 2);
                if (parts.Length == 2)
                {
                    string speaker = parts[0].Replace("**", "").Trim();
                    string content = parts[1].Trim();

                    if (speaker == "失败" || speaker == "胜利")
                    {
                        lines.Add(new StoryLine { speaker = "", text = content, isNarration = true });
                    }
                    else
                    {
                        lines.Add(new StoryLine { speaker = speaker, text = content, isNarration = false });
                    }
                    continue;
                }
            }
            // 普通旁白
            lines.Add(new StoryLine { speaker = "", text = line.Replace("**", ""), isNarration = true });
        }

        if (lines.Count == 0)
        {
            Debug.LogWarning("解析出来的剧情是空的，请检查 text.txt 的标题格式。");
        }
    }

    IEnumerator PlayStory()
    {
        while (currentIndex < lines.Count)
        {
            StoryLine line = lines[currentIndex];

            if (line.isNarration)
            {
                nameText.text = "";
                contentText.color = new Color(0.7f, 0.7f, 0.7f);
            }
            else
            {
                nameText.text = line.speaker;
                contentText.color = Color.white;
            }

            // 打字机效果
            isTyping = true;
            skipTyping = false;
            contentText.text = "";
            foreach (char c in line.text)
            {
                if (skipTyping) break;
                contentText.text += c;
                yield return new WaitForSeconds(0.03f);
            }
            contentText.text = line.text;
            isTyping = false;

            // 等待点击
            bool clicked = false;
            yield return new WaitForSeconds(0.3f);
            while (!clicked)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) clicked = true;
                yield return null;
            }
            currentIndex++;
        }

        GoToNextPhase();
    }

    void GoToNextPhase()
    {
        // 如果刚播完的是结局，返回主菜单
        if (StoryProgress.IsEnding)
        {
            StoryProgress.IsStoryMode = false;
            StoryProgress.IsEnding = false;
            SceneManager.LoadScene("MainMenu");
            return;
        }

        // 如果刚播完过场，准备进入战斗
        StoryProgress.NextActIndex++;
        SceneManager.LoadScene("Game");
    }

    void OnClickNext()
    {
        if (isTyping) skipTyping = true;
    }
}
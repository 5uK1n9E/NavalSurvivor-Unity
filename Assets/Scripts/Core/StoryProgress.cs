public static class StoryProgress
{
    public static bool IsStoryMode = false;   // 是否在故事模式中
    public static int NextActIndex = 0;       // 接下来要播放第几幕 (0~3)
    public static int CurrentBattle = 0;      // 当前是第几战 (0,1,2)
    public static bool IsEnding = false;      // 当前是否在播放结局
    public static bool IsVictory = false;     // 胜利还是失败结局
}
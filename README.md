# Naval Survivor（北洋水师：致远舰）

> 一个 Unity 2D（正交）射击+生存小游戏：玩家驾驶致远舰在来袭敌军的波次中存活，并通过“卡牌强化”不断成长。游戏也包含按幕推进的故事模式与两种结局。

---

## 游戏特色

- **2D 顶视/侧向弹幕射击**：鼠标指向自动连发，玩家 W/A/S/D（或方向键）移动、按 **Space** 冲刺。
- **生存核心**：两项失败资源——
  - **耐久 dur**：归零即失败
  - **燃料 fuel**：耗尽即失败
- **波次系统**：敌舰按波次不断生成，波次越后敌人血量/伤害逐步增强。
- **掉落+拾取**：击毁敌舰可掉落燃料、修复、护盾、射速提升、信号干扰（减速）等。
- **战斗卡牌强化**：每清完一波（生存模式）会抽 3 张卡，点击后立即获得对应强化。
- **故事模式**：按幕播放剧情（打字机效果），战斗胜负决定最终结局。

---

## 模式概览

### 生存模式（Survival）
- 无限波次（`WaveManager` 不设置上限），目标是尽可能活得更久。
- 每当场上敌人全部清空，游戏会暂停并进入 **卡牌选择界面**，抽取 3 张随机强化。

### 故事模式（Story）
- 主菜单可进入故事模式。
- 分为 **3 场战斗**：
  - 第 1 场：3 波
  - 第 2 场：3 波
  - 第 3 场：5 波
- 完成 3 场战斗进入胜利结局；中途耐久归零或燃料耗尽进入失败结局。

---

## 操作说明

- **移动**：W/A/S/D 或方向键
- **冲刺**：按住 **Space**（需同时移动）
- **射击方向**：鼠标指向（自动按射速发射）
- **暂停**：ESC 或右上角暂停按钮
- **重开**：失败/胜利后按 **R**
- **查看面板**：战斗中按 **Tab** 打开强化/属性面板

---

## 核心系统（简述）

### 1) 波次生成与敌人类型
游戏在 `WaveManager` 中根据当前波次生成敌人队列，并在玩家附近随机生成：
- 敌舰类型：`scout / cruiser / battleship`
- 敌人血量与伤害会随波次逐步提升（同时用“阶段”改变类型概率）。

关键脚本：
- `Assets/Scripts/Core/WaveManager.cs`

### 2) 玩家射击与伤害
- 玩家会根据 **射击冷却**自动发射子弹。
- 子弹会有散射角度、穿透次数（或无限穿透：齐射协议）。
- 伤害在 `Player` 内综合计算（基础伤害 + 强化修正等）。

关键脚本：
- `Assets/Scripts/Entities/Player.cs`
- `Assets/Scripts/Entities/Bullet.cs`
- `Assets/Scripts/Entities/BulletPool.cs`
- `Assets/Scripts/Entities/CollisionSystem.cs`

### 3) 状态效果：燃烧与连锁闪电
- **燃烧（Burn）**：命中可触发燃烧层数，敌人会随时间掉血。
- **连锁闪电（Chain Lightning）**：在一定半径内对多个敌人造成额外伤害。

关键脚本：
- `Assets/Scripts/Entities/Enemy.cs`
- `Assets/Scripts/Entities/CollisionSystem.cs`

### 4) 导弹轰炸
部分强化会触发导弹轰炸系统：
- 轰炸会先出现预警圈与倒计时
- 倒计时结束后进入爆炸阶段，造成范围伤害与特效

关键脚本：
- `Assets/Scripts/Entities/MissileManager.cs`
- `Assets/Scripts/Entities/MissileStrike.cs`

### 5) 掉落与拾取
- 敌人死亡后以概率生成掉落物
- `CollisionSystem` 每帧检查玩家与掉落物距离并拾取生效

关键脚本：
- `Assets/Scripts/Entities/Enemy.cs`
- `Assets/Scripts/Entities/Drop.cs`
- `Assets/Scripts/Entities/CollisionSystem.cs`

---

## 卡牌强化系统

- 清完一波敌人后，进入卡牌选择界面：
  - **每次抽取 3 张卡**
  - 点击卡牌后立即应用强化，并恢复游戏继续下一波

关键脚本：
- `Assets/Scripts/UI/CardSelectionUI.cs`
- `Assets/Scripts/Upgrades/UpgradeManager.cs`
- `Assets/Scripts/Upgrades/UpgradeData.cs`

---

## 故事文本与结局

- 剧情文字来源于 `Assets/Resources/story.txt`。
- `StoryManager` 负责：
  - 选择对应幕的 CG 背景
  - 逐行解析对话/旁白
  - 打字机式显示并等待点击/空格推进
  - 战斗结束后根据胜负播放结局并回到主菜单

关键脚本：
- `Assets/Scripts/Core/StoryManager.cs`
- `Assets/Resources/story.txt`

---

## 项目结构（建议阅读路径）

- **核心逻辑**：`Assets/Scripts/Core/*`
- **实体与战斗**：`Assets/Scripts/Entities/*`
- **UI**：`Assets/Scripts/UI/*`
- **强化卡牌**：`Assets/Scripts/Upgrades/*`
- **场景**：
  - `Assets/Scenes/MainMenu.unity`
  - `Assets/Scenes/Game.unity`
  - `Assets/Scenes/Story.unity`

---

## 如何运行（开发者）

1. 使用 Unity 打开该工程目录。
2. 将 **MainMenu** 作为入口场景运行（或直接在 Unity Editor 里打开 `Assets/Scenes/MainMenu.unity` 并点击 Play）。
3. 如果你要打包，请在 Build Settings 中确保包含：
   - `MainMenu`
   - `Story`
   - `Game`

---

## 备注

本 README 依据当前工作区代码与资源脚本进行整理，不包含未经验证的功能推测。若你希望我进一步把每张强化卡的“触发条件/叠加规则/数值变化”整理成表格，也可以继续告诉我你的偏好格式（例如：按品质分组或按功能分组）。

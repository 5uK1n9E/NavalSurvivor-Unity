// GameConfig.cs
using System.Collections.Generic;
using UnityEngine;

public static class GameConfig
{
    //  调试
    public const bool DebugMode = false;

    //  屏幕 / 分辨率
    public const int W = 1200;
    public const int H = 800;
    public const float FPS = 60f;

    public const float PixelsPerUnit = 1f;
    // 坐标系原点偏移
    public const float OriginOffsetX = W / 2f;
    public const float OriginOffsetY = H / 2f;

    // 帧秒换算
    public static float FramesToSeconds(float frames) => frames / FPS;
    public static float SecondsToFrames(float seconds) => seconds * FPS;

    //  颜色
    public static readonly Color32 C_SEA = new Color32(8, 35, 75, 255);
    public static readonly Color32 C_WAVE = new Color32(18, 65, 115, 255);
    public static readonly Color32 C_PLAYER = new Color32(70, 90, 110, 255);
    public static readonly Color32 C_PLAYER_D = new Color32(155, 55, 55, 255);
    public static readonly Color32 C_FUEL = new Color32(240, 200, 40, 255);
    public static readonly Color32 C_REPAIR = new Color32(40, 215, 90, 255);
    public static readonly Color32 C_UPGRADE = new Color32(200, 75, 240, 255);
    public static readonly Color32 C_SHIELD = new Color32(60, 140, 255, 255);
    public static readonly Color32 C_FIRE_RATE = new Color32(255, 220, 50, 255);
    public static readonly Color32 C_JAMMING = new Color32(60, 180, 255, 255);
    public static readonly Color32 C_BULLET = new Color32(255, 240, 70, 255);
    public static readonly Color32 C_ENEMY_B = new Color32(255, 90, 90, 255);
    public static readonly Color32 C_WHITE = new Color32(255, 255, 255, 255);
    public static readonly Color32 C_DIM = new Color32(150, 150, 150, 255);
    public static readonly Color32 C_DARK = new Color32(15, 28, 50, 255);

    // 按钮颜色
    public static readonly Color32 BTN_START = new Color32(25, 65, 90, 255);
    public static readonly Color32 BTN_START_H = new Color32(35, 90, 120, 255);
    public static readonly Color32 BTN_EXIT = new Color32(55, 35, 35, 255);
    public static readonly Color32 BTN_EXIT_H = new Color32(80, 45, 45, 255);
    public static readonly Color32 BTN_MENU = new Color32(30, 55, 80, 255);
    public static readonly Color32 BTN_MENU_H = new Color32(42, 78, 110, 255);
    public static readonly Color32 BTN_RESTART = new Color32(30, 70, 50, 255);
    public static readonly Color32 BTN_RESTART_H = new Color32(42, 100, 70, 255);
    public static readonly Color32 BTN_PAUSE = new Color32(40, 40, 60, 255);
    public static readonly Color32 BTN_PAUSE_H = new Color32(60, 60, 90, 255);

    //  资源管理
    public const float MaxFuel = 100f;
    public const float FuelCostPerSecond = 0.04f * FPS;
    public const float FuelCostSprintPerSecond = 0.08f * FPS;
    public const int MaxDur = 100;

    // 胜利波次
    public const int WinWaves = 10;
    public const int WinWavesStory1 = 3;
    public const int WinWavesStory2 = 3;
    public const int WinWavesStory3 = 5;
    public static readonly float[] StoryHpMult = { 0.15f, 0.30f, 0.45f };

    //  导弹轰炸
    public const float MissileMinSpacing = 900f / FPS;
    public const int MissileCountMin = 2;
    public const int MissileCountMax = 4;
    public const float MissileCountdown = 120f / FPS;
    public const float MissileExplodeDuration = 180f / FPS;
    public const int MissileRadius = 80;
    public const int MissileDamage = 25;
    public const int MissileEnemyDamage = 10;

    //  道具掉落
    public const int DropFuel = 18;
    public const int DropRepair = 12;
    public const int DropUp = 5;
    public const int DropRepP = 35;
    public const int DropShieldP = 5;
    public const int DropFireRateP = 5;
    public const int DropJammingP = 5;

    //  得分
    public const int ScoreEnemy = 10;
    public const int ScoreBoss = 50;
    public const int ScoreFuel = 5;
    public const int ScoreRepair = 8;
    public const int ScoreUpgrade = 15;

    //  战斗参数
    public const float PlayerSpeed = 3.5f * FPS;
    public const float PlayerSprintSpeed = 6.3f * FPS;
    public const float ShootInterval = 13f / FPS;
    public const int BulletDmg = 10;
    public const float BulletSpeed = 8f * FPS;
    public const float Spread = 0.15f;
    public const int Range = 380;
    public const float HintDuration = 180f / FPS;
    public const float InvincibleDuration = 45f / FPS;
    public const float StrengthenDuration = 600f / FPS;

    //  贴图尺寸
    public const int PlayerSize = 80;


    //  强化道具
    public const float FireRateBoostDuration = 900f / FPS;
    public const float FireRateBoostMult = 0.6f;
    public const float SignalJammingDuration = 360f / FPS;
    public const float JammingSpeedMult = 0.45f;

    //  敌舰配置
    public struct EnemyStats
    {
        public int hp;
        public float speed;
        public int dmg;
        public float fireRate;
        public int sz;
        public int bulletSize;
        public float bulletSpeed;
        public Color32 color;
    }

    public static readonly Dictionary<string, EnemyStats> ETYPES = new()
    {
        ["scout"] = new EnemyStats
        {
            hp = 18,
            speed = 1.5f * FPS,
            dmg = 5,
            fireRate = 100f / FPS,
            sz = 26,
            bulletSize = 8,
            bulletSpeed = 3f * FPS,
            color = new Color32(175, 55, 55, 255)
        },
        ["cruiser"] = new EnemyStats
        {
            hp = 45,
            speed = 0.9f * FPS,
            dmg = 10,
            fireRate = 80f / FPS,
            sz = 40,
            bulletSize = 12,
            bulletSpeed = 3f * FPS,
            color = new Color32(150, 45, 45, 255)
        },
        ["battleship"] = new EnemyStats
        {
            hp = 110,
            speed = 0.5f * FPS,
            dmg = 22,
            fireRate = 55f / FPS,
            sz = 56,
            bulletSize = 16,
            bulletSpeed = 4f * FPS,
            color = new Color32(130, 38, 38, 255)
        },
    };

    //  生存模式敌军成长
    public const int WaveStageLength = 10;
    public const float WaveHpPerWave = 0.10f;
    public const float WaveHpStageBonus = 0.80f;
    public const int WaveCountPerWave = 2;
    public const int WaveCountStageBonus = 5;

    //  卡牌强化系统
    public const int CardWaveInterval = 1;
    public const string UpgradeQualityCommon = "common";
    public const string UpgradeQualityRare = "rare";
    public const string UpgradeQualityEpic = "epic";
    public const string UpgradeQualityGold = "gold";
    public const float UpgradeRareChance = 0.20f;
    public const float UpgradeEpicChance = 0.08f;
    public const float UpgradeGoldChance = 0.03f;

    //  燃烧子弹 DoT
    public const float BurnBaseDps = 2f;
    public const float BurnBaseDuration = 3.0f;
    public const float BurnDpsPerStack = 1f;
    public const float BurnDurPerStack = 0.8f;
    public const float BurnTickRate = 0.5f;

    //  连锁闪电
    public const int ChainLightningBaseRadius = 120;
    public const int ChainLightningBaseDamage = 6;
    public const int ChainLightningRadiusPerStack = 25;
    public const int ChainLightningDmgPerStack = 3;
    public const int ChainLightningMaxTarget = 4;

    //  导弹偏转

    public const float MissileShuntPlayerDamageReducePerStack = 0.15f;
    public const float MissileShuntEnemyDamageBonusPerStack = 0.20f;
    public const float MissileShuntPlayerMinMult = 0.10f;

    //  反击 / 浴血 / 满盈
    public const int CounterStrikeDamage = 18;
    public const int LifestealBaseHeal = 3;
    public const int LifestealPerStack = 2;
    public const int FullHpBonusDmgPerStack = 4;

    //  重型装甲 / 钢铁碰撞
    public const int HeavyArmorPlayerSizePerStack = 6;
    public const int HeavyArmorDefPerStack = 8;
    public const float HeavyArmorSpdPenaltyPerStack = 0.4f;
    public const int CollisionDamagePerStack = 10;
    public const int CollisionArmorPenaltyPerStack = 3;

    //  自动瞄准
    public const float AutoAimFireRateMultPerStack = 0.20f;
    public const float AutoAimFireRateMultMin = 0.30f;
    public const float AutoAimDmgMultPerStack = 0.12f;
    public const float AutoAimDmgMultMin = 0.40f;

    //  强化轰炸
    public const float BombardmentDmgMultPerStack = 0.30f;
    public const float BombardmentSpacingMultPerStack = 0.15f;
    public const float BombardmentSpacingMultMin = 0.20f;
    public const float BombardmentSpacingFloor = 180f / FPS;
    public const int MissileCountPerStack = 1;
    public const int MissileCountCap = 12;

    //  血祭 / 杀戮契约
    public const int BloodPactMaxDurPerStack = 50;
    public const float BloodPactDurMult = 0.7f;
    public const int BloodPactMinDur = 20;
    public const int BloodOathDmgPenalty = 8;
    public const int BloodOathKillTarget = 15;
    public const int BloodOathDmgReward = 20;

    //  超载反应
    public const float OverloadBaseMult = 1.5f;
    public const float OverloadChainBonus = 0.2f;
    public const float OverloadBurnBonus = 0.2f;

    //  轰炸协议 / 齐射协议
    public const float BombardmentModeSpacing = 450f / FPS;
    public const float BombardmentModeDmgMult = 3.0f;
    public const int BombardmentModeCountBonus = 5;
    public const float VolleyProtocolDmgMult = 1.8f;

    //  血怒 / 静海协议
    public const float BloodlustBaseMult = 0.10f;
    public const float BloodlustMaxMult = 1.00f;
    public const float BloodlustTriggerRatio = 0.10f;
    public const float ShieldChargeDuration = 480f / FPS;

    //  不屈意志 / 死亡回响
    public const float ReviveHealRatio = 0.5f;
    public const float ReviveInvincibleDuration = 120f / FPS;
    public const int DeathRingBulletCount = 12;
    public const float DeathRingDmgRatio = 0.5f;
    public const float DeathRingBulletSpeed = 6.0f * FPS;
    public const int DeathRingBulletRadius = 4;

    //  烈焰喷射
    public const float FireBreathCooldown = 180f / FPS;
    public const int FireBreathDamage = 100;
    public const int FireBreathRange = 280;
    public const int FireBreathHalfAngle = 35;
    public const float FireBreathVisualDuration = 24f / FPS;

    //  工具函数
    public static float Clamp(float v, float lo, float hi) => Mathf.Clamp(v, lo, hi);
    public static float Dst(Vector2 a, Vector2 b) => Vector2.Distance(a, b);
    public static float Ang(Vector2 from, Vector2 to)
        => Mathf.Atan2(to.y - from.y, to.x - from.x);
}
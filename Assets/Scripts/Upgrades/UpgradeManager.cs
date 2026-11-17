using System.Collections.Generic;
using UnityEngine;

public static class UpgradeManager
{
    // 定义所有卡牌
    static List<UpgradeData> allCards = new List<UpgradeData>()
    {
        // 绿色普通
            new UpgradeData(UpgradeType.MaxDur, "耐久强化", "船体耐久提升", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.BulletDmg, "火力强化", "炮弹伤害增加", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.FireRate, "射速强化", "火炮射击间隔减少", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.FuelSave, "燃料节省", "燃料消耗速率降低", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.MoveSpd, "移动加速", "移动速度增加", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.Armor, "装甲强化", "护甲提升，受到伤害减少", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.BulletSize, "弹道增大", "炮弹尺寸增加", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.SmallerPlayer, "目标缩小", "船体碰撞体积缩小", UpgradeQuality.Common),
            new UpgradeData(UpgradeType.SlowEnemyBullets, "减速弹道", "敌方炮弹速度降低", UpgradeQuality.Common),
            
            // 蓝色高级
            new UpgradeData(UpgradeType.BulletBurn, "燃烧弹药", "命中使敌人燃烧；重复选择叠加伤害", UpgradeQuality.Rare),
            new UpgradeData(UpgradeType.BulletChain, "闪电弹药", "命中释放连锁闪电；重复选择叠加范围", UpgradeQuality.Rare),
            new UpgradeData(UpgradeType.MissileShunt, "爆破偏转", "导弹自伤降低，敌伤提高", UpgradeQuality.Rare),
            new UpgradeData(UpgradeType.CounterStrike, "反击冲击", "受击时对随机敌人反击一定伤害", UpgradeQuality.Rare),
            new UpgradeData(UpgradeType.Lifesteal, "浴血击杀", "击杀敌军恢复一定耐久", UpgradeQuality.Rare),
            new UpgradeData(UpgradeType.FullHpPower, "满盈之力", "船体未受损时炮弹伤害增加", UpgradeQuality.Rare),

            // 紫色史诗
            new UpgradeData(UpgradeType.PierceRound, "破甲弹", "子弹穿透。伤害较大幅度上升，射速降低", UpgradeQuality.Epic),
            new UpgradeData(UpgradeType.HeavyArmor, "重型装甲", "体型变大，护甲大幅上升，移速降低", UpgradeQuality.Epic),
            new UpgradeData(UpgradeType.CollisionDamage, "钢铁碰撞", "受到碰撞伤害时对敌军同样造成碰撞伤害，但护甲下降", UpgradeQuality.Epic),
            new UpgradeData(UpgradeType.AutoAim, "自动瞄准", "射速大幅上升，自动锁敌，但伤害降低", UpgradeQuality.Epic),
            new UpgradeData(UpgradeType.Bombardment, "强化轰炸", "轰炸导弹造成的伤害和数量提高，同时间隔缩短", UpgradeQuality.Epic),
            new UpgradeData(UpgradeType.BloodPact, "血祭", "最大耐久大幅增加，但当前耐久减半", UpgradeQuality.Epic),
            new UpgradeData(UpgradeType.BloodOath, "杀戮契约", "伤害一定程度降低，击杀一定数量敌军后伤害大幅增加", UpgradeQuality.Epic),

            // 金色传说
            new UpgradeData(UpgradeType.OverloadReaction, "超载反应", "闪电弹药击中处于燃烧状态的敌人时触发超载反应，伤害大幅上升", UpgradeQuality.Gold),
            new UpgradeData(UpgradeType.BombardmentMode, "轰炸协议", "子弹伤害和发射频率大幅降低，轰炸频率大幅上升且固定，轰炸不再对自身造成伤害", UpgradeQuality.Gold),
            new UpgradeData(UpgradeType.VolleyProtocol, "齐射协议", "炮弹常驻双发，拥有无限穿透，伤害大幅上升", UpgradeQuality.Gold),
            new UpgradeData(UpgradeType.Bloodlust, "血怒", "船体耐久越低，炮弹伤害越高", UpgradeQuality.Gold),
            new UpgradeData(UpgradeType.ShieldCharge, "静海协议", "一定时间未受损获得护盾，持有护盾期间燃料不消耗", UpgradeQuality.Gold),
            new UpgradeData(UpgradeType.Revive, "不屈意志", "耐久归零时复活一次", UpgradeQuality.Gold),
            new UpgradeData(UpgradeType.DeathRing, "死亡回响", "击杀敌军后在其位置散发出多枚环形炮弹", UpgradeQuality.Gold),
            new UpgradeData(UpgradeType.FireBreath, "烈焰喷射", "每隔一段时间在船头处喷射扇形火焰造成高额伤害", UpgradeQuality.Gold)
    };

    public static List<UpgradeData> GetRandomCards(Player p)
    {
        List<UpgradeData> results = new List<UpgradeData>();

        // 剔除已经满级的卡
        List<UpgradeData> availablePool = allCards.FindAll(x => !IsMaxed(p, x.type));

        for (int i = 0; i < 3; i++)
        {
            if (availablePool.Count == 0) break;

            float r = Random.value;
            UpgradeQuality targetQ = UpgradeQuality.Common;
            if (r < 0.03f) targetQ = UpgradeQuality.Gold;
            else if (r < 0.11f) targetQ = UpgradeQuality.Epic;
            else if (r < 0.31f) targetQ = UpgradeQuality.Rare;

            // 从可用池里抽
            var qualPool = availablePool.FindAll(x => x.quality == targetQ);
            if (qualPool.Count == 0) qualPool = availablePool.FindAll(x => x.quality == UpgradeQuality.Common); // 降级兜底

            if (qualPool.Count > 0)
            {
                int rand = Random.Range(0, qualPool.Count);
                UpgradeData picked = qualPool[rand];
                results.Add(picked);
                availablePool.Remove(picked); // 同一次抽卡里防重复
            }
        }
        return results;
    }

    // 检查是否达到上限（决定了能不能重复抽）
    public static bool IsMaxed(Player p, UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.FireRate: return p.fireRateBonus <= -16f / 60f;
            case UpgradeType.FuelSave: return p.fuelSaveBonus >= 90f;
            case UpgradeType.MoveSpd: return p.moveSpdBonus >= 4f;
            case UpgradeType.SmallerPlayer: return p.playerSizeBonus <= -16;
            case UpgradeType.SlowEnemyBullets: return p.enemyBulletSlowBonus >= 2f;
            case UpgradeType.BloodOath: return p.bloodOathPending; // 契约进行中不出现
            // 金色只需一层
            case UpgradeType.OverloadReaction: return p.overloadReactionStack >= 1;
            case UpgradeType.BombardmentMode: return p.bombardmentModeStack >= 1;
            case UpgradeType.VolleyProtocol: return p.volleyProtocolStack >= 1;
            case UpgradeType.Bloodlust: return p.bloodlustStack >= 1;
            case UpgradeType.ShieldCharge: return p.shieldChargeStack >= 1;
            case UpgradeType.Revive: return p.reviveStack >= 1;
            case UpgradeType.DeathRing: return p.deathRingStack >= 1;
            case UpgradeType.FireBreath: return p.fireBreathStack >= 1;
            default: return false; // 其它都可以无限叠
        }
    }

    public static void ApplyUpgrade(Player p, UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MaxDur: p.maxDurBonus += 20; p.dur = Mathf.Min(p.dur + 20, GameConfig.MaxDur + p.maxDurBonus); break;
            case UpgradeType.BulletDmg: p.dmgBonus += 2; break;
            case UpgradeType.FireRate: p.fireRateBonus -= 2f / 60f; break;
            case UpgradeType.FuelSave: p.fuelSaveBonus += 10; break;
            case UpgradeType.MoveSpd: p.moveSpdBonus += 0.2f; break;
            case UpgradeType.Armor: p.armorBonus -= 2; break;
            case UpgradeType.BulletSize: p.bulletSizeBonus += 3; break;
            case UpgradeType.SmallerPlayer: p.playerSizeBonus -= 2; p.radius = Mathf.Max(20, p.radius - 2); break;
            case UpgradeType.SlowEnemyBullets: p.enemyBulletSlowBonus += 0.2f; break;

            case UpgradeType.PierceRound: p.pierceStack++; p.dmgBonus += 12; p.fireRateBonus += 4f / 60f; break;
            case UpgradeType.BulletBurn: p.burnStack++; break;
            case UpgradeType.BulletChain: p.chainStack++; break;
            case UpgradeType.MissileShunt: p.missileShuntStack++; break;
            case UpgradeType.CounterStrike: p.counterStrikeStack++; break;
            case UpgradeType.Lifesteal: p.lifestealStack++; break;
            case UpgradeType.FullHpPower: p.fullHpPowerStack++; break;

            case UpgradeType.HeavyArmor:
                p.playerSizeBonus += 6; p.radius += 6; p.armorBonus -= 8; p.heavyArmorSpdPenalty += 0.4f; break;
            case UpgradeType.CollisionDamage: p.collisionDamageStack++; p.armorBonus += 3; break;
            case UpgradeType.AutoAim: p.autoAimStack++; break;
            case UpgradeType.Bombardment: p.bombardmentStack++; break;
            case UpgradeType.BloodPact:
                p.maxDurBonus += 50; p.dur = Mathf.Max(20, p.dur * 0.7f); break;
            case UpgradeType.BloodOath:
                p.dmgBonus -= 8; p.bloodOathPending = true; p.bloodOathKills = 0; break;

            case UpgradeType.OverloadReaction: p.overloadReactionStack++; break;
            case UpgradeType.BombardmentMode: p.bombardmentModeStack++; break;
            case UpgradeType.VolleyProtocol: p.volleyProtocolStack++; break;
            case UpgradeType.Bloodlust: p.bloodlustStack++; break;
            case UpgradeType.ShieldCharge: p.shieldChargeStack++; break;
            case UpgradeType.Revive: p.reviveStack++; break;
            case UpgradeType.DeathRing: p.deathRingStack++; break;
            case UpgradeType.FireBreath: p.fireBreathStack++; break;
        }
    }
}
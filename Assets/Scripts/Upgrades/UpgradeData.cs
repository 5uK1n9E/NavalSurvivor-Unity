using UnityEngine;

public enum UpgradeType
{
    // 绿色
    MaxDur, BulletDmg, FireRate, FuelSave, MoveSpd, Armor,
    BulletSize, SmallerPlayer, SlowEnemyBullets,
    // 蓝色
    BulletBurn, BulletChain, MissileShunt, CounterStrike, Lifesteal, FullHpPower,
    // 紫色
    PierceRound, HeavyArmor, CollisionDamage, AutoAim, Bombardment, BloodPact, BloodOath,
    // 金色（只能获取一次）
    OverloadReaction, BombardmentMode, VolleyProtocol, Bloodlust, ShieldCharge, Revive, DeathRing, FireBreath
}

public enum UpgradeQuality { Common, Rare, Epic, Gold }

public struct UpgradeData
{
    public UpgradeType type;
    public string name;
    public string description;
    public UpgradeQuality quality;

    public UpgradeData(UpgradeType type, string name, string desc, UpgradeQuality quality)
    {
        this.type = type;
        this.name = name;
        this.description = desc;
        this.quality = quality;
    }
}
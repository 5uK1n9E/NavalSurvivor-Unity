// CoordUtil.cs
using UnityEngine;

public static class CoordUtil
{
    public static Vector2 PyToUnity(float pyX, float pyY)
    {
        float ux = pyX - GameConfig.OriginOffsetX;
        float uy = GameConfig.OriginOffsetY - pyY;
        return new Vector2(ux, uy);
    }

    public static Vector2 UnityToPy(float ux, float uy)
    {
        float pyX = ux + GameConfig.OriginOffsetX;
        float pyY = GameConfig.OriginOffsetY - uy;
        return new Vector2(pyX, pyY);
    }

    public static float PyAngleToUnityZ(float pyAngleRad)
    {
        return pyAngleRad * Mathf.Rad2Deg;
    }

    public static float Ang(Vector2 from, Vector2 to)
    {
        return Mathf.Atan2(to.y - from.y, to.x - from.x);
    }

    public static float NormalizeAngle(float rad)
    {
        while (rad > Mathf.PI) rad -= Mathf.PI * 2f;
        while (rad < -Mathf.PI) rad += Mathf.PI * 2f;
        return rad;
    }
}
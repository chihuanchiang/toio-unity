using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.MathUtils;
using static System.Math;

public class Island{
    public enum Type {Normal, Dummy, PowerUpHp, PowerUpAtk, PowerUpDex, Prison};

    public Island(Vector pos, float radius) {
        Pos = pos;
        Radius = radius;
        SetType(Type.Normal);

        float scale = 0.56f / 410f;
        Pos3 = new Vector3((float)Pos.x - originX, 0.0f, originY - (float)Pos.y);
        Pos3 *= scale;
        Radius3 = Radius * scale;
    }

#if (UNITY_EDITOR || UNITY_STANDALONE)
    public static float originX = 455f, originY = 250f;
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
    public static float originX = 402f, originY = 358f;
#endif

    public Color Color { get; set; }
    public Vector Pos { get; set; }
    public Vector3 Pos3 { get; set; }
    public float Radius { get; set; }
    public float Radius3 { get; set; }
    public Type type { get; set; }

    public void SetType(Type type) {
        this.type = type;
        switch (type) {
            case Type.Normal:
                Color = new Color(0, 0, 1, 0.3f);
                break;
            case Type.Dummy:
                Color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                break;
            case Type.PowerUpHp:
                Color = new Color(0, 1, 0, 0.3f);
                break;
            case Type.PowerUpAtk:
                Color = new Color(1, 0, 0, 0.3f);
                break;
            case Type.PowerUpDex:
                Color = new Color(1, 0.6f, 0.2f, 0.3f);
                break;
            case Type.Prison:
                Color = new Color(0, 0, 0, 0.3f);
                break;
            default:
                Debug.LogError("Wrong island type");
                break;
        }
    }
}

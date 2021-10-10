using System.Collections;
using System.Collections.Generic;
using static System.Math;
using UnityEngine;
using toio;
using toio.MathUtils;

public class Island {
    public enum Types {Normal, Dummy, PowerUpHp, PowerUpStr, PowerUpLuck, Prison};

#if (UNITY_EDITOR || UNITY_STANDALONE)
    public const float OriginX = 455f, OriginY = 250f;
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
    public const float OriginX = 402f, OriginY = 358f;
#endif

    public Color Color;
    public Vector Pos;
    public Vector3 Pos3;
    public float Radius;
    public float Radius3;
    public Types Type;

    public Island(Vector pos, float radius) {
        Pos = pos;
        Radius = radius;
        SetType(Types.Normal);

        float scale = 0.56f / 410f;
        Pos3 = new Vector3((float)Pos.x - OriginX, 0.0f, OriginY - (float)Pos.y);
        Pos3 *= scale;
        Radius3 = Radius * scale;
    }

    public void SetType(Types type) {
        Type = type;
        switch (type) {
            case Types.Normal:
                Color = new Color(0, 0, 1, 0.3f);
                break;
            case Types.Dummy:
                Color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
                break;
            case Types.PowerUpHp:
                Color = new Color(0, 1, 0, 0.3f);
                break;
            case Types.PowerUpStr:
                Color = new Color(1, 0, 0, 0.3f);
                break;
            case Types.PowerUpLuck:
                Color = new Color(1, 0.6f, 0.2f, 0.3f);
                break;
            case Types.Prison:
                Color = new Color(0, 0, 0, 0.3f);
                break;
            default:
                Debug.LogError("Wrong island type");
                break;
        }
    }
}

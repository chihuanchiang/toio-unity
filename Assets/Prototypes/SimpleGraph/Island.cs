using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toio;
using toio.MathUtils;
using static System.Math;

public class Island{
    public Island(Vector pos, Color color, float radius) {
        Pos = pos;
        Radius = radius;
        Color = color;

#if (UNITY_EDITOR || UNITY_STANDALONE)
        originX = 455f;
        originY = 250f;
#elif (UNITY_IOS || UNITY_ANDROID || UNITY_WEBGL)
        originX = 402f;
        originY = 358f;
#endif
        float scale = 0.56f / 410f;
        Pos3 = new Vector3((float)Pos.x - originX, 0.0f, originY - (float)Pos.y);
        Pos3 *= scale;
        Radius3 = Radius * scale;
    }

    public float originX { get; set; }
    public float originY { get; set; }
    public Color Color { get; set; }
    public Vector Pos { get; set; }
    public Vector3 Pos3 { get; set; }
    public float Radius { get; set; }
    public float Radius3 { get; set; }
}

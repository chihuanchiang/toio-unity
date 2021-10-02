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

        float scale = 0.56f / 410f;
        float originX = 455f, originY = 250f;
        Pos3 = new Vector3((float)Pos.x - originX, 0.0f, originY - (float)Pos.y);
        Pos3 *= scale;
        Radius3 = Radius * scale;
    }
    public Color Color { get; set; }
    public Vector Pos { get; set; }
    public Vector3 Pos3 { get; set; }
    public float Radius { get; set; }
    public float Radius3 { get; set; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BezierCuverUtils
{
    public static float QuadraticBezierCurve(float a, float b, float c, float time)
    {
        float ab = Mathf.Lerp(a, b, time);
        float bc = Mathf.Lerp(b, c, time);

        return Mathf.Lerp(ab, bc, time);
    }

}

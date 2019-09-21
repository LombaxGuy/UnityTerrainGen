using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelpfullFunctions
{
    public static bool IsPointInRectangle(Vector2 a, Vector2 b, Vector2 d, Vector2 p)
    {
        Vector2 AB = new Vector2(b.x - a.x, b.y - a.y);
        Vector2 AD = new Vector2(d.x - a.x, d.y - a.y);
        Vector2 AP = new Vector2(p.x - a.x, p.y - a.y);

        float dotAMAB = Vector2.Dot(AP, AB);
        float dotABAB = Vector2.Dot(AB, AB);
        float dotAPAD = Vector2.Dot(AP, AD);
        float dotADAD = Vector2.Dot(AD, AD);

        if (0 < dotAMAB && dotAMAB < dotABAB && 0 < dotAPAD && dotAPAD < dotADAD)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static Vector2 RotatePointAroundPivot(Vector2 point, Vector2 pivot, float anglesInDegrees)
    {
        Vector2 rotatedPoint;

        float tempX = point.x - pivot.x;
        float tempZ = point.y - pivot.y;

        float rotatedX = (tempX * Mathf.Cos(Mathf.Deg2Rad * anglesInDegrees)) - (tempZ * Mathf.Sin(Mathf.Deg2Rad * anglesInDegrees));
        float rotatedY = (tempX * Mathf.Sin(Mathf.Deg2Rad * anglesInDegrees)) + (tempZ * Mathf.Cos(Mathf.Deg2Rad * anglesInDegrees));

        rotatedPoint = new Vector2(rotatedX, rotatedY) + pivot;

        return rotatedPoint;
    }
}

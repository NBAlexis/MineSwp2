using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPuzzleCommonFunctions
{
    public static Vector3 plane = new Vector3(1.0f, 0.0f, 1.0f);
    public static Rect rectIdentity = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

    static public float PointToLineSq(Vector2 point, Vector2 linep1, Vector2 linep2, out Vector2 vPos, bool bOnLine = false)
    {
        float fLineWidthSq = (linep1 - linep2).sqrMagnitude;
        vPos = Vector2.Dot(linep1 - linep2, point - linep2) * (linep1 - linep2) / fLineWidthSq + linep2;
        if (bOnLine)
        {
            if (Vector2.Dot(vPos - linep1, vPos - linep2) > 0.0f)
            {
                vPos = (vPos - linep1).sqrMagnitude > (vPos - linep2).sqrMagnitude ? linep2 : linep1;
            }
        }

        return (vPos - point).sqrMagnitude;
    }

    static public Vector2 PointToLine(Vector2 point, Vector2 linep1, Vector2 linep2)
    {
        float fLineWidthSq = (linep1 - linep2).sqrMagnitude;
        Vector2 vRetPos = Vector2.Dot(linep1 - linep2, point - linep2) * (linep1 - linep2) / fLineWidthSq + linep2;
        if (Vector2.Dot(vRetPos - linep1, vRetPos - linep2) > 0.0f)
        {
            vRetPos = (vRetPos - linep1).sqrMagnitude > (vRetPos - linep2).sqrMagnitude ? linep2 : linep1;
        }
        return vRetPos;
    }

    static public float PointToLine3Sq(Vector3 point, Vector3 linep1, Vector3 linep2, out Vector3 vPos, bool bOnLine = false)
    {
        float fLineWidthSq = (linep1 - linep2).sqrMagnitude;
        vPos = Vector3.Dot(linep1 - linep2, point - linep2) * (linep1 - linep2) / fLineWidthSq + linep2;
        if (bOnLine)
        {
            if (Vector3.Dot(vPos - linep1, vPos - linep2) > 0.0f)
            {
                vPos = (vPos - linep1).sqrMagnitude > (vPos - linep2).sqrMagnitude ? linep2 : linep1;
            }
        }

        return (vPos - point).sqrMagnitude;
    }

    static public bool IsLineCross(Vector2 vL1P1, Vector2 vL1P2, Vector2 vBot1, Vector2 vL2P1, Vector2 vL2P2, Vector2 vBot2, bool bWeak = false)
    {
        if (bWeak)
        {
            return Vector2.Dot(vL2P1 - vL1P1, vBot1) * Vector2.Dot(vL2P2 - vL1P1, vBot1) <= -0.0001f
                && Vector2.Dot(vL1P1 - vL2P1, vBot2) * Vector2.Dot(vL1P2 - vL2P1, vBot2) <= -0.0001f;
        }
        return Vector2.Dot(vL2P1 - vL1P1, vBot1) * Vector2.Dot(vL2P2 - vL1P1, vBot1) <= 0.0001f
            && Vector2.Dot(vL1P1 - vL2P1, vBot2) * Vector2.Dot(vL1P2 - vL2P1, vBot2) <= 0.0001f;
    }

    static public Vector2 LineCrossWithPoint(Vector2 vP1, Vector2 vP2, Vector2 vP3, Vector2 vP4)
    {
        Vector2 vBot1 = new Vector2(vP1.y - vP2.y, vP2.x - vP1.x);
        float fP3Right = Vector2.Dot((vP3 - vP1), vBot1);
        float fP4Right = Vector2.Dot((vP4 - vP1), vBot1);
        if (fP3Right * fP4Right >= -0.001f)
        {
            return Mathf.Abs(fP3Right) < Mathf.Abs(fP4Right) ? vP3 : vP4;
        }

        return vP3 + (vP4 - vP3) * fP3Right / (fP3Right - fP4Right);
    }

    static public Vector2 LineCrossWithPoint(Vector2 vP1, Vector2 vP2, Vector2 vP3, Vector2 vP4, out bool bHit)
    {
        Vector2 vBot1 = new Vector2(vP1.y - vP2.y, vP2.x - vP1.x);
        Vector2 vBot2 = new Vector2(vP3.y - vP4.y, vP4.x - vP3.x);
        float fP3Right = Vector2.Dot((vP3 - vP1), vBot1);
        float fP4Right = Vector2.Dot((vP4 - vP1), vBot1);
        float fP1Right = Vector2.Dot((vP1 - vP3), vBot2);
        float fP2Right = Vector2.Dot((vP2 - vP3), vBot2);

        if (fP3Right * fP4Right >= -0.001f || fP1Right * fP2Right >= -0.001f)
        {
            bHit = false;
            return (vP3 - vP1).sqrMagnitude < (vP4 - vP1).sqrMagnitude ? vP3 : vP4;
        }

        bHit = true;
        return vP3 + (vP4 - vP3) * fP3Right / (fP3Right - fP4Right);
    }

    static public bool IsParall(Vector2 line1p1, Vector2 line1p2, Vector2 line2p1, Vector2 line2p2)
    {
        Vector2 dir1 = (line1p2 - line1p1).normalized;
        Vector2 dir2 = (line2p2 - line2p1).normalized;
        return Vector2.Dot(dir1, dir2) < -0.95f || Vector2.Dot(dir1, dir2) > 0.95f;
    }
}

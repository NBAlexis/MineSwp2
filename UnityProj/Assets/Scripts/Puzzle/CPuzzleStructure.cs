using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Mono.Cecil;
using UnityEngine;


public class CLine
{
    public Vector2 m_vPos1;
    public Vector2 m_vPos2;

    public float GetLength()
    {
        return (m_vPos1 - m_vPos2).magnitude;
    }

    public List<CCrossPoint> m_pCrosses;
    public List<CNode> m_pNodes;
}

public class CCrossPoint
{
    public Vector2 m_vPos;
    public CLine m_pLine1;
    public CLine m_pLine2;
}

public class CNode
{
    public Vector2 m_vCenter;
    public List<CLine> m_pLines;
    public List<CCrossPoint> m_pCrossPoints;
}

public class CPuzzleStructure
{
    public int m_iWidth;
    public int m_iHeight;
    public Vector2 m_vSeps;

    public List<CLine> m_pLines;

    public bool HasDuplicatedStraightLine(CLine line)
    {
        int iP1X; int iP1Y; int iP2X; int iP2Y;
        CPuzzleCreator.GetGrid(this, line.m_vPos1, out iP1X, out iP1Y);
        CPuzzleCreator.GetGrid(this, line.m_vPos2, out iP2X, out iP2Y);
        if (iP1X == iP2X && iP1Y == iP2Y)
        {
            return true;
        }

        if (iP1X != iP2X && iP1Y != iP2Y)
        {
            //not straight
            return false;
        }

        if (line.GetLength() > 0.5f * CPuzzleCreator.m_fScreenHeight)
        {
            return true;
        }

        for (int i = 0; i < m_pLines.Count; ++i)
        {
            int iLineP1X; int iLineP1Y; int iLineP2X; int iLineP2Y;
            CPuzzleCreator.GetGrid(this, m_pLines[i].m_vPos1, out iLineP1X, out iLineP1Y);
            CPuzzleCreator.GetGrid(this, m_pLines[i].m_vPos2, out iLineP2X, out iLineP2Y);

            if (iP1X == iP2X)
            {
                if (iLineP1X == iP1X && iLineP2X == iP1X)
                {
                    if ((iLineP1Y - iP1Y)*(iLineP2Y - iP2Y) <= 0
                     || (iLineP2Y - iP1Y)*(iLineP1Y - iP2Y) <= 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (iLineP1Y == iP1Y && iLineP2Y == iP1Y)
                {
                    if ((iLineP1X - iP1X) * (iLineP2X - iP2X) <= 0
                     || (iLineP2X - iP1X) * (iLineP1X - iP2X) <= 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool HasDuplicatedLine(CLine line)
    {
        int iP1X; int iP1Y; int iP2X; int iP2Y;
        CPuzzleCreator.GetGrid(this, line.m_vPos1, out iP1X, out iP1Y);
        CPuzzleCreator.GetGrid(this, line.m_vPos2, out iP2X, out iP2Y);
        if (iP1X == iP2X && iP1Y == iP2Y)
        {
            return true;
        }

        if (line.GetLength() > 0.5f * CPuzzleCreator.m_fScreenWidth)
        {
            return true;
        }

        for (int i = 0; i < m_pLines.Count; ++i)
        {
            int iLineP1X; int iLineP1Y; int iLineP2X; int iLineP2Y;
            CPuzzleCreator.GetGrid(this, m_pLines[i].m_vPos1, out iLineP1X, out iLineP1Y);
            CPuzzleCreator.GetGrid(this, m_pLines[i].m_vPos2, out iLineP2X, out iLineP2Y);

            if (iP1X == iLineP1X
             && iP1Y == iLineP1Y
             && iP2X == iLineP2X
             && iP2Y == iLineP2Y)
            {
                return true;
            }

            if (CPuzzleCommonFunctions.IsParall(line.m_vPos1, line.m_vPos2, m_pLines[i].m_vPos1, m_pLines[i].m_vPos2))
            {
                Vector2 output;
                float fSq1 = CPuzzleCommonFunctions.PointToLineSq(line.m_vPos1, m_pLines[i].m_vPos1, m_pLines[i].m_vPos2, out output, true);
                float fSq2 = CPuzzleCommonFunctions.PointToLineSq(line.m_vPos2, m_pLines[i].m_vPos1, m_pLines[i].m_vPos2, out output, true);

                if (fSq1 < m_vSeps.x * m_vSeps.x
                 || fSq2 < m_vSeps.x * m_vSeps.x)
                {
                    return true;
                }                
            }
        }
        return false;
    }
}

public class CPuzzleCreator
{
    public const float m_fScreenWidth = 12.8f;
    public const float m_fScreenHeight = 7.2f;

    static public Vector2 GetRandomPosInGrid(CPuzzleStructure puzzle, int iX, int iY, bool bStraight = false)
    {
        if (bStraight)
        {
            return new Vector2(iX * puzzle.m_vSeps.x + 0.5f * puzzle.m_vSeps.x,
                               iY * puzzle.m_vSeps.y + 0.5f * puzzle.m_vSeps.y);            
        }
        return new Vector2(iX * puzzle.m_vSeps.x + Random.Range(0.0f, puzzle.m_vSeps.x),
                           iY * puzzle.m_vSeps.y + Random.Range(0.0f, puzzle.m_vSeps.y));
    }

    static public void GetGrid(CPuzzleStructure puzzle, Vector2 v2, out int iX, out int iY)
    {
        iX = Mathf.FloorToInt(v2.x / puzzle.m_vSeps.x);
        iY = Mathf.FloorToInt(v2.y / puzzle.m_vSeps.y);
    }

    static public CPuzzleStructure CreatePuzzle(int iWidth, int iHeight)
    {
        CPuzzleStructure puzzle = new CPuzzleStructure();
        puzzle.m_iWidth = iWidth;
        puzzle.m_iHeight = iHeight;
        puzzle.m_vSeps = new Vector2(m_fScreenWidth/iWidth, m_fScreenHeight/iHeight);
        CreateLines(puzzle);

        return puzzle;
    }

    static public void CreateLines(CPuzzleStructure puzzle)
    {
        puzzle.m_pLines = new List<CLine>();

        for (int i = 0; i < puzzle.m_iWidth * puzzle.m_iHeight * 32; ++i)
        {
            int p1x = Random.Range(0, puzzle.m_iWidth);
            int p1y = Random.Range(0, puzzle.m_iHeight);
            int p2x = Random.Range(0, puzzle.m_iWidth);
            int p2y = Random.Range(0, puzzle.m_iHeight);
            if (Random.Range(0.0f, 1.0f) > 0.5f)
            {
                p2x = p1x;
            }
            else
            {
                p2y = p1y;
            }

            CLine newLine = new CLine();
            newLine.m_vPos1 = GetRandomPosInGrid(puzzle, p1x, p1y);
            newLine.m_vPos2 = GetRandomPosInGrid(puzzle, p2x, p2y);
            if (!puzzle.HasDuplicatedStraightLine(newLine))
            {
                puzzle.m_pLines.Add(newLine);
            }
        }

        for (int i = 0; i < puzzle.m_iWidth * puzzle.m_iHeight * 6; ++i)
        {
            int p1x = Random.Range(0, puzzle.m_iWidth);
            int p1y = Random.Range(0, puzzle.m_iHeight);
            int p2x = Random.Range(0, puzzle.m_iWidth);
            int p2y = Random.Range(0, puzzle.m_iHeight);

            CLine newLine = new CLine();
            newLine.m_vPos1 = GetRandomPosInGrid(puzzle, p1x, p1y);
            newLine.m_vPos2 = GetRandomPosInGrid(puzzle, p2x, p2y);
            if (!puzzle.HasDuplicatedLine(newLine)
             && !puzzle.HasDuplicatedStraightLine(newLine))
            {
                puzzle.m_pLines.Add(newLine);
            }
        }
    }

    static public void CalculateLineCrosses(CPuzzleStructure puzzle)
    {
        
    }

    static public void CalculateNodes(CPuzzleStructure puzzle)
    {
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ELineType
{
    ELT_Normal,
    ELT_Frozen,
    ELT_Empty,
}

public class CLines
{
    public int m_iStartX;
    public int m_iStartY;
    public int m_iEndX;
    public int m_iEndY;

    public List<int> m_iNodeXs;
    public List<int> m_iNodeYs;

    public ELineType m_iType;
}

public class CPuzzle
{
    public byte[,] m_byGrids;
    public CLines[] m_pLines;
    public bool m_bValid = true;

    public bool HasGrid(int iX, int iY)
    {
        return iX >= 0 && iX < m_byGrids.GetLength(0) && iY >= 0 && iY < m_byGrids.GetLength(1);
    }

    public bool HasWall(EDir eDir, int iX, int iY)
    {
        if (iX < 0)
        {
            return eDir == EDir.RU || eDir == EDir.Right;
        }
        if (iX > m_byGrids.GetLength(0) - 1)
        {
            return eDir == EDir.LD || eDir == EDir.Left;
        }
        if (iY < 0)
        {
            return eDir == EDir.LD || eDir == EDir.Down;
        }
        if (iY > m_byGrids.GetLength(0) - 1)
        {
            return eDir == EDir.RU || eDir == EDir.Up;
        }

        if (IsEmpty(iX, iY))
        {
            return true;
        }

        return 0 != (m_byGrids[iX, iY] & (1 << (int) eDir));
    }

    public bool HasBomb(int iX, int iY)
    {
        return 0 != (m_byGrids[iX, iY] & (1 << (int)EDir.Max));
    }

    public void PutBomb(int iX, int iY)
    {
        m_byGrids[iX, iY] |= (byte)(1 << (int) EDir.Max);
    }

    public bool IsEmpty(int iX, int iY)
    {
        return 0 != (m_byGrids[iX, iY] & (1 << ((int)EDir.Max + 1)));
    }

    public void SetEmpty(int iX, int iY)
    {
        m_byGrids[iX, iY] |= (byte)(1 << ((int)EDir.Max + 1));
    }

    #region Generate Puzzle

    public void PutEmptys(int iEmptyNumber)
    {
        List<int> possibleX = new List<int>();
        List<int> possibleY = new List<int>();
        for (int i = 0; i < m_byGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_byGrids.GetLength(1); ++j)
            {
                possibleX.Add(i);
                possibleY.Add(j);
            }
        }

        if (possibleX.Count < iEmptyNumber)
        {
            m_bValid = false;
            return;
        }

        for (int i = 0; i < iEmptyNumber; ++i)
        {
            int iChoice = Random.Range(0, possibleX.Count);
            SetEmpty(possibleX[iChoice], possibleY[iChoice]);
            possibleX.RemoveAt(iChoice);
        }
    }

    public void AddLines()
    {
        List<CLines> lines = new List<CLines>();
        for (int i = 0; i < m_byGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_byGrids.GetLength(1); ++j)
            {
                if (!IsEmpty(i, j))
                {
                    for (int k = 0; k < (int)EDir.Max / 2; ++k)
                    {
                        EDir eOp = PuzzleCreator.GetOp((EDir) k);
                        if (HasWall(eOp, i, j)
                         || HasWall((EDir)k, i + PuzzleCreator.deltaX[(int)eOp], j + PuzzleCreator.deltaY[(int)eOp]))
                        {
                            //start point
                            CLines line = new CLines();
                            line.m_iStartX = i;
                            line.m_iStartY = j;
                            line.m_iNodeXs = new List<int>();
                            line.m_iNodeYs = new List<int>();

                            bool bMeetWall = false;
                            int iEndX = i;
                            int iEndY = j;
                            for (int l = 0; !bMeetWall; ++l)
                            {
                                int iCheckPosX = i + PuzzleCreator.deltaX[k] * l;
                                int iCheckPosY = j + PuzzleCreator.deltaY[k] * l;
                                line.m_iNodeXs.Add(iCheckPosX);
                                line.m_iNodeYs.Add(iCheckPosY);

                                if (HasWall((EDir)k, iCheckPosX, iCheckPosY))
                                {
                                    bMeetWall = true;
                                    iEndX = iCheckPosX;
                                    iEndY = iCheckPosY;
                                }

                                if (!bMeetWall)
                                {
                                    int iCheckPosX1 = i + PuzzleCreator.deltaX[k] * (l + 1);
                                    int iCheckPosY1 = j + PuzzleCreator.deltaY[k] * (l + 1);

                                    if (HasWall(PuzzleCreator.GetOp((EDir)k), iCheckPosX1, iCheckPosY1))
                                    {
                                        bMeetWall = true;
                                        iEndX = iCheckPosX;
                                        iEndY = iCheckPosY;
                                    }
                                }
                            }

                            if (iEndX != line.m_iStartX
                             || iEndY != line.m_iStartY)
                            {
                                line.m_iEndX = iEndX;
                                line.m_iEndY = iEndY;
                                lines.Add(line);
                            }
                        }
                    }
                }
            }
        }
        m_pLines = lines.ToArray();        
    }

    public void PutBombs(int iBombNumber)
    {
        int iMaxTry = 200;
        while (iBombNumber > 0 && iMaxTry > 0)
        {
            --iMaxTry;
            if (0 == iMaxTry)
            {
                Debug.LogWarning("Wrong Puzzle 1!");
                m_bValid = false;
                break;
            }
            int iX = Random.Range(0, m_byGrids.GetLength(0));
            int iY = Random.Range(0, m_byGrids.GetLength(1));
            if (!HasBomb(iX, iY) && !IsEmpty(iX, iY))
            {
                PutBomb(iX, iY);
                --iBombNumber;
            }
        }
    }

    public void PutWalls(int iWallNumber)
    {
        int iMaxTry = 1000;
        while (iWallNumber > 0 && iMaxTry > 0)
        {
            --iMaxTry;
            if (0 == iMaxTry)
            {
                Debug.LogWarning("Wrong Puzzle 2!");
                m_bValid = false;
                break;
            }
            int iX = Random.Range(0, m_byGrids.GetLength(0));
            int iY = Random.Range(0, m_byGrids.GetLength(1));
            List<int> walls = new List<int>
            {
                (int) EDir.Left,
                (int) EDir.Right,
                (int) EDir.Up,
                (int) EDir.Down,
                (int) EDir.LD,
                (int) EDir.RU
            };

            for (int i = 0; i < (int)EDir.Max; ++i)
            {
                //if me have left wall, remove left wall
                if (HasWall((EDir)i, iX, iY))
                {
                    walls.Remove(i);
                }
            }

            for (int i = 0; i < (int)EDir.Max; ++i)
            {
                //i is left
                //op is right
                //if my right, has left wall
                //remove my right wall
                EDir op = PuzzleCreator.GetOp((EDir)i);
                if (HasWall((EDir)i, iX + PuzzleCreator.deltaX[(int)op], iY + PuzzleCreator.deltaY[(int)op]))
                {
                    walls.Remove((int)op);
                }
            }

            if (walls.Count > 1)
            {
                int iWall = walls[Random.Range(0, walls.Count)];
                m_byGrids[iX, iY] = (byte)(m_byGrids[iX, iY] | (1 << iWall));
                --iWallNumber;
            }
        }
    }

    #endregion
}

public enum EDir
{
    RU,
    Right,
    Down,
    LD,
    Left,
    Up,

    Max,
}

public class PuzzleCreator 
{
    public static EDir GetOp(EDir dir)
    {
        return (EDir)(((int)dir + (int)EDir.Max / 2) % ((int)EDir.Max));
    }

    public static readonly int[] deltaX =
    {
        1, 1, 0, -1, -1, 0
    };

    public static readonly int[] deltaY =
    {
        -1, 0, 1, 1, 0, -1
    };

    static public CPuzzle CreatePuzzle(int iWidth, int iHeight, int iWallNumber, int iEmptyNumber)
    {
        CPuzzle ret = new CPuzzle();
        ret.m_byGrids = new byte[iWidth, iHeight];
        ret.PutEmptys(iEmptyNumber);
        ret.PutWalls(iWallNumber);
        ret.AddLines();

        return ret;
    }
}

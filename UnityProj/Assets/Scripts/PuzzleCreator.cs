using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ELineType
{
    ELT_Normal,
    ELT_Frozen,
}

public class CLines
{
    public int m_iStartX;
    public int m_iStartY;
    public int m_iEndX;
    public int m_iEndY;

    public List<int> m_iNodeXs;
    public List<int> m_iNodeYs;

    public ELineType m_eType = ELineType.ELT_Normal;
}

public class CPuzzle
{
    public ushort[,] m_ushGrids;
    public CLines[] m_pLines;
    public bool m_bValid = true;

    public int m_iGoldNumber = 0;
    public int m_iMouseNumber = 0;
    public int m_iBombNumber = 0;
    public int m_iGhostBombNumber = 0;

    public byte[] ToByteArray()
    {
        using (BinaryWriter brw = new BinaryWriter(new MemoryStream()))
        {
            brw.Write((byte)m_iGoldNumber);
            brw.Write((byte)m_iMouseNumber);
            brw.Write((byte)m_iBombNumber);
            brw.Write((byte)m_iGhostBombNumber);
            brw.Write((byte)m_ushGrids.GetLength(0));
            brw.Write((byte)m_ushGrids.GetLength(1));
            for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
            {
                for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
                {
                    brw.Write(m_ushGrids[i, j]);
                }                
            }

            brw.Write(m_pLines.Length);
            for (int i = 0; i < m_pLines.Length; ++i)
            {
                brw.Write((byte)m_pLines[i].m_iStartX);
                brw.Write((byte)m_pLines[i].m_iStartY);
                brw.Write((byte)m_pLines[i].m_iEndX);
                brw.Write((byte)m_pLines[i].m_iEndY);
                brw.Write((byte)m_pLines[i].m_eType);

                brw.Write((byte)m_pLines[i].m_iNodeXs.Count);
                for (int j = 0; j < m_pLines[i].m_iNodeXs.Count; ++j)
                {
                    brw.Write((byte)m_pLines[i].m_iNodeXs[j]);
                    brw.Write((byte)m_pLines[i].m_iNodeYs[j]);
                }
            }

            brw.Flush();

            int iLength = (int)brw.BaseStream.Length;
            using (BinaryReader binReader = new BinaryReader(brw.BaseStream))
            {
                binReader.BaseStream.Position = 0;
                byte[] bytes = binReader.ReadBytes(iLength);
                brw.BaseStream.Read(bytes, 0, iLength);
                return bytes;
            }
        }
    }

    public void FromByteArray(byte[] data)
    {
        using (MemoryStream ms = new MemoryStream(data))
        {
            ms.Position = 0;
            BinaryReader bwr = new BinaryReader(ms);

            m_iGoldNumber = bwr.ReadByte();
            m_iMouseNumber = bwr.ReadByte();
            m_iBombNumber = bwr.ReadByte();
            m_iGhostBombNumber = bwr.ReadByte();
            int iWidth = bwr.ReadByte();
            int iHeight = bwr.ReadByte();
            m_ushGrids = new ushort[iWidth, iHeight];
            for (int i = 0; i < iWidth; ++i)
            {
                for (int j = 0; j < iHeight; ++j)
                {
                    m_ushGrids[i, j] = bwr.ReadUInt16();
                }                
            }

            int iLineCount = bwr.ReadInt32();
            List<CLines> lines = new List<CLines>();
            for (int i = 0; i < iLineCount; ++i)
            {
                CLines line = new CLines();
                line.m_iStartX = bwr.ReadByte();
                line.m_iStartY = bwr.ReadByte();
                line.m_iEndX = bwr.ReadByte();
                line.m_iEndY = bwr.ReadByte();

                line.m_eType = (ELineType)bwr.ReadByte();
                line.m_iNodeXs = new List<int>();
                line.m_iNodeYs = new List<int>();
                byte nodeCount = bwr.ReadByte();
                for (int j = 0; j < nodeCount; ++j)
                {
                    line.m_iNodeXs.Add(bwr.ReadByte());
                    line.m_iNodeYs.Add(bwr.ReadByte());
                }

                lines.Add(line);
            }
            m_pLines = lines.ToArray();
            ms.Close();
        }
    }

    public bool HasGrid(int iX, int iY)
    {
        return iX >= 0 && iX < m_ushGrids.GetLength(0) && iY >= 0 && iY < m_ushGrids.GetLength(1);
    }

    public bool HasWall(EDir eDir, int iX, int iY)
    {
        if (iX < 0)
        {
            return eDir == EDir.RU || eDir == EDir.Right;
        }
        if (iX > m_ushGrids.GetLength(0) - 1)
        {
            return eDir == EDir.LD || eDir == EDir.Left;
        }
        if (iY < 0)
        {
            return eDir == EDir.LD || eDir == EDir.Down;
        }
        if (iY > m_ushGrids.GetLength(0) - 1)
        {
            return eDir == EDir.RU || eDir == EDir.Up;
        }

        if (IsEmpty(iX, iY))
        {
            return true;
        }

        return 0 != (m_ushGrids[iX, iY] & (1 << (int)eDir));
    }

    public bool HasBomb(int iX, int iY)
    {
        return (0 != (m_ushGrids[iX, iY] & (1 << (int)EDir.Bomb))) || (0 != (m_ushGrids[iX, iY] & (1 << (int)EDir.GhostBomb)));
    }

    public bool HasGhostBomb(int iX, int iY)
    {
        return (0 != (m_ushGrids[iX, iY] & (1 << (int)EDir.GhostBomb)));
    }

    public void PutBomb(int iX, int iY)
    {
        m_ushGrids[iX, iY] |= (ushort)(1 << (int)EDir.Bomb);
    }

    public void PutGhostBomb(int iX, int iY)
    {
        m_ushGrids[iX, iY] |= (ushort)(1 << (int)EDir.GhostBomb);
    }

    public void RemoveBomb(int iX, int iY)
    {
        ushort b1 = (1 << (int)EDir.Bomb);
        b1 = (ushort)(~b1);
        m_ushGrids[iX, iY] = (ushort)(b1 & m_ushGrids[iX, iY]);

        b1 = (1 << (int)EDir.GhostBomb);
        b1 = (ushort)(~b1);
        m_ushGrids[iX, iY] = (ushort)(b1 & m_ushGrids[iX, iY]);
    }

    public bool IsEmpty(int iX, int iY)
    {
        return 0 != (m_ushGrids[iX, iY] & (1 << (int)EDir.Max));
    }

    public void SetEmpty(int iX, int iY)
    {
        m_ushGrids[iX, iY] |= (ushort)(1 << (int)EDir.Max);
    }

    public int GetBombNumber()
    {
        int iNum = 0;
        for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
            {
                if (HasBomb(i, j))
                {
                    ++iNum;
                }
            }
        }
        return iNum;
    }

    #region Generate Puzzle

    public void PutEmptys(int iEmptyNumber)
    {
        List<int> possibleX = new List<int>();
        List<int> possibleY = new List<int>();
        for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
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

    public void AddLines(int iFrozenLineNumber)
    {
        List<CLines> lines = new List<CLines>();
        for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
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

        //========================================
        //we have lines, we can check nodes without lines
        for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
            {
                if (!IsEmpty(i, j))
                {
                    bool bHasMe = false;
                    for (int k = 0; k < m_pLines.Length; ++k)
                    {
                        for (int l = 0; l < m_pLines[k].m_iNodeXs.Count; ++l)
                        {
                            if (m_pLines[k].m_iNodeXs[l] == i
                             && m_pLines[k].m_iNodeYs[l] == j)
                            {
                                bHasMe = true;
                                break;
                            }
                        }
                        if (bHasMe)
                        {
                            break;
                        }
                    }
                    if (!bHasMe)
                    {
                        SetEmpty(i, j);
                    }
                }
            }
        }

        
        for (int i = 0; i < iFrozenLineNumber; ++i)
        {
            int iChoose = Random.Range(0, m_pLines.Length);
            int iTry = 200;
            while (m_pLines[iChoose].m_eType == ELineType.ELT_Frozen && iTry > 0)
            {
                --iTry;
                iChoose = Random.Range(0, m_pLines.Length);
            }
            m_pLines[iChoose].m_eType = ELineType.ELT_Frozen;
        }
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
            int iX = Random.Range(0, m_ushGrids.GetLength(0));
            int iY = Random.Range(0, m_ushGrids.GetLength(1));
            if (!HasBomb(iX, iY) && !IsEmpty(iX, iY))
            {
                int iHead = 0;
                int iLine = 0;
                for (int i = 0; i < m_pLines.Length; ++i)
                {
                    if (iX == m_pLines[i].m_iStartX && iY == m_pLines[i].m_iStartY)
                    {
                        ++iHead;
                    }
                    else if (iX == m_pLines[i].m_iEndX && iY == m_pLines[i].m_iEndY)
                    {
                        ++iHead;
                    }
                    for (int j = 0; j < m_pLines[i].m_iNodeXs.Count; ++j)
                    {
                        if (iX == m_pLines[i].m_iNodeXs[j] && iY == m_pLines[i].m_iNodeYs[j])
                        {
                            ++iLine;
                        }
                    }
                }
                if (1 != iHead || 1 != iLine)
                {
                    PutBomb(iX, iY);
                    --iBombNumber;                    
                }
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
            int iX = Random.Range(0, m_ushGrids.GetLength(0));
            int iY = Random.Range(0, m_ushGrids.GetLength(1));
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
                m_ushGrids[iX, iY] = (ushort)(m_ushGrids[iX, iY] | (1 << iWall));
                --iWallNumber;
            }
        }
    }

    public void PutGhostBomb(int iGhostBombNumber)
    {
        int iMaxTry = 200;
        while (iGhostBombNumber > 0 && iMaxTry > 0)
        {
            --iMaxTry;
            if (0 == iMaxTry)
            {
                Debug.LogWarning("Wrong Puzzle 1!");
                m_bValid = false;
                break;
            }
            int iX = Random.Range(0, m_ushGrids.GetLength(0));
            int iY = Random.Range(0, m_ushGrids.GetLength(1));
            if (!HasBomb(iX, iY) && !IsEmpty(iX, iY))
            {
                int iHead = 0;
                int iLine = 0;
                for (int i = 0; i < m_pLines.Length; ++i)
                {
                    if (iX == m_pLines[i].m_iStartX && iY == m_pLines[i].m_iStartY)
                    {
                        ++iHead;
                    }
                    else if (iX == m_pLines[i].m_iEndX && iY == m_pLines[i].m_iEndY)
                    {
                        ++iHead;
                    }
                    for (int j = 0; j < m_pLines[i].m_iNodeXs.Count; ++j)
                    {
                        if (iX == m_pLines[i].m_iNodeXs[j] && iY == m_pLines[i].m_iNodeYs[j])
                        {
                            ++iLine;
                        }
                    }
                }
                if (1 != iHead || 1 != iLine)
                {
                    PutGhostBomb(iX, iY);
                    --iGhostBombNumber;
                }
            }
        }
    }

    public bool CheckConnection()
    {
        byte[,] dots = new byte[m_ushGrids.GetLength(0), m_ushGrids.GetLength(1)];
        bool bDrawStart = false;
        for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
            {
                if (!IsEmpty(i, j))
                {
                    if (!bDrawStart)
                    {
                        dots[i, j] = 1;
                        bDrawStart = true;
                    }
                    else
                    {
                        dots[i, j] = 0;
                    }
                }
                else
                {
                    dots[i, j] = 2;
                }
            }            
        }

        bool bHasNew = true;
        while (bHasNew)
        {
            bHasNew = false;
            for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
            {
                for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
                {
                    if (1 == dots[i, j])
                    {
                        for (int k = 0; k < m_pLines.Length; ++k)
                        {
                            bool bIsLine = false;
                            for (int l = 0; l < m_pLines[k].m_iNodeXs.Count; ++l)
                            {
                                if (m_pLines[k].m_iNodeXs[l] == i
                                 && m_pLines[k].m_iNodeYs[l] == j)
                                {
                                    bIsLine = true;
                                    break;
                                }
                            }

                            if (bIsLine)
                            {
                                for (int l = 0; l < m_pLines[k].m_iNodeXs.Count; ++l)
                                {
                                    if (0 == dots[m_pLines[k].m_iNodeXs[l], m_pLines[k].m_iNodeYs[l]])
                                    {
                                        dots[m_pLines[k].m_iNodeXs[l], m_pLines[k].m_iNodeYs[l]] = 1;
                                        bHasNew = true;
                                    }
                                }
                            }
                        }
                        dots[i, j] = 2;
                    }
                }
            }
        }

        for (int i = 0; i < m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_ushGrids.GetLength(1); ++j)
            {
                if (0 == dots[i, j])
                {
                    return false;
                }
            }
        }

        return true;
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

    Max, //6

    Bomb,
    GhostBomb,
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

    static public CPuzzle CreatePuzzle(int iWidth, int iHeight, int iWallNumber, int iEmptyNumber, int iFrozenLineNumber)
    {
        CPuzzle ret = new CPuzzle();
        ret.m_ushGrids = new ushort[iWidth, iHeight];
        ret.PutEmptys(iEmptyNumber);
        ret.PutWalls(iWallNumber);
        ret.AddLines(iFrozenLineNumber);

        return ret;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPuzzle
{
    public byte[,] m_byGrids;
}

public enum EDir
{
    Up,
    Right,
    Down,
    Left,
}

public class PuzzleCreator : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
		
	}

    static public CPuzzle CreatePuzzle(int iWidth, int iHeight, int iMineNumber, int iWallNumber)
    {
        CPuzzle ret = new CPuzzle();
        ret.m_byGrids = new byte[iWidth, iHeight];

        while (iMineNumber > 0)
        {
            int iX = Random.Range(0, iWidth);
            int iY = Random.Range(0, iHeight);
            if (0 == (ret.m_byGrids[iX, iY] & 32))
            {
                ret.m_byGrids[iX, iY] = (byte)(ret.m_byGrids[iX, iY] | (byte)32);
                --iMineNumber;
            }
        }

        while (iWallNumber > 0)
        {
            int iX = Random.Range(0, iWidth);
            int iY = Random.Range(0, iHeight);
            List<int> walls = new List<int>();
            walls.Add(0);
            walls.Add(1);
            walls.Add(2);
            walls.Add(3);
            for (int i = 0; i < 4; ++i)
            {
                if (0 != (ret.m_byGrids[iX, iY] & (1 << i)))
                {
                    walls.Remove(i);
                }
            }

            if (0 == iX && walls.Contains(3))
            {
                walls.Remove(3);
            }
            if (iWidth - 1 == iX && walls.Contains(1))
            {
                walls.Remove(1);
            }
            if (0 == iY && walls.Contains(0))
            {
                walls.Remove(0);
            }
            if (iHeight - 1 == iY && walls.Contains(2))
            {
                walls.Remove(2);
            }

            //left grid have right wall
            if (iX - 1 >= 0 && walls.Contains(3) && (0 != (ret.m_byGrids[iX - 1, iY] & (1 << 1))))
            {
                walls.Remove(3);
            }
            //right grid have left wall
            if (iX + 1 < iWidth && walls.Contains(1) && (0 != (ret.m_byGrids[iX + 1, iY] & (1 << 3))))
            {
                walls.Remove(1);
            }
            //up grid have down wall
            if (iY - 1 >= 0 && walls.Contains(0) && (0 != (ret.m_byGrids[iX, iY - 1] & (1 << 2))))
            {
                walls.Remove(0);
            }
            //down grid have up wall
            if (iY + 1 < iHeight && walls.Contains(2) && (0 != (ret.m_byGrids[iX, iY + 1] & (1 << 0))))
            {
                walls.Remove(2);
            }

            if (walls.Count > 1)
            {
                int iWall = walls[Random.Range(0, walls.Count)];
                ret.m_byGrids[iX, iY] = (byte)(ret.m_byGrids[iX, iY] | (1 << iWall));
                --iWallNumber;
            }
        }

        return ret;
    }
}

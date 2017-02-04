using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EPage
{
    EP_Start,
    EP_Game,
    EP_Result,
}

public class AUI : MonoBehaviour
{

    public GameObject[] m_pPages;

	// Use this for initialization
	void Start () 
    {
        m_pPages[(int)EPage.EP_Start].SetActive(true);
        m_pPages[(int)EPage.EP_Game].SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

    }

    #region Start

    public void OnStartGameButton()
    {
        CPuzzle newPuzzle = PuzzleCreator.CreatePuzzle(10, 15, MineNumber, WallNumber);
        PutPuzzle(newPuzzle);

        m_pPages[(int)EPage.EP_Start].SetActive(false);
        m_pPages[(int)EPage.EP_Game].SetActive(true);

        m_pMineNumber.text = MineNumber.ToString();
        m_bOver = false;
        m_pRes.enabled = false;
    }

    #endregion

    #region Game

    public const int MineNumber = 40;
    public const int WallNumber = 40;
    public AGridToggle m_pOneGrid;
    public AGridToggle[,] m_pGrids;
    public CPuzzle m_pPuzzle;

    public Text m_pMineNumber;
    public Text m_pRes;
    public bool m_bOver = false;

    public void PutPuzzle(CPuzzle puzzle)
    {
        m_pPuzzle = puzzle;
        if (null != m_pGrids)
        {

            for (int i = 0; i < m_pGrids.GetLength(0); ++i)
            {
                for (int j = 0; j < m_pGrids.GetLength(1); ++j)
                {
                    if (null != m_pGrids[i, j])
                    {
                        m_pGrids[i, j].gameObject.name = "Discard";
                        Destroy(m_pGrids[i, j].gameObject);
                    }
                }
            }
        }

        m_pGrids = new AGridToggle[puzzle.m_byGrids.GetLength(0), puzzle.m_byGrids.GetLength(1)];

        for (int i = 0; i < puzzle.m_byGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < puzzle.m_byGrids.GetLength(1); ++j)
            {
                GameObject newGrid = Instantiate(m_pOneGrid.gameObject);
                newGrid.transform.SetParent(m_pOneGrid.transform.parent);

                AGridToggle newG = newGrid.GetComponent<AGridToggle>();
                newG.SetOwner(this);
                newG.m_iX = i;
                newG.m_iY = j;
                newG.PutToGridWithWH(puzzle.m_byGrids.GetLength(0), puzzle.m_byGrids.GetLength(1));
                newG.SetGridState(EGridState.EGS_Close);

                m_pGrids[i, j] = newG;
            }            
        }
    }

    public void OnTagMine(int iX, int iY)
    {
        if (m_bOver)
        {
            return;
        }

        bool[] bWall =
        {
            m_pGrids[iX, iY].m_pWalls[0].enabled,
            m_pGrids[iX, iY].m_pWalls[1].enabled,
            m_pGrids[iX, iY].m_pWalls[2].enabled,
            m_pGrids[iX, iY].m_pWalls[3].enabled,
        };

        if (EGridState.EGS_Close == m_pGrids[iX, iY].m_eState)
        {
            m_pGrids[iX, iY].SetGridState(EGridState.EGS_Tag);
        }
        else if (EGridState.EGS_Tag == m_pGrids[iX, iY].m_eState)
        {
            m_pGrids[iX, iY].SetGridState(EGridState.EGS_Close);
        }
        for (int i = 0; i < 4; ++i)
        {
            if (bWall[i])
            {
                m_pGrids[iX, iY].ShowWall(i);
            }
        }

        int iMineNumber = MineNumber;
        for (int i = 0; i < m_pPuzzle.m_byGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_byGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_Tag == m_pGrids[i, j].m_eState)
                {
                    --iMineNumber;
                }
            }            
        }
        m_pMineNumber.text = iMineNumber.ToString();
    }

    public void OnPressMine(int iX, int iY)
    {
        if (m_bOver)
        {
            return;
        }

        if (EGridState.EGS_Tag == m_pGrids[iX, iY].m_eState
         || EGridState.EGS_OpenBomb == m_pGrids[iX, iY].m_eState)
        {
            return;
        }

        byte g = m_pPuzzle.m_byGrids[iX, iY];
        if (0 != (g & (1 << 5)))
        {
            m_pGrids[iX, iY].SetGridState(EGridState.EGS_OpenBomb);
            for (int i = 0; i < 4; ++i)
            {
                if (0 != (g & (1 << i)))
                {
                    m_pGrids[iX, iY].ShowWall(i);
                }
            }
            m_bOver = true;
            m_pRes.text = "You Lose!";
            m_pRes.enabled = true;
            return;
        }

        m_pGrids[iX, iY].SetGridState(EGridState.EGS_OpenNoBomb);

        #region Checks

        #region Left Right

        //=================================
        //Left
        bool bMeetWall = false;
        int iShowWallLeftX = -1;
        int iShowWallLeftY = -1;
        int iLeftWallDir = -1;
        int iLeftNumber = 0;
        //Debug.Log(m_pPuzzle.m_byGrids[iX, iY]);
        for (int i = iX; i >= 0 && !bMeetWall; --i)
        {
            //Debug.Log(m_pPuzzle.m_byGrids[i, iY]);
            if (0 != (m_pPuzzle.m_byGrids[i, iY] & (1 << 5)))
            {
                ++iLeftNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[i, iY] & (1 << 3)))
            {
                bMeetWall = true;
                iShowWallLeftX = i;
                iShowWallLeftY = iY;
                iLeftWallDir = 3;
            }

            if (!bMeetWall && i - 1 >= 0)
            {
                if (0 != (m_pPuzzle.m_byGrids[i - 1, iY] & (1 << 1)))
                {
                    bMeetWall = true;
                    iShowWallLeftX = i - 1;
                    iShowWallLeftY = iY;
                    iLeftWallDir = 1;
                }
            }
        }

        //=================================
        //Right
        bMeetWall = false;
        int iShowWallRightX = -1;
        int iShowWallRightY = -1;
        int iRightWallDir = -1;
        int iRightNumber = 0;

        for (int i = iX; i < m_pGrids.GetLength(0) && !bMeetWall; ++i)
        {
            if (0 != (m_pPuzzle.m_byGrids[i, iY] & (1 << 5)))
            {
                ++iRightNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[i, iY] & (1 << 1)))
            {
                bMeetWall = true;
                iShowWallRightX = i;
                iShowWallRightY = iY;
                iRightWallDir = 1;
            }

            if (!bMeetWall && i + 1 < m_pGrids.GetLength(0))
            {
                if (0 != (m_pPuzzle.m_byGrids[i + 1, iY] & (1 << 3)))
                {
                    bMeetWall = true;
                    iShowWallRightX = i + 1;
                    iShowWallRightY = iY;
                    iRightWallDir = 3;
                }
            }
        }

        #endregion

        //=================================
        //Up
        bMeetWall = false;
        int iShowWallUpX = -1;
        int iShowWallUpY = -1;
        int iUpWallDir = -1;
        int iUpNumber = 0;

        for (int i = iY; i < m_pGrids.GetLength(1) && !bMeetWall; ++i)
        {
            if (0 != (m_pPuzzle.m_byGrids[iX, i] & (1 << 5)))
            {
                ++iUpNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX, i] & (1 << 0)))
            {
                bMeetWall = true;
                iShowWallUpX = iX;
                iShowWallUpY = i;
                iUpWallDir = 0;
            }

            if (!bMeetWall && i + 1 < m_pGrids.GetLength(1))
            {
                if (0 != (m_pPuzzle.m_byGrids[iX, i + 1] & (1 << 2)))
                {
                    bMeetWall = true;
                    iShowWallUpX = iX;
                    iShowWallUpY = i + 1;
                    iUpWallDir = 2;
                }
            }
        }

        //=================================
        //Down
        bMeetWall = false;
        int iShowWallDownX = -1;
        int iShowWallDownY = -1;
        int iDownWallDir = -1;
        int iDownNumber = 0;
        for (int i = iY; i >= 0 && !bMeetWall; --i)
        {
            if (0 != (m_pPuzzle.m_byGrids[iX, i] & (1 << 5)))
            {
                ++iDownNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX, i] & (1 << 2)))
            {
                bMeetWall = true;
                iShowWallDownX = iX;
                iShowWallDownY = i;
                iDownWallDir = 2;
            }

            if (!bMeetWall && i - 1 >= 0)
            {
                if (0 != (m_pPuzzle.m_byGrids[iX, i - 1] & (1 << 0)))
                {
                    bMeetWall = true;
                    iShowWallDownX = iX;
                    iShowWallDownY = i - 1;
                    iDownWallDir = 0;
                }
            }
        }

        #endregion

        #region LU

        bMeetWall = false;
        int iShowWallLUX1 = -1;
        int iShowWallLUY1 = -1;
        int iLUWallDir1 = -1;
        int iShowWallLUX2 = -1;
        int iShowWallLUY2 = -1;
        int iLUWallDir2 = -1;
        int iLUNumber = 0;
        //Debug.Log(m_pPuzzle.m_byGrids[iX, iY]);
        for (int i = 0; (iX - i) >= 0 && (iY + i) < m_pGrids.GetLength(1) && !bMeetWall; ++i)
        {
            //Debug.Log(m_pPuzzle.m_byGrids[i, iY]);
            if (0 != (m_pPuzzle.m_byGrids[iX - i, iY + i] & (1 << 5)))
            {
                ++iLUNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX - i, iY + i] & (1 << 3)))
            {
                bMeetWall = true;
                iShowWallLUX1 = iX - i;
                iShowWallLUY1 = iY + i;
                iLUWallDir1 = 3;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX - i, iY + i] & (1 << 0)))
            {
                bMeetWall = true;
                iShowWallLUX2 = iX - i;
                iShowWallLUY2 = iY + i;
                iLUWallDir2 = 0;
            }

            if (!bMeetWall && (iX - i - 1) >= 0 && (iY + i + 1) < m_pGrids.GetLength(1))
            {
                if (0 != (m_pPuzzle.m_byGrids[iX - i - 1, iY + i + 1] & (1 << 1)))
                {
                    bMeetWall = true;
                    iShowWallLUX1 = iX - i - 1;
                    iShowWallLUY1 = iY + i + 1;
                    iLUWallDir1 = 1;
                }

                if (0 != (m_pPuzzle.m_byGrids[iX - i - 1, iY + i + 1] & (1 << 2)))
                {
                    bMeetWall = true;
                    iShowWallLUX2 = iX - i - 1;
                    iShowWallLUY2 = iY + i + 1;
                    iLUWallDir2 = 2;
                }
            }
        }

        #endregion

        #region RU

        bMeetWall = false;
        int iShowWallRUX1 = -1;
        int iShowWallRUY1 = -1;
        int iRUWallDir1 = -1;
        int iShowWallRUX2 = -1;
        int iShowWallRUY2 = -1;
        int iRUWallDir2 = -1;
        int iRUNumber = 0;
        //Debug.Log(m_pPuzzle.m_byGrids[iX, iY]);
        for (int i = 0; (iX + i) < m_pGrids.GetLength(0) && (iY + i) < m_pGrids.GetLength(1) && !bMeetWall; ++i)
        {
            //Debug.Log(m_pPuzzle.m_byGrids[i, iY]);
            if (0 != (m_pPuzzle.m_byGrids[iX + i, iY + i] & (1 << 5)))
            {
                ++iRUNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX + i, iY + i] & (1 << 1)))
            {
                bMeetWall = true;
                iShowWallRUX1 = iX + i;
                iShowWallRUY1 = iY + i;
                iRUWallDir1 = 1;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX + i, iY + i] & (1 << 0)))
            {
                bMeetWall = true;
                iShowWallRUX2 = iX + i;
                iShowWallRUY2 = iY + i;
                iRUWallDir2 = 0;
            }

            if (!bMeetWall && (iX + i + 1) < m_pGrids.GetLength(0) && (iY + i + 1) < m_pGrids.GetLength(1))
            {
                if (0 != (m_pPuzzle.m_byGrids[iX + i + 1, iY + i + 1] & (1 << 3)))
                {
                    bMeetWall = true;
                    iShowWallRUX1 = iX + i + 1;
                    iShowWallRUY1 = iY + i + 1;
                    iRUWallDir1 = 3;
                }

                if (0 != (m_pPuzzle.m_byGrids[iX + i + 1, iY + i + 1] & (1 << 2)))
                {
                    bMeetWall = true;
                    iShowWallRUX2 = iX + i + 1;
                    iShowWallRUY2 = iY + i + 1;
                    iRUWallDir2 = 2;
                }
            }
        }

        #endregion

        #region LD

        bMeetWall = false;
        int iShowWallLDX1 = -1;
        int iShowWallLDY1 = -1;
        int iLDWallDir1 = -1;
        int iShowWallLDX2 = -1;
        int iShowWallLDY2 = -1;
        int iLDWallDir2 = -1;
        int iLDNumber = 0;
        //Debug.Log(m_pPuzzle.m_byGrids[iX, iY]);
        for (int i = 0; (iX - i) >= 0 && (iY - i) >= 0 && !bMeetWall; ++i)
        {
            //Debug.Log(m_pPuzzle.m_byGrids[i, iY]);
            if (0 != (m_pPuzzle.m_byGrids[iX - i, iY - i] & (1 << 5)))
            {
                ++iLDNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX - i, iY - i] & (1 << 3)))
            {
                bMeetWall = true;
                iShowWallLDX1 = iX - i;
                iShowWallLDY1 = iY - i;
                iLDWallDir1 = 3;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX - i, iY - i] & (1 << 2)))
            {
                bMeetWall = true;
                iShowWallLDX2 = iX - i;
                iShowWallLDY2 = iY - i;
                iLDWallDir2 = 2;
            }

            if (!bMeetWall && (iX - i - 1) >= 0 && (iY - i - 1) >= 0)
            {
                if (0 != (m_pPuzzle.m_byGrids[iX - i - 1, iY - i - 1] & (1 << 1)))
                {
                    bMeetWall = true;
                    iShowWallLDX1 = iX - i - 1;
                    iShowWallLDY1 = iY - i - 1;
                    iLDWallDir1 = 1;
                }

                if (0 != (m_pPuzzle.m_byGrids[iX - i - 1, iY - i - 1] & (1 << 0)))
                {
                    bMeetWall = true;
                    iShowWallLDX2 = iX - i - 1;
                    iShowWallLDY2 = iY - i - 1;
                    iLDWallDir2 = 0;
                }
            }
        }

        #endregion

        #region RD

        bMeetWall = false;
        int iShowWallRDX1 = -1;
        int iShowWallRDY1 = -1;
        int iRDWallDir1 = -1;
        int iShowWallRDX2 = -1;
        int iShowWallRDY2 = -1;
        int iRDWallDir2 = -1;
        int iRDNumber = 0;
        //Debug.Log(m_pPuzzle.m_byGrids[iX, iY]);
        for (int i = 0; (iX + i) < m_pGrids.GetLength(0) && (iY - i) >= 0 && !bMeetWall; ++i)
        {
            //Debug.Log(m_pPuzzle.m_byGrids[i, iY]);
            if (0 != (m_pPuzzle.m_byGrids[iX + i, iY - i] & (1 << 5)))
            {
                ++iRDNumber;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX + i, iY - i] & (1 << 1)))
            {
                bMeetWall = true;
                iShowWallRDX1 = iX + i;
                iShowWallRDY1 = iY - i;
                iRDWallDir1 = 1;
            }
            if (0 != (m_pPuzzle.m_byGrids[iX + i, iY - i] & (1 << 2)))
            {
                bMeetWall = true;
                iShowWallRDX2 = iX + i;
                iShowWallRDY2 = iY - i;
                iRDWallDir2 = 2;
            }

            if (!bMeetWall && (iX + i + 1) < m_pGrids.GetLength(0) && (iY - i - 1) >= 0)
            {
                if (0 != (m_pPuzzle.m_byGrids[iX + i + 1, iY - i - 1] & (1 << 3)))
                {
                    bMeetWall = true;
                    iShowWallRDX1 = iX + i + 1;
                    iShowWallRDY1 = iY - i - 1;
                    iRDWallDir1 = 3;
                }

                if (0 != (m_pPuzzle.m_byGrids[iX + i + 1, iY - i - 1] & (1 << 0)))
                {
                    bMeetWall = true;
                    iShowWallRDX2 = iX + i + 1;
                    iShowWallRDY2 = iY - i - 1;
                    iRDWallDir2 = 2;
                }
            }
        }

        #endregion

        string[] numbers = new string[]
        {
            0 == iUpNumber ? "" : iUpNumber.ToString(),
            0 == iRightNumber ? "" : iRightNumber.ToString(),
            0 == iDownNumber ? "" : iDownNumber.ToString(),
            0 == iLeftNumber ? "" : iLeftNumber.ToString(),
            0 == iLUNumber ? "" : iLUNumber.ToString(),
            0 == iRUNumber ? "" : iRUNumber.ToString(),
            0 == iLDNumber ? "" : iLDNumber.ToString(),
            0 == iRDNumber ? "" : iRDNumber.ToString(),
        };
        m_pGrids[iX, iY].SetNumber(numbers);

        #region Walls

        if (iShowWallLeftX >= 0)
        {
            m_pGrids[iShowWallLeftX, iShowWallLeftY].ShowWall(iLeftWallDir);
        }

        if (iShowWallRightX >= 0)
        {
            m_pGrids[iShowWallRightX, iShowWallRightY].ShowWall(iRightWallDir);
        }

        if (iShowWallUpX >= 0)
        {
            m_pGrids[iShowWallUpX, iShowWallUpY].ShowWall(iUpWallDir);
        }

        if (iShowWallDownX >= 0)
        {
            m_pGrids[iShowWallDownX, iShowWallDownY].ShowWall(iDownWallDir);
        }

        if (iShowWallLUX1 >= 0)
        {
            m_pGrids[iShowWallLUX1, iShowWallLUY1].ShowWall(iLUWallDir1);
        }
        if (iShowWallLUX2 >= 0)
        {
            m_pGrids[iShowWallLUX2, iShowWallLUY2].ShowWall(iLUWallDir2);
        }
        if (iShowWallRUX1 >= 0)
        {
            m_pGrids[iShowWallRUX1, iShowWallRUY1].ShowWall(iRUWallDir1);
        }
        if (iShowWallRUX2 >= 0)
        {
            m_pGrids[iShowWallRUX2, iShowWallRUY2].ShowWall(iRUWallDir2);
        }

        if (iShowWallLDX1 >= 0)
        {
            m_pGrids[iShowWallLDX1, iShowWallLDY1].ShowWall(iLDWallDir1);
        }
        if (iShowWallLDX2 >= 0)
        {
            m_pGrids[iShowWallLDX2, iShowWallLDY2].ShowWall(iLDWallDir2);
        }
        if (iShowWallRDX1 >= 0)
        {
            m_pGrids[iShowWallRDX1, iShowWallRDY1].ShowWall(iRDWallDir1);
        }
        if (iShowWallRDX2 >= 0)
        {
            m_pGrids[iShowWallRDX2, iShowWallRDY2].ShowWall(iRDWallDir2);
        }

        #endregion

        int iAll = 0;
        for (int i = 0; i < m_pPuzzle.m_byGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_byGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_OpenNoBomb == m_pGrids[i, j].m_eState)
                {
                    ++iAll;
                }
            }
        }

        if (iAll >= m_pPuzzle.m_byGrids.GetLength(0) * m_pPuzzle.m_byGrids.GetLength(1) - MineNumber)
        {
            m_bOver = true;
            m_pRes.text = "You Win!";
            m_pRes.enabled = true;
            return;
        }

        if (0 == iUpNumber 
         && iY + 1 < m_pGrids.GetLength(1)
         && m_pGrids[iX, iY + 1].m_eState != EGridState.EGS_OpenNoBomb
         && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 0)))
         && (0 == (m_pPuzzle.m_byGrids[iX, iY + 1] & (1 << 2)))
         ) 
        {
            OnPressMine(iX, iY + 1);
        }

        if (0 == iDownNumber
         && iY - 1 >= 0
         && m_pGrids[iX, iY - 1].m_eState != EGridState.EGS_OpenNoBomb
         && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 2)))
         && (0 == (m_pPuzzle.m_byGrids[iX, iY - 1] & (1 << 0)))
            )
        {
            OnPressMine(iX, iY - 1);
        }

        if (0 == iLeftNumber
         && iX - 1 >= 0
         && m_pGrids[iX - 1, iY].m_eState != EGridState.EGS_OpenNoBomb
         && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 3)))
         && (0 == (m_pPuzzle.m_byGrids[iX - 1, iY] & (1 << 1)))
         )
        {
            OnPressMine(iX - 1, iY);
        }

        if (0 == iRightNumber
         && iX + 1 < m_pGrids.GetLength(0)
         && m_pGrids[iX + 1, iY].m_eState != EGridState.EGS_OpenNoBomb
         && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 1)))
         && (0 == (m_pPuzzle.m_byGrids[iX + 1, iY] & (1 << 3)))
         )
        {
            OnPressMine(iX + 1, iY);
        }

        if (0 == iLUNumber
             && iY + 1 < m_pGrids.GetLength(1)
             && iX - 1 >= 0
             && m_pGrids[iX - 1, iY + 1].m_eState != EGridState.EGS_OpenNoBomb
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 0)))
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 3)))
             && (0 == (m_pPuzzle.m_byGrids[iX - 1, iY + 1] & (1 << 2)))
             && (0 == (m_pPuzzle.m_byGrids[iX - 1, iY + 1] & (1 << 1)))
             )
        {
            OnPressMine(iX - 1, iY + 1);
        }

        if (0 == iRUNumber
             && iY + 1 < m_pGrids.GetLength(1)
             && iX + 1 < m_pGrids.GetLength(0)
             && m_pGrids[iX + 1, iY + 1].m_eState != EGridState.EGS_OpenNoBomb
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 0)))
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 1)))
             && (0 == (m_pPuzzle.m_byGrids[iX + 1, iY + 1] & (1 << 2)))
             && (0 == (m_pPuzzle.m_byGrids[iX + 1, iY + 1] & (1 << 3)))
             )
        {
            OnPressMine(iX + 1, iY + 1);
        }

        if (0 == iLDNumber
             && iY - 1 >= 0
             && iX - 1 >= 0
             && m_pGrids[iX - 1, iY - 1].m_eState != EGridState.EGS_OpenNoBomb
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 2)))
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 3)))
             && (0 == (m_pPuzzle.m_byGrids[iX - 1, iY - 1] & (1 << 0)))
             && (0 == (m_pPuzzle.m_byGrids[iX - 1, iY - 1] & (1 << 1)))
             )
        {
            OnPressMine(iX - 1, iY - 1);
        }

        if (0 == iRDNumber
             && iY - 1 >= 0
             && iX + 1 < m_pGrids.GetLength(0)
             && m_pGrids[iX + 1, iY - 1].m_eState != EGridState.EGS_OpenNoBomb
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 2)))
             && (0 == (m_pPuzzle.m_byGrids[iX, iY] & (1 << 1)))
             && (0 == (m_pPuzzle.m_byGrids[iX + 1, iY - 1] & (1 << 0)))
             && (0 == (m_pPuzzle.m_byGrids[iX + 1, iY - 1] & (1 << 3)))
             )
        {
            OnPressMine(iX + 1, iY - 1);
        }
    }

    #endregion
}

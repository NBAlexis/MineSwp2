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

public enum EPressType
{
    EPT_Dig,
    EPT_Tag,
}

public class AUI : MonoBehaviour
{
    public static readonly string[] bombNum =
    {
        "",
        "<color=#0000FF>1</color>",
        "<color=#0088FF>2</color>",
        "<color=#00FFFF>3</color>",
        "<color=#00FF88>4</color>",
        "<color=#00FF00>5</color>",
        "<color=#88FF00>6</color>",
        "<color=#FFFF00>7</color>",
        "<color=#FF8800>8</color>",
        "<color=#FF0000>9</color>",
        "<color=#FF0000>10</color>",
        "<color=#FF0000>11</color>",
    };
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
        CPuzzle newPuzzle = PuzzleCreator.CreatePuzzle(TestWidth, TestHeight, WallNumber, EmptyNumber);
        Debug.Log(newPuzzle.m_pLines.Length);
        newPuzzle.PutBombs(MineNumber);
        PutPuzzle(newPuzzle);

        m_pPages[(int)EPage.EP_Start].SetActive(false);
        m_pPages[(int)EPage.EP_Game].SetActive(true);

        m_pMineNumber.text = MineNumber.ToString();
        m_bOver = false;
        m_pRes.enabled = false;
    }

    #endregion

    #region Game

    public const int MineNumber = 45;
    public const int WallNumber = 50;
    public const int EmptyNumber = 20;
    public const int TestWidth = 12;
    public const int TestHeight = 12;
    public AGridToggle m_pOneGrid;
    public GameObject m_pOneLine;
    public AGridToggle[,] m_pGrids;
    public GameObject[] m_pLines;
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
        if (null != m_pLines)
        {
            for (int i = 0; i < m_pLines.Length; ++i)
            {
                if (null != m_pLines[i])
                {
                    m_pLines[i].name = "Discard";
                    Destroy(m_pLines[i]);
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
                if (puzzle.IsEmpty(i, j))
                {
                    newG.SetGridState(EGridState.EGS_Empty);
                }
                else
                {
                    newG.SetGridState(EGridState.EGS_Close);    
                }

                m_pGrids[i, j] = newG;
            }            
        }

        m_pLines = new GameObject[puzzle.m_pLines.Length];
        for (int i = 0; i < puzzle.m_pLines.Length; ++i)
        {
            float fX1 = m_pGrids[puzzle.m_pLines[i].m_iStartX, puzzle.m_pLines[i].m_iStartY].transform.localPosition.x;
            float fY1 = m_pGrids[puzzle.m_pLines[i].m_iStartX, puzzle.m_pLines[i].m_iStartY].transform.localPosition.y;

            float fX2 = m_pGrids[puzzle.m_pLines[i].m_iEndX, puzzle.m_pLines[i].m_iEndY].transform.localPosition.x;
            float fY2 = m_pGrids[puzzle.m_pLines[i].m_iEndX, puzzle.m_pLines[i].m_iEndY].transform.localPosition.y;

            GameObject newLine = Instantiate(m_pOneLine.gameObject);
            newLine.transform.SetParent(m_pOneLine.transform.parent);

            newLine.transform.localScale = new Vector3(Mathf.Sqrt((fX1 - fX2) * (fX1 - fX2) + (fY1 - fY2) * (fY1 - fY2)) / 100.0f, 1.0f, 1.0f);
            newLine.transform.localPosition = new Vector3((fX1 + fX2) * 0.5f, (fY1 + fY2) * 0.5f, 0.0f);
            newLine.transform.eulerAngles =  new Vector3(0.0f, 0.0f, Mathf.Atan2(fY2 - fY1, fX2 - fX1) * 180.0f / Mathf.PI);

            m_pLines[i] = newLine;
        }
    }

    private void OnTagMine(int iX, int iY)
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
        /*
        for (int i = 0; i < 4; ++i)
        {
            if (bWall[i])
            {
                m_pGrids[iX, iY].ShowWall(i);
            }
        }
        */

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

    private void OnDig(int iX, int iY)
    {
        if (m_pPuzzle.HasBomb(iX, iY))
        {
            ShowAllBombs();
            m_bOver = true;
            m_pRes.text = "You Lose!";
            m_pRes.enabled = true;
            return;
        }

        m_pGrids[iX, iY].SetGridState(EGridState.EGS_OpenNoBomb);

        int[] iShowWallX = new int[(int)EDir.Max];
        int[] iShowWallY = new int[(int)EDir.Max];
        EDir[] iShowWallDir = new EDir[(int)EDir.Max];
        int[] iNumber = new int[(int)EDir.Max];
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            bool bMeetWall = false;
            iShowWallX[i] = -1;
            iShowWallY[i] = -1;
            iShowWallDir[i] = EDir.Max;
            iNumber[i] = 0;

            for (int j = 0; !bMeetWall; ++j)
            {
                int iCheckPosX = iX + PuzzleCreator.deltaX[i] * j;
                int iCheckPosY = iY + PuzzleCreator.deltaY[i] * j;

                if (m_pPuzzle.HasBomb(iCheckPosX, iCheckPosY))
                {
                    ++iNumber[i];
                }
                if (m_pPuzzle.HasWall((EDir)i, iCheckPosX, iCheckPosY))
                {
                    bMeetWall = true;
                    iShowWallX[i] = iCheckPosX;
                    iShowWallY[i] = iCheckPosY;
                    iShowWallDir[i] = (EDir)i;
                }

                if (!bMeetWall)
                {
                    iCheckPosX = iX + PuzzleCreator.deltaX[i] * (j + 1);
                    iCheckPosY = iY + PuzzleCreator.deltaY[i] * (j + 1);
                    if (m_pPuzzle.HasWall(PuzzleCreator.GetOp((EDir)i), iCheckPosX, iCheckPosY))
                    {
                        bMeetWall = true;
                        iShowWallX[i] = iCheckPosX;
                        iShowWallY[i] = iCheckPosY;
                        iShowWallDir[i] = PuzzleCreator.GetOp((EDir)i);
                    }
                }
            }
        }

        //Show Number and Walls
        string[] numbers = new string[(int)EDir.Max];
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            numbers[i] = bombNum[iNumber[i]];
            /*
            if (m_pPuzzle.HasGrid(iShowWallX[i], iShowWallY[i]))
            {
                m_pGrids[iShowWallX[i], iShowWallY[i]].ShowWall((int)iShowWallDir[i]);
            }
            */
        }

        m_pGrids[iX, iY].SetNumber(numbers);

        //Open other grids
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            if (0 == iNumber[i])
            {
                int iNextP1X = iX + PuzzleCreator.deltaX[i];
                int iNextP1Y = iY + PuzzleCreator.deltaY[i];
                if (m_pPuzzle.HasGrid(iNextP1X, iNextP1Y)
                 && !m_pPuzzle.HasWall((EDir)i, iX, iY)
                 && !m_pPuzzle.HasWall(PuzzleCreator.GetOp((EDir)i), iNextP1X, iNextP1Y)
                 && m_pGrids[iNextP1X, iNextP1Y].m_eState != EGridState.EGS_OpenNoBomb)
                {
                    OnDig(iNextP1X, iNextP1Y);
                }
            }
        }

        for (int i = 0; i < m_pPuzzle.m_byGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_byGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_OpenNoBomb == m_pGrids[i, j].m_eState)
                {
                    for (int k = 0; k < (int) EDir.Max; ++k)
                    {
                        int iNewX = i + PuzzleCreator.deltaX[k];
                        int iNewY = j + PuzzleCreator.deltaY[k];
                        if (m_pPuzzle.HasGrid(iNewX, iNewY) && EGridState.EGS_OpenNoBomb == m_pGrids[iNewX, iNewY].m_eState)
                        {
                            m_pGrids[i, j].m_pNumbers[k].text = "";
                        }
                    }
                }
            }
        }        

        //Check Win
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
        }        
    }

    private void ShowAllBombs()
    {
        for (int i = 0; i < m_pGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pGrids.GetLength(1); ++j)
            {
                if (m_pPuzzle.HasBomb(i, j))
                {
                    m_pGrids[i, j].SetGridState(EGridState.EGS_OpenBomb);
                    /*
                    for (int k = 0; k < (int)EDir.Max; ++k)
                    {
                        if (m_pPuzzle.HasWall((EDir)k, i, j))
                        {
                            m_pGrids[i, j].ShowWall(k);
                        }
                    }   
                    */
                }
            }
        }
    }

    public void OnPressMine(int iX, int iY, EPressType eType)
    {
        if (m_bOver)
        {
            return;
        }

        switch (eType)
        {
            case EPressType.EPT_Tag:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].m_eState
                 || EGridState.EGS_Tag == m_pGrids[iX, iY].m_eState)
                {
                    OnTagMine(iX, iY);
                }
                break;
            case EPressType.EPT_Dig:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].m_eState)
                {
                    OnDig(iX, iY);
                }
                break;
        }
    }

    #endregion
}

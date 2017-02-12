using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum EPage
{
    EP_Start,
    EP_Game,
    EP_Choose,
}

public enum EPressType
{
    EPT_Dig,
    EPT_Tag,
    EPT_GoldenDig,
    EPT_Mouse,

    EPT_Max,
}

public class AUI : MonoBehaviour
{
    public static AUI UI;
    //max is 8
    public static readonly string[] bombNum =
    {
        "",
        "<color=#0000FF>1</color>",
        "<color=#00FFFF>2</color>",
        "<color=#00FF00>3</color>",
        "<color=#FFFF00>4</color>",
        "<color=#FF0000>5</color>",
        "<color=#FF0000>6</color>",
        "<color=#FF0000>7</color>",
        "<color=#FF0000>8</color>",
    };

    public GameObject[] m_pPages;

	// Use this for initialization
	void Start () 
    {
        PlayerSave.Initial();
        UI = this;
        m_pPages[(int)EPage.EP_Start].SetActive(true);
        m_pPages[(int)EPage.EP_Game].SetActive(false);
        m_pPages[(int)EPage.EP_Choose].SetActive(false);
        HideDialog();
    }
	
	// Update is called once per frame
	void Update () {

    }

    #region Start

    public void OnQuitButton()
    {
        ShowDialog("Do you want to quit?", OnRealQuit);
    }

    private void OnRealQuit()
    {
        Application.Quit();
    }

    public void OnChooseLevelButton()
    {
        m_pPages[(int)EPage.EP_Start].SetActive(false);
        m_pPages[(int)EPage.EP_Choose].SetActive(true);
        m_pPages[(int)EPage.EP_Game].SetActive(false);
        OnEnterChooseLevel();
    }

    public void OnStartGameButton()
    {
        GoLevel(65);
    }

    #endregion

    #region Choose

    public int m_iCurrentLevel = 0;
    public void GoLevel(int iLevel)
    {
        m_iCurrentLevel = iLevel;
        if (0 == iLevel)
        {
            
        }
        else if (iLevel > 0 && iLevel <= 64)
        {
            byte[] data = Resources.Load<TextAsset>("puzzle_" + iLevel).bytes;
            CPuzzle pz = new CPuzzle();
            pz.FromByteArray(data);
            GoPuzzle(pz);
        }
        else if (iLevel > 64)
        {
            StartRandomLevel();
        }
    }

    private void StartRandomLevel()
    {
        int iRandom = Random.Range(0, 5);
        int iMineNumber = CConst.MH_MineNumber;
        int iGhostMineNumber = CConst.MH_GhostMineNumber;
        int iWallNumber = CConst.MH_WallNumber;
        int iEmptyNumber = CConst.MH_EmptyNumber;
        int iFrozenNumber = CConst.MH_FrozenNumber;
        int iTestWidth = CConst.MH_TestWidth;
        int iTestHeight = CConst.MH_TestHeight;
        int iGold = CConst.MH_Gold;
        int iMouse = CConst.MH_Mouse;
        switch (iRandom)
        {
            case 1:
                        iMineNumber = CConst.H_MineNumber;
                        iGhostMineNumber = CConst.H_GhostMineNumber;
                        iWallNumber = CConst.H_WallNumber;
                        iEmptyNumber = CConst.H_EmptyNumber;
                        iFrozenNumber = CConst.H_FrozenNumber;
                        iTestWidth = CConst.H_Width;
                        iTestHeight = CConst.H_Height;
                        iGold = CConst.H_Gold;
                        iMouse = CConst.H_Mouse;
                break;
            case 2:
                        iMineNumber = CConst.VH_MineNumber;
                        iGhostMineNumber = CConst.VH_GhostMineNumber;
                        iWallNumber = CConst.VH_WallNumber;
                        iEmptyNumber = CConst.VH_EmptyNumber;
                        iFrozenNumber = CConst.VH_FrozenNumber;
                        iTestWidth = CConst.VH_Width;
                        iTestHeight = CConst.VH_Height;
                        iGold = CConst.VH_Gold;
                        iMouse = CConst.VH_Mouse;
                break;
            case 3:
                        iMineNumber = CConst.VVH_MineNumber;
                        iGhostMineNumber = CConst.VVH_GhostMineNumber;
                        iWallNumber = CConst.VVH_WallNumber;
                        iEmptyNumber = CConst.VVH_EmptyNumber;
                        iFrozenNumber = CConst.VVH_FrozenNumber;
                        iTestWidth = CConst.VVH_Width;
                        iTestHeight = CConst.VVH_Height;
                        iGold = CConst.VVH_Gold;
                        iMouse = CConst.VVH_Mouse;
                break;
            case 4:
                        iMineNumber = CConst.VVVH_MineNumber;
                        iGhostMineNumber = CConst.VVVH_GhostMineNumber;
                        iWallNumber = CConst.VVVH_WallNumber;
                        iEmptyNumber = CConst.VVVH_EmptyNumber;
                        iFrozenNumber = CConst.VVVH_FrozenNumber;
                        iTestWidth = CConst.VVVH_Width;
                        iTestHeight = CConst.VVVH_Height;
                        iGold = CConst.VVVH_Gold;
                        iMouse = CConst.VVVH_Mouse;
                break;
        }

        CPuzzle newPuzzle = PuzzleCreator.CreatePuzzle(iTestWidth, iTestHeight, iWallNumber, iEmptyNumber, iFrozenNumber);
        while (!newPuzzle.CheckConnection())
        {
            newPuzzle = PuzzleCreator.CreatePuzzle(iTestWidth, iTestHeight, iWallNumber, iEmptyNumber, iFrozenNumber);
        }
        newPuzzle.m_iGoldNumber = iGold;
        newPuzzle.m_iMouseNumber = iMouse;
        newPuzzle.m_iBombNumber = iMineNumber;
        newPuzzle.m_iGhostBombNumber = iGhostMineNumber;
        GoPuzzle(newPuzzle);
    }

    private void GoPuzzle(CPuzzle puzzle)
    {
        puzzle.PutBombs(puzzle.m_iBombNumber);
        puzzle.PutGhostBomb(puzzle.m_iGhostBombNumber);
        PutPuzzle(puzzle);
        m_pPages[(int)EPage.EP_Start].SetActive(false);
        m_pPages[(int)EPage.EP_Choose].SetActive(false);
        m_pPages[(int)EPage.EP_Game].SetActive(true);
        m_bOver = false;
        HideResoult();
    }

    public GameObject m_pLevelBtPrefab;
    public GameObject[] m_pLevelBts;

    public void OnEnterChooseLevel()
    {
        if (null != m_pLevelBts)
        {
            for (int i = 0; i < m_pLevelBts.Length; ++i)
            {
                if (null != m_pLevelBts[i])
                {
                    m_pLevelBts[i].name = "discard";
                    Destroy(m_pLevelBts[i]);
                }
            }
        }


    }

    public void OnPressChooseLevelButton(GameObject bt)
    {
        
    }

    #endregion

    #region Game

    public const int MineNumber = 6;
    public const int GhostMineNumber = 0;
    public const int WallNumber = 5;
    public const int EmptyNumber = 2;
    public const int FrozenNumber = 0;
    public const int TestWidth = 4;
    public const int TestHeight = 4;
    public const int GoldNumber = 2;
    public const int MouseNumber = 0;

    public AGridToggle m_pOneGrid;
    public GameObject m_pOneLine;
    public AGridToggle[,] m_pGrids;
    public GameObject[] m_pLines;
    public CPuzzle m_pPuzzle;

    public Text m_pMineNumber;
    public Text m_pSpecialMineNumber;
    public bool m_bOver = false;

    public int m_iGoldenDigCount = GoldNumber;
    public int m_iMouseCount = MouseNumber;

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

        m_pGrids = new AGridToggle[puzzle.m_ushGrids.GetLength(0), puzzle.m_ushGrids.GetLength(1)];

        for (int i = 0; i < puzzle.m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < puzzle.m_ushGrids.GetLength(1); ++j)
            {
                GameObject newGrid = Instantiate(m_pOneGrid.gameObject);
                newGrid.transform.SetParent(m_pOneGrid.transform.parent);

                AGridToggle newG = newGrid.GetComponent<AGridToggle>();
                newG.SetOwner(this);
                newG.m_iX = i;
                newG.m_iY = j;
                newG.PutToGridWithWH(puzzle.m_ushGrids.GetLength(0), puzzle.m_ushGrids.GetLength(1));
                newG.m_pLines = new List<CLines>();
                newG.m_pStartLines = new List<CLines>();
                if (puzzle.IsEmpty(i, j))
                {
                    newG.SetGridState(EGridState.EGS_Empty, EPressType.EPT_Max);
                }
                //else
                //{
                //    newG.SetGridState(EGridState.EGS_Close, EPressType.EPT_Max);    
                //}

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
            m_pGrids[puzzle.m_pLines[i].m_iStartX, puzzle.m_pLines[i].m_iStartY].m_pStartLines.Add(puzzle.m_pLines[i]);
            m_pGrids[puzzle.m_pLines[i].m_iEndX, puzzle.m_pLines[i].m_iEndY].m_pStartLines.Add(puzzle.m_pLines[i]); 

            for (int j = 0; j < puzzle.m_pLines[i].m_iNodeXs.Count; ++j)
            {
                m_pGrids[puzzle.m_pLines[i].m_iNodeXs[j], puzzle.m_pLines[i].m_iNodeYs[j]].m_pLines.Add(puzzle.m_pLines[i]);      
            }
        }

        for (int i = 0; i < puzzle.m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < puzzle.m_ushGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_Empty != m_pGrids[i, j].GetState())
                {
                    bool bFrozen = false;
                    for (int k = 0; k < m_pGrids[i, j].m_pLines.Count; ++k)
                    {
                        if (m_pGrids[i, j].m_pLines[k].m_eType == ELineType.ELT_Frozen)
                        {
                            bFrozen = true;
                            break;
                        }
                    }

                    if (bFrozen)
                    {
                        m_pGrids[i, j].SetGridState(EGridState.EGS_CloseFrozen, EPressType.EPT_Max);
                    }
                    else if (1 == m_pGrids[i, j].m_pStartLines.Count && 1 == m_pGrids[i, j].m_pLines.Count)
                    {
                        m_pGrids[i, j].SetGridState(EGridState.EGS_CloseSafe, EPressType.EPT_Max);
                    }
                    else
                    {
                        m_pGrids[i, j].SetGridState(EGridState.EGS_Close, EPressType.EPT_Max);    
                    }
                }
            }
        }

        m_iGoldenDigCount = puzzle.m_iGoldNumber;
        m_iMouseCount = puzzle.m_iMouseNumber;
        m_ePressNow = EPressType.EPT_Dig;
        FinishDig();
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

        if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
         || EGridState.EGS_CloseFrozen == m_pGrids[iX, iY].GetState())
        {
            m_pGrids[iX, iY].SetGridState(EGridState.EGS_Tag, EPressType.EPT_Max);
        }
        else if (EGridState.EGS_Tag == m_pGrids[iX, iY].GetState())
        {
            bool bFrozen = false;
            for (int i = 0; i < m_pGrids[iX, iY].m_pLines.Count; ++i)
            {
                if (m_pGrids[iX, iY].m_pLines[i].m_eType == ELineType.ELT_Frozen)
                {
                    bFrozen = true;
                    break;
                }
            }
            m_pGrids[iX, iY].SetGridState(bFrozen ? EGridState.EGS_CloseFrozen : EGridState.EGS_Close, EPressType.EPT_Max);
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
    }

    private void OnDig(int iX, int iY, bool bLink)
    {
        if (m_pPuzzle.HasBomb(iX, iY))
        {
            m_pGrids[iX, iY].PlayEffect(EGridEffect.EGE_Explode);
            ShowAllBombs();
            m_bOver = true;
            ShowResoult(false);
            return;
        }

        bool bHasGhost = false;
        for (int i = 0; i < m_pGrids[iX, iY].m_pLines.Count; ++i)
        {
            for (int j = 0; j < m_pGrids[iX, iY].m_pLines[i].m_iNodeXs.Count; ++j)
            {
                if (m_pPuzzle.HasGhostBomb(
                    m_pGrids[iX, iY].m_pLines[i].m_iNodeXs[j],
                    m_pGrids[iX, iY].m_pLines[i].m_iNodeYs[j]))
                {
                    bHasGhost = true;
                    break;
                }
            }
            if (bHasGhost)
            {
                break;
            }
        }
        ++m_iOpenIndex;
        m_pGrids[iX, iY].SetGridState(
            bHasGhost ? EGridState.EGS_OpenGhost : EGridState.EGS_OpenNoBomb,
            bLink ? EPressType.EPT_Max : m_ePressNow, 
            m_iOpenIndex * 0.05f);

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
                 && m_pGrids[iNextP1X, iNextP1Y].GetState() != EGridState.EGS_OpenNoBomb
                 && m_pGrids[iNextP1X, iNextP1Y].GetState() != EGridState.EGS_OpenGhost)
                {
                    OnDig(iNextP1X, iNextP1Y, true);
                }
            }
        }
    }

    private void OnMouseDig(int iX, int iY)
    {
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            bool bMeetWall = false;

            for (int j = 0; !bMeetWall; ++j)
            {
                int iCheckPosX = iX + PuzzleCreator.deltaX[i] * j;
                int iCheckPosY = iY + PuzzleCreator.deltaY[i] * j;

                if (m_pPuzzle.HasBomb(iCheckPosX, iCheckPosY))
                {
                    m_pPuzzle.RemoveBomb(iCheckPosX, iCheckPosY);
                    RefreshNodesWithRemovedBomb(m_pGrids[iCheckPosX, iCheckPosY]);
                }
                if (m_pGrids[iCheckPosX, iCheckPosY].GetState() != EGridState.EGS_OpenNoBomb
                 && m_pGrids[iCheckPosX, iCheckPosY].GetState() != EGridState.EGS_OpenGhost)
                {
                    OnDig(iCheckPosX, iCheckPosY, false);
                }
                if (m_pPuzzle.HasWall((EDir)i, iCheckPosX, iCheckPosY))
                {
                    bMeetWall = true;
                }

                if (!bMeetWall)
                {
                    iCheckPosX = iX + PuzzleCreator.deltaX[i] * (j + 1);
                    iCheckPosY = iY + PuzzleCreator.deltaY[i] * (j + 1);
                    if (m_pPuzzle.HasWall(PuzzleCreator.GetOp((EDir)i), iCheckPosX, iCheckPosY))
                    {
                        bMeetWall = true;
                    }
                }
            }
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
                    m_pGrids[i, j].SetGridState(EGridState.EGS_OpenBomb, EPressType.EPT_Max);
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

    private int m_iOpenIndex = 0;
    public void OnPressMine(int iX, int iY, EPressType eType)
    {
        if (m_bOver)
        {
            return;
        }

        m_iOpenIndex = 0;
        switch (eType)
        {
            case EPressType.EPT_Tag:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_CloseFrozen == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_Tag == m_pGrids[iX, iY].GetState())
                {
                    OnTagMine(iX, iY);
                    FinishDig();
                }
                break;
            case EPressType.EPT_Dig:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_CloseSafe == m_pGrids[iX, iY].GetState())
                {
                    OnDig(iX, iY, false);
                    FinishDig();
                }
                break;
            case EPressType.EPT_GoldenDig:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_CloseSafe == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_CloseFrozen == m_pGrids[iX, iY].GetState())
                {
                    if (m_iGoldenDigCount > 0)
                    {
                        if (m_pPuzzle.HasBomb(iX, iY))
                        {
                            m_pPuzzle.RemoveBomb(iX, iY);
                            RefreshNodesWithRemovedBomb(m_pGrids[iX, iY]);
                        }
                        OnDig(iX, iY, false);

                        --m_iGoldenDigCount;
                        FinishDig();
                    }
                }
                break;
            case EPressType.EPT_Mouse:
                if (m_iMouseCount > 0)
                {
                    if (EGridState.EGS_OpenNoBomb == m_pGrids[iX, iY].GetState()
                     || EGridState.EGS_OpenGhost == m_pGrids[iX, iY].GetState())
                    {
                        --m_iMouseCount;
                        OnMouseDig(iX, iY);
                        FinishDig();
                    }
                }
                break;
        }
    }

    public void OnGameRefreshButton()
    {
        ShowDialog("Do you want to restart?", OnRealGameRefreshButton);
    }

    private void OnRealGameRefreshButton()
    {
        GoLevel(m_iCurrentLevel);
    }

    public void OnGameQuitButton()
    {
        ShowDialog("Do you want to quit?", OnRealGameQuitButton);
    }

    private void OnRealGameQuitButton()
    {
        if (m_iCurrentLevel >= 1 && m_iCurrentLevel <= 64)
        {
            m_pPages[(int)EPage.EP_Start].SetActive(false);
            m_pPages[(int)EPage.EP_Choose].SetActive(true);
            m_pPages[(int)EPage.EP_Game].SetActive(false);
            OnEnterChooseLevel();
        }
        else
        {
            m_pPages[(int)EPage.EP_Start].SetActive(true);
            m_pPages[(int)EPage.EP_Choose].SetActive(false);
            m_pPages[(int)EPage.EP_Game].SetActive(false);
        }
    }

    public Image[] m_pButtons;
    public Text m_pGoldenDig;
    public Text m_pMouseDig;
    public EPressType m_ePressNow = EPressType.EPT_Dig;

    public static readonly Color m_pColorToggled = new Color(216.0f / 255.0f, 252.0f / 255.0f, 255.0f / 255.0f);
    public static readonly Color m_pColorUnToggled = new Color(255.0f / 255.0f, 245.0f / 255.0f, 188.0f / 255.0f);
    public static readonly Color m_pColorUseOut = new Color(133.0f / 255.0f, 133.0f / 255.0f, 133.0f / 255.0f);

    public GameObject[] m_pItemInfos;

    private void SetButtonColors()
    {
        if (EPressType.EPT_GoldenDig == m_ePressNow && m_iGoldenDigCount <= 0)
        {
            m_ePressNow = EPressType.EPT_Dig;
        }
        if (EPressType.EPT_Mouse == m_ePressNow && m_iMouseCount <= 0)
        {
            m_ePressNow = EPressType.EPT_Dig;
        }

        for (int i = 0; i < (int)EPressType.EPT_Max; ++i)
        {
            if (i == (int) m_ePressNow)
            {
                m_pButtons[i].color = m_pColorToggled;
                foreach (Image img in m_pItemInfos[i].GetComponentsInChildren<Image>())
                {
                    img.enabled = true;
                }
                foreach (Text img in m_pItemInfos[i].GetComponentsInChildren<Text>())
                {
                    img.enabled = true;
                }
            }
            else
            {
                m_pButtons[i].color = m_pColorUnToggled;
                foreach (Image img in m_pItemInfos[i].GetComponentsInChildren<Image>())
                {
                    img.enabled = false;
                }
                foreach (Text img in m_pItemInfos[i].GetComponentsInChildren<Text>())
                {
                    img.enabled = false;
                }
            }
        }
        m_pGoldenDig.text = m_iGoldenDigCount.ToString();
        if (0 == m_iGoldenDigCount)
        {
            m_pButtons[2].color = m_pColorUseOut;
        }
        m_pMouseDig.text = m_iMouseCount.ToString();
        if (0 == m_iMouseCount)
        {
            m_pButtons[3].color = m_pColorUseOut;
        }

        for (int i = 0; i < m_pGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pGrids.GetLength(1); ++j)
            {
                if (m_pGrids[i, j].GetState() != EGridState.EGS_Empty)
                {
                    m_pGrids[i, j].SetEdge(m_ePressNow);
                }
            }            
        }
    }

    public void OnGameB1()
    {
        m_ePressNow = EPressType.EPT_Dig;
        SetButtonColors();
    }

    public void OnGameB2()
    {
        m_ePressNow = EPressType.EPT_Tag;
        SetButtonColors();
    }

    public void OnGameB3()
    {
        if (m_iGoldenDigCount > 0)
        {
            m_ePressNow = EPressType.EPT_GoldenDig;
            SetButtonColors();            
        }
    }

    public void OnGameB4()
    {
        if (m_iMouseCount > 0)
        {
            m_ePressNow = EPressType.EPT_Mouse;
            SetButtonColors();            
        }
    }

    private void RefreshNodesWithRemovedBomb(AGridToggle gridToggle)
    {
        for (int i = 0; i < gridToggle.m_pLines.Count; ++i)
        {
            for (int j = 0; j < gridToggle.m_pLines[i].m_iNodeXs.Count; ++j)
            {
                int iX = gridToggle.m_pLines[i].m_iNodeXs[j];
                int iY = gridToggle.m_pLines[i].m_iNodeYs[j];

                if (m_pGrids[iX, iY].GetState() == EGridState.EGS_OpenNoBomb
                 || m_pGrids[iX, iY].GetState() == EGridState.EGS_OpenGhost)
                {
                    RefreshOneNode(m_pGrids[iX, iY]);
                }
            }
        }
    }

    private void RefreshOneNode(AGridToggle gridToggle)
    {
        int iX = gridToggle.m_iX;
        int iY = gridToggle.m_iY;
        int[] iNumber = new int[(int)EDir.Max];
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            bool bMeetWall = false;
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
                }

                if (!bMeetWall)
                {
                    iCheckPosX = iX + PuzzleCreator.deltaX[i] * (j + 1);
                    iCheckPosY = iY + PuzzleCreator.deltaY[i] * (j + 1);
                    if (m_pPuzzle.HasWall(PuzzleCreator.GetOp((EDir)i), iCheckPosX, iCheckPosY))
                    {
                        bMeetWall = true;
                    }
                }
            }
        }

        string[] numbers = new string[(int)EDir.Max];
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            numbers[i] = bombNum[iNumber[i]];
        }

        m_pGrids[iX, iY].SetNumber(numbers);

        if (EGridState.EGS_OpenGhost == m_pGrids[iX, iY].GetState())
        {
            bool bHasGhost = false;
            for (int i = 0; i < m_pGrids[iX, iY].m_pLines.Count; ++i)
            {
                for (int j = 0; j < m_pGrids[iX, iY].m_pLines[i].m_iNodeXs.Count; ++j)
                {
                    if (m_pPuzzle.HasGhostBomb(
                        m_pGrids[iX, iY].m_pLines[i].m_iNodeXs[j],
                        m_pGrids[iX, iY].m_pLines[i].m_iNodeYs[j]))
                    {
                        bHasGhost = true;
                        break;
                    }
                }
                if (bHasGhost)
                {
                    break;
                }
            }
            if (!bHasGhost)
            {
                ++m_iOpenIndex;
                m_pGrids[iX, iY].SetGridState(EGridState.EGS_OpenNoBomb, EPressType.EPT_Max, m_iOpenIndex * 0.05f);
                m_pGrids[iX, iY].SetNumber(numbers);
            }
        }
    }

    private void FinishDig()
    {
        //======================================================
        //hide numbers
        for (int i = 0; i < m_pPuzzle.m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_ushGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_OpenNoBomb == m_pGrids[i, j].GetState())
                {
                    for (int k = 0; k < (int)EDir.Max; ++k)
                    {
                        int iNewX = i + PuzzleCreator.deltaX[k];
                        int iNewY = j + PuzzleCreator.deltaY[k];
                        if (m_pPuzzle.HasGrid(iNewX, iNewY)
                         && EGridState.EGS_OpenNoBomb == m_pGrids[iNewX, iNewY].GetState())
                        {
                            m_pGrids[i, j].m_pNumbers[k].text = "";
                        }
                    }
                }
            }
        }

        //======================================================
        //show edge and button
        SetButtonColors();

        //======================================================
        //mine number
        int iBomb = 0;
        int iTag = 0;
        int iSpecialBomb = 0;
        for (int i = 0; i < m_pPuzzle.m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_ushGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_Tag == m_pGrids[i, j].GetState())
                {
                    ++iTag;
                }
                if (m_pPuzzle.HasGhostBomb(i, j))
                {
                    ++iSpecialBomb;
                }
                if (m_pPuzzle.HasBomb(i, j))
                {
                    ++iBomb;
                }
            }
        }
        m_pMineNumber.text = (iBomb - iTag).ToString();
        m_pSpecialMineNumber.text = iSpecialBomb.ToString();

        //======================================================
        //check win
        int iAll = 0;
        for (int i = 0; i < m_pPuzzle.m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_ushGrids.GetLength(1); ++j)
            {
                if (!m_pPuzzle.IsEmpty(i, j) && !m_pPuzzle.HasBomb(i, j)
                  && EGridState.EGS_OpenNoBomb != m_pGrids[i, j].GetState()
                  && EGridState.EGS_OpenGhost != m_pGrids[i, j].GetState())
                {
                    ++iAll;
                    break;
                }
            }
            if (iAll > 0)
            {
                break;
            }
        }

        if (0 == iAll)
        {
            ShowResoult(true);
        }  
    }

    public Image m_pResoultBg;
    public Button[] m_pResoultButtons;
    public Image[] m_pResoultButtonImgs1;
    public Image[] m_pResoultButtonImgs2;
    public Text m_pRes;

    public void ShowResoult(bool bWin)
    {
        if (bWin && m_iCurrentLevel >= 1 && m_iCurrentLevel <= 64)
        {
            PlayerSave.m_bLevelPassed[m_iCurrentLevel - 1] = true;
            PlayerSave.Save();
        }

        m_pResoultBg.enabled = true;
        m_pRes.enabled = true;
        m_pRes.text = bWin ? "<color=#00FFFF>You Win!</color>" : "<color=#FF0000>You Lose!</color>";
        if (bWin && m_iCurrentLevel >= 1 && m_iCurrentLevel < 64)
        {
            m_pResoultButtons[0].transform.localPosition = new Vector3(-80.0F, -120.0F, 0.0F);
            m_pResoultButtons[1].transform.localPosition = new Vector3(0.0F, -120.0F, 0.0F);
            m_pResoultButtons[2].transform.localPosition = new Vector3(80.0F, -120.0F, 0.0F);
            for (int i = 0; i < 3; ++i)
            {
                m_pResoultButtons[i].enabled = true;
                m_pResoultButtonImgs1[i].enabled = true;
                m_pResoultButtonImgs2[i].enabled = true;
            }
        }
        else
        {
            m_pResoultButtons[0].transform.localPosition = new Vector3(-50.0F, -120.0F, 0.0F);
            m_pResoultButtons[1].transform.localPosition = new Vector3(50.0F, -120.0F, 0.0F);
            for (int i = 0; i < 3; ++i)
            {
                m_pResoultButtons[i].enabled = (2 != i);
                m_pResoultButtonImgs1[i].enabled = (2 != i);
                m_pResoultButtonImgs2[i].enabled = (2 != i);
            }
        }
    }

    public void HideResoult()
    {
        m_pResoultBg.enabled = false;
        m_pRes.enabled = false;
        for (int i = 0; i < 3; ++i)
        {
            m_pResoultButtons[i].enabled = false;
            m_pResoultButtonImgs1[i].enabled = false;
            m_pResoultButtonImgs2[i].enabled = false;
        }
    }

    public void OnResoultDialogQuit()
    {
        if (m_iCurrentLevel >= 1 && m_iCurrentLevel <= 64)
        {
            m_pPages[(int)EPage.EP_Start].SetActive(false);
            m_pPages[(int)EPage.EP_Choose].SetActive(true);
            m_pPages[(int)EPage.EP_Game].SetActive(false);
            OnEnterChooseLevel();
        }
        else
        {
            m_pPages[(int)EPage.EP_Start].SetActive(true);
            m_pPages[(int)EPage.EP_Choose].SetActive(false);
            m_pPages[(int)EPage.EP_Game].SetActive(false);
        }
    }

    public void OnResoultDialogRestart()
    {
        GoLevel(m_iCurrentLevel);
    }

    public void OnResoultDialogNext()
    {
        GoLevel(m_iCurrentLevel + 1);
    }

    #endregion

    #region Dialog

    public GameObject m_pDialog;
    public Text m_pDialogTx;
    public Action m_pYesAction;
    public Action m_pNoAction;

    public void ShowDialog(string sContent, Action yesAction, Action noAction = null)
    {
        m_pDialogTx.text = sContent;
        m_pDialog.SetActive(true);
        m_pYesAction = yesAction;
        m_pNoAction = noAction;
    }

    public void HideDialog()
    {
        m_pDialog.SetActive(false);
    }

    public void DialogYes()
    {
        HideDialog();
        if (null != m_pYesAction)
        {
            m_pYesAction();
        }
    }

    public void DialogNo()
    {
        HideDialog();
        if (null != m_pNoAction)
        {
            m_pNoAction();
        }
    }

    #endregion
}

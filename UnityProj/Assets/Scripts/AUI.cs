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
	void Update () 
    {
	    if (m_pPages[(int) EPage.EP_Game].activeSelf && 0 == m_iCurrentLevel)
	    {
	        TickTraining(Time.deltaTime);
	    }
    }

    #region Start

    public void OnQuitButton()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        ShowDialog("Do you want to quit?", OnRealQuit);
    }

    private void OnRealQuit()
    {
        Application.Quit();
    }

    public void OnChooseLevelButton()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        m_pPages[(int)EPage.EP_Start].SetActive(false);
        m_pPages[(int)EPage.EP_Choose].SetActive(true);
        m_pPages[(int)EPage.EP_Game].SetActive(false);
        OnEnterChooseLevel();
    }

    public void OnStartGameButton()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        GoLevel(CConst.LevelCount + 1);
    }

    public void OnTrainingButton()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        GoLevel(0);
    }

    #endregion

    #region Choose

    public int m_iCurrentLevel = 0;
    public void GoLevel(int iLevel)
    {
        m_iCurrentLevel = iLevel;
        if (0 == iLevel)
        {
            CPuzzle npz = new CPuzzle();
            npz.m_ushGrids = new ushort[5,5];
            npz.SetEmpty(0, 0); npz.SetEmpty(0, 4);
            npz.SetEmpty(1, 0); npz.SetEmpty(1, 4);
            npz.SetEmpty(2, 0); npz.SetEmpty(2, 4);
            npz.SetEmpty(3, 0); npz.SetEmpty(3, 4);
            npz.SetEmpty(4, 0); npz.SetEmpty(4, 4);

            npz.SetEmpty(0, 1); npz.SetEmpty(0, 3);
            npz.SetEmpty(1, 1); npz.SetEmpty(1, 3);
            npz.SetEmpty(2, 1); npz.SetEmpty(2, 3);
            npz.SetEmpty(4, 1); npz.SetEmpty(4, 3);

            npz.PutBomb(3, 1);
            npz.PutBomb(1, 2);
            npz.PutBomb(3, 2);

            npz.m_ushGrids[3, 3] |= (1 << (int)EDir.RU);

            npz.m_pLines = new CLines[4];
            npz.m_pLines[0] = new CLines
            {
                m_eType = ELineType.ELT_Normal,
                m_iStartX = 0,
                m_iStartY = 2,
                m_iEndX = 4,
                m_iEndY = 2,
            };
            npz.m_pLines[0].m_iNodeXs = new List<int>();npz.m_pLines[0].m_iNodeYs = new List<int>();
            npz.m_pLines[0].m_iNodeXs.Add(0); npz.m_pLines[0].m_iNodeYs.Add(2);
            npz.m_pLines[0].m_iNodeXs.Add(1); npz.m_pLines[0].m_iNodeYs.Add(2);
            npz.m_pLines[0].m_iNodeXs.Add(2); npz.m_pLines[0].m_iNodeYs.Add(2);
            npz.m_pLines[0].m_iNodeXs.Add(3); npz.m_pLines[0].m_iNodeYs.Add(2);
            npz.m_pLines[0].m_iNodeXs.Add(4); npz.m_pLines[0].m_iNodeYs.Add(2);

            npz.m_pLines[1] = new CLines
            {
                m_eType = ELineType.ELT_Normal,
                m_iStartX = 3,
                m_iStartY = 1,
                m_iEndX = 3,
                m_iEndY = 3,
            };
            npz.m_pLines[1].m_iNodeXs = new List<int>(); npz.m_pLines[1].m_iNodeYs = new List<int>();
            npz.m_pLines[1].m_iNodeXs.Add(3); npz.m_pLines[1].m_iNodeYs.Add(1);
            npz.m_pLines[1].m_iNodeXs.Add(3); npz.m_pLines[1].m_iNodeYs.Add(2);
            npz.m_pLines[1].m_iNodeXs.Add(3); npz.m_pLines[1].m_iNodeYs.Add(3);

            npz.m_pLines[2] = new CLines
            {
                m_eType = ELineType.ELT_Normal,
                m_iStartX = 3,
                m_iStartY = 1,
                m_iEndX = 2,
                m_iEndY = 2,
            };
            npz.m_pLines[2].m_iNodeXs = new List<int>(); npz.m_pLines[2].m_iNodeYs = new List<int>();
            npz.m_pLines[2].m_iNodeXs.Add(3); npz.m_pLines[2].m_iNodeYs.Add(1);
            npz.m_pLines[2].m_iNodeXs.Add(2); npz.m_pLines[2].m_iNodeYs.Add(2);

            npz.m_pLines[3] = new CLines
            {
                m_eType = ELineType.ELT_Normal,
                m_iStartX = 3,
                m_iStartY = 3,
                m_iEndX = 2,
                m_iEndY = 2,
            };
            npz.m_pLines[3].m_iNodeXs = new List<int>(); npz.m_pLines[3].m_iNodeYs = new List<int>();
            npz.m_pLines[3].m_iNodeXs.Add(3); npz.m_pLines[3].m_iNodeYs.Add(3);
            npz.m_pLines[3].m_iNodeXs.Add(2); npz.m_pLines[3].m_iNodeYs.Add(2);

            GoPuzzle(npz);
        }
        else if (iLevel > 0 && iLevel <= CConst.LevelCount)
        {
            byte[] data = Resources.Load<TextAsset>("puzzle_" + iLevel).bytes;
            CPuzzle pz = new CPuzzle();
            pz.FromByteArray(data);
            GoPuzzle(pz);
        }
        else if (iLevel > CConst.LevelCount)
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
        if (0 == m_iCurrentLevel)
        {
            OnEnterTraining();
        }
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

        m_pLevelBts = new GameObject[CConst.LevelCount];
        for (int i = 0; i < CConst.LevelCount; ++i)
        {
            m_pLevelBts[i] = Instantiate(m_pLevelBtPrefab, m_pLevelBtPrefab.transform.parent);
            int iX = i % 12;
            int iY = i / 12;
            m_pLevelBts[i].transform.localPosition = new Vector3((iX - 5.5f) * 60.0f, (3.5f - iY) * 60.0f, 0.0f);
            m_pLevelBts[i].name = "Level" + (i + 1);
            m_pLevelBts[i].GetComponentInChildren<Text>().text = (i + 1).ToString();

            if (0 == i || PlayerSave.m_bLevelPassed[i - 1])
            {
                if (PlayerSave.m_bLevelPassed[i])
                {
                    m_pLevelBts[i].GetComponent<Image>().color = m_pColorToggled;
                }
                else
                {
                    m_pLevelBts[i].GetComponent<Image>().color = m_pColorUnToggled;
                }
            }
            else
            {
                m_pLevelBts[i].GetComponent<Image>().color = m_pColorUseOut;
            }
        }
    }

    public void OnPressChooseLevelButton(GameObject bt)
    {
        for (int i = 0; i < CConst.LevelCount; ++i)
        {
            if (bt.name.Equals("Level" + (i + 1)))
            {
                if (0 == i || PlayerSave.m_bLevelPassed[i - 1])
                {
                    ASound.Sound.PlayUISound(EUISound.EUS_Press);
                    GoLevel(i + 1);
                    return;
                }
                else
                {
                    ASound.Sound.PlayUISound(EUISound.EUS_Error);
                }
            }
        }
    }

    public void OnChooseLevelBack()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        m_pPages[(int)EPage.EP_Start].SetActive(true);
        m_pPages[(int)EPage.EP_Choose].SetActive(false);
        m_pPages[(int)EPage.EP_Game].SetActive(false);
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

        m_pTrainingText.enabled = (0 == m_iCurrentLevel);
    }

    private void OnTagMine(int iX, int iY)
    {
        if (m_bOver)
        {
            return;
        }

        ASound.Sound.PlayUISound(EUISound.EUS_Tag);
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
            ASound.Sound.PlayUISound(EUISound.EUS_Explode);
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

        for (int i = 0; i < m_pGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pGrids.GetLength(1); ++j)
            {
                if (!m_pGrids[i, j].IsReady())
                {
                    return;
                }
            }            
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
                else
                {
                    ASound.Sound.PlayUISound(EUISound.EUS_Error);
                }
                break;
            case EPressType.EPT_Dig:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_CloseSafe == m_pGrids[iX, iY].GetState())
                {
                    OnDig(iX, iY, false);
                    FinishDig();
                }
                else
                {
                    ASound.Sound.PlayUISound(EUISound.EUS_Error);
                }
                break;
            case EPressType.EPT_GoldenDig:
                if (m_iGoldenDigCount > 0)
                {
                    if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
                        || EGridState.EGS_CloseSafe == m_pGrids[iX, iY].GetState()
                        || EGridState.EGS_CloseFrozen == m_pGrids[iX, iY].GetState())
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
                    else
                    {
                        ASound.Sound.PlayUISound(EUISound.EUS_Error);
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
                    else
                    {
                        ASound.Sound.PlayUISound(EUISound.EUS_Error);
                    }
                }
                break;
        }
    }

    public void OnGameRefreshButton()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        ShowDialog("Do you want to restart?", OnRealGameRefreshButton);
    }

    private void OnRealGameRefreshButton()
    {
        GoLevel(m_iCurrentLevel);
    }

    public void OnGameQuitButton()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        ShowDialog("Do you want to quit?", OnRealGameQuitButton);
    }

    private void OnRealGameQuitButton()
    {
        if (m_iCurrentLevel >= 1 && m_iCurrentLevel <= CConst.LevelCount)
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
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        m_ePressNow = EPressType.EPT_Dig;
        SetButtonColors();
    }

    public void OnGameB2()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        m_ePressNow = EPressType.EPT_Tag;
        SetButtonColors();
    }

    public void OnGameB3()
    {
        if (m_iGoldenDigCount > 0)
        {
            ASound.Sound.PlayUISound(EUISound.EUS_Press);
            m_ePressNow = EPressType.EPT_GoldenDig;
            SetButtonColors();
        }
        else
        {
            ASound.Sound.PlayUISound(EUISound.EUS_Error);
        }
    }

    public void OnGameB4()
    {
        if (m_iMouseCount > 0)
        {
            ASound.Sound.PlayUISound(EUISound.EUS_Press);
            m_ePressNow = EPressType.EPT_Mouse;
            SetButtonColors();
        }
        else
        {
            ASound.Sound.PlayUISound(EUISound.EUS_Error);
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
        if (bWin && m_iCurrentLevel >= 1 && m_iCurrentLevel <= CConst.LevelCount)
        {
            PlayerSave.m_bLevelPassed[m_iCurrentLevel - 1] = true;
            PlayerSave.Save();
        }
        //ASound.Sound.PlayUISound(bWin ? EUISound.EUS_Win : EUISound.EUS_Lose);

        m_pResoultBg.enabled = true;
        m_pRes.enabled = true;
        m_pRes.text = bWin ? "<color=#00FFFF>You Win!</color>" : "<color=#FF0000>You Lose!</color>";
        if (bWin && m_iCurrentLevel >= 1 && m_iCurrentLevel < CConst.LevelCount)
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
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        if (m_iCurrentLevel >= 1 && m_iCurrentLevel <= CConst.LevelCount)
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
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        GoLevel(m_iCurrentLevel);
    }

    public void OnResoultDialogNext()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        GoLevel(m_iCurrentLevel + 1);
    }

    #endregion

    #region Training

    public Text m_pTrainingText;
    private List<Text> m_pLineHighLightTexts;
    private List<List<Image>> m_pLineHighLightImages;
    private List<List<Color>> m_pLineHighLightImageColors;
    private float m_fTraningTimer = 0.0f;
    public void OnEnterTraining()
    {
        m_pGrids[0, 2].SetGridState(EGridState.EGS_OpenNoBomb, EPressType.EPT_Max);
        m_pGrids[0, 2].SetNumber(new[] { bombNum[0], bombNum[2], bombNum[0], bombNum[0], bombNum[0], bombNum[0] });
        m_pGrids[2, 2].SetGridState(EGridState.EGS_OpenNoBomb, EPressType.EPT_Max);
        m_pGrids[2, 2].SetNumber(new[] { bombNum[1], bombNum[1], bombNum[0], bombNum[0], bombNum[1], bombNum[0] });
        m_pGrids[3, 3].SetGridState(EGridState.EGS_OpenNoBomb, EPressType.EPT_Max);
        m_pGrids[3, 3].SetNumber(new[] { bombNum[0], bombNum[0], bombNum[0], bombNum[0], bombNum[0], bombNum[2] });

        m_pGrids[4, 2].SetGridState(EGridState.EGS_Close, EPressType.EPT_Max);

        m_pLineHighLightTexts = new List<Text>();
        m_pLineHighLightImages = new List<List<Image>>();
        m_pLineHighLightImageColors = new List<List<Color>>();

        //===============================================
        //line 1
        m_pLineHighLightTexts.Add(m_pGrids[2, 2].m_pNumbers[(int)EDir.RU]);
        List<Image> img1 = new List<Image>();
        List<Color> color1 = new List<Color>();

        img1.Add(m_pGrids[3, 1].GetComponent<Image>());
        Color c11 = m_pGrids[3, 1].GetComponent<Image>().color; 
        color1.Add(c11);
        color1.Add(new Color(c11.r, c11.g, c11.b, 0.5f));
        m_pGrids[3, 1].m_pBomb.enabled = true;
        m_pGrids[3, 1].m_pBomb.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img1.Add(m_pGrids[3, 1].m_pBomb);
        color1.Add(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        color1.Add(new Color(1.0f, 1.0f, 1.0f, 0.5f));
        m_pLineHighLightImages.Add(img1);
        m_pLineHighLightImageColors.Add(color1);

        //===============================================
        //line 2
        m_pLineHighLightTexts.Add(m_pGrids[0, 2].m_pNumbers[(int)EDir.Right]);
        List<Image> img2 = new List<Image>();
        List<Color> color2 = new List<Color>();

        img2.Add(m_pGrids[1, 2].GetComponent<Image>());
        Color c21 = m_pGrids[1, 2].GetComponent<Image>().color;
        color2.Add(c21);
        color2.Add(new Color(c21.r, c21.g, c21.b, 0.5f));

        img2.Add(m_pGrids[3, 2].GetComponent<Image>());
        Color c22 = m_pGrids[3, 2].GetComponent<Image>().color;
        color2.Add(c22);
        color2.Add(new Color(c22.r, c22.g, c22.b, 0.5f));

        img2.Add(m_pGrids[4, 2].GetComponent<Image>());
        Color c23 = m_pGrids[4, 2].GetComponent<Image>().color;
        color2.Add(c23);
        color2.Add(new Color(c23.r, c23.g, c23.b, 0.5f));

        img2.Add(m_pGrids[2, 2].GetComponent<Image>());
        Color c24 = m_pGrids[2, 2].GetComponent<Image>().color;
        color2.Add(c24);
        color2.Add(new Color(c24.r, c24.g, c24.b, 0.5f));

        m_pGrids[1, 2].m_pBomb.enabled = true;
        m_pGrids[1, 2].m_pBomb.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img2.Add(m_pGrids[1, 2].m_pBomb);
        color2.Add(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        color2.Add(new Color(1.0f, 1.0f, 1.0f, 0.5f));

        m_pGrids[3, 2].m_pBomb.enabled = true;
        m_pGrids[3, 2].m_pBomb.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img2.Add(m_pGrids[3, 2].m_pBomb);
        color2.Add(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        color2.Add(new Color(1.0f, 1.0f, 1.0f, 0.5f));

        m_pLineHighLightImages.Add(img2);
        m_pLineHighLightImageColors.Add(color2);

        //===============================================
        //line 3
        m_pLineHighLightTexts.Add(m_pGrids[2, 2].m_pNumbers[(int)EDir.Right]);
        List<Image> img3 = new List<Image>();
        List<Color> color3 = new List<Color>();
        img3.Add(m_pGrids[3, 2].GetComponent<Image>());
        Color c31 = m_pGrids[3, 2].GetComponent<Image>().color;
        color3.Add(c31);
        color3.Add(new Color(c31.r, c31.g, c31.b, 0.5f));

        img3.Add(m_pGrids[4, 2].GetComponent<Image>());
        Color c32 = m_pGrids[4, 2].GetComponent<Image>().color;
        color3.Add(c32);
        color3.Add(new Color(c32.r, c32.g, c32.b, 0.5f));

        m_pGrids[3, 2].m_pBomb.enabled = true;
        m_pGrids[3, 2].m_pBomb.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img3.Add(m_pGrids[3, 2].m_pBomb);
        color3.Add(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        color3.Add(new Color(1.0f, 1.0f, 1.0f, 0.5f));

        m_pLineHighLightImages.Add(img3);
        m_pLineHighLightImageColors.Add(color3);

        //===============================================
        //line 4
        m_pLineHighLightTexts.Add(m_pGrids[3, 3].m_pNumbers[(int)EDir.Up]);
        List<Image> img4 = new List<Image>();
        List<Color> color4 = new List<Color>();
        img4.Add(m_pGrids[3, 1].GetComponent<Image>());
        Color c41 = m_pGrids[3, 1].GetComponent<Image>().color;
        color4.Add(c41);
        color4.Add(new Color(c41.r, c41.g, c41.b, 0.5f));

        img4.Add(m_pGrids[3, 2].GetComponent<Image>());
        Color c42 = m_pGrids[3, 2].GetComponent<Image>().color;
        color4.Add(c42);
        color4.Add(new Color(c42.r, c42.g, c42.b, 0.5f));

        m_pGrids[3, 1].m_pBomb.enabled = true;
        m_pGrids[3, 1].m_pBomb.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img4.Add(m_pGrids[3, 1].m_pBomb);
        color4.Add(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        color4.Add(new Color(1.0f, 1.0f, 1.0f, 0.5f));

        m_pGrids[3, 2].m_pBomb.enabled = true;
        m_pGrids[3, 2].m_pBomb.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img4.Add(m_pGrids[3, 2].m_pBomb);
        color4.Add(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        color4.Add(new Color(1.0f, 1.0f, 1.0f, 0.5f));

        m_pLineHighLightImages.Add(img4);
        m_pLineHighLightImageColors.Add(color4);

        //===============================================
        //line 5
        m_pLineHighLightTexts.Add(m_pGrids[2, 2].m_pNumbers[(int)EDir.Left]);
        List<Image> img5 = new List<Image>();
        List<Color> color5 = new List<Color>();
        img5.Add(m_pGrids[1, 2].GetComponent<Image>());
        Color c51 = m_pGrids[1, 2].GetComponent<Image>().color;
        color5.Add(c51);
        color5.Add(new Color(c51.r, c51.g, c51.b, 0.5f));

        img5.Add(m_pGrids[0, 2].GetComponent<Image>());
        Color c52 = m_pGrids[0, 2].GetComponent<Image>().color;
        color5.Add(c52);
        color5.Add(new Color(c52.r, c52.g, c52.b, 0.5f));

        m_pGrids[1, 2].m_pBomb.enabled = true;
        m_pGrids[1, 2].m_pBomb.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        img5.Add(m_pGrids[1, 2].m_pBomb);
        color5.Add(new Color(1.0f, 1.0f, 1.0f, 0.0f));
        color5.Add(new Color(1.0f, 1.0f, 1.0f, 0.5f));

        m_pLineHighLightImages.Add(img5);
        m_pLineHighLightImageColors.Add(color5);
    }

    public void TickTraining(float fDeltaTime)
    {
        m_fTraningTimer += fDeltaTime;
        if (m_fTraningTimer > 10.0f)
        {
            m_fTraningTimer = 0.0f;
        }
        int iStage = 0;
        float fRate = 0.0f;
        if (m_fTraningTimer < 2.0f)
        {
            iStage = 0;
            fRate = Mathf.Clamp01(1.0f - Mathf.Abs(m_fTraningTimer - 1.0f));
        }
        else if (m_fTraningTimer < 4.0f)
        {
            iStage = 1;
            fRate = Mathf.Clamp01(1.0f - Mathf.Abs(m_fTraningTimer - 3.0f));            
        }
        else if (m_fTraningTimer < 6.0f)
        {
            iStage = 2;
            fRate = Mathf.Clamp01(1.0f - Mathf.Abs(m_fTraningTimer - 5.0f));
        }
        else if (m_fTraningTimer < 8.0f)
        {
            iStage = 3;
            fRate = Mathf.Clamp01(1.0f - Mathf.Abs(m_fTraningTimer - 7.0f));
        }
        else
        {
            iStage = 4;
            fRate = Mathf.Clamp01(1.0f - Mathf.Abs(m_fTraningTimer - 9.0f));
        }

        for (int i = 0; i < 5; ++i)
        {
            if (i != iStage)
            {
                float fRealRate = 0.0f;
                m_pLineHighLightTexts[i].transform.localScale = new Vector3(
                    1.0f + fRealRate * 0.6f, 
                    1.0f + fRealRate * 0.6f, 
                    1.0f + fRealRate * 0.6f);
                m_pLineHighLightTexts[i].color = (1.0f - fRealRate)*Color.white + fRealRate*Color.yellow;
                if (m_pLineHighLightTexts[i].text.Equals("1") 
                 || m_pLineHighLightTexts[i].text.Equals(bombNum[1]))
                {
                    m_pLineHighLightTexts[i].text = bombNum[1];
                }

                if (m_pLineHighLightTexts[i].text.Equals("2")
                 || m_pLineHighLightTexts[i].text.Equals(bombNum[2]))
                {
                    m_pLineHighLightTexts[i].text = bombNum[2];
                }

                for (int j = 0; j < m_pLineHighLightImages[i].Count; ++j)
                {
                    m_pLineHighLightImages[i][j].color = (1.0F - fRealRate) * m_pLineHighLightImageColors[i][j * 2] +
                                                         fRealRate * m_pLineHighLightImageColors[i][j * 2 + 1];
                }
            }
        }

        for (int i = 0; i < 5; ++i)
        {
            if (i == iStage)
            {
                float fRealRate = fRate;
                m_pLineHighLightTexts[i].transform.localScale = new Vector3(
                    1.0f + fRealRate * 0.6f,
                    1.0f + fRealRate * 0.6f,
                    1.0f + fRealRate * 0.6f);

                m_pLineHighLightTexts[i].color = (1.0f - fRealRate) * Color.white + fRealRate * Color.yellow;
                if (m_pLineHighLightTexts[i].text.Equals("1")
                 || m_pLineHighLightTexts[i].text.Equals(bombNum[1]))
                {
                    m_pLineHighLightTexts[i].text = "1";
                }

                if (m_pLineHighLightTexts[i].text.Equals("2")
                 || m_pLineHighLightTexts[i].text.Equals(bombNum[2]))
                {
                    m_pLineHighLightTexts[i].text = "2";
                }

                for (int j = 0; j < m_pLineHighLightImages[i].Count; ++j)
                {
                    m_pLineHighLightImages[i][j].enabled = true;
                    m_pLineHighLightImages[i][j].color = (1.0F - fRealRate) * m_pLineHighLightImageColors[i][j * 2] +
                                                         fRealRate * m_pLineHighLightImageColors[i][j * 2 + 1];
                }
            }
        }
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
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        HideDialog();
        if (null != m_pYesAction)
        {
            m_pYesAction();
        }
    }

    public void DialogNo()
    {
        ASound.Sound.PlayUISound(EUISound.EUS_Press);
        HideDialog();
        if (null != m_pNoAction)
        {
            m_pNoAction();
        }
    }

    #endregion
}

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
        UI = this;
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
        newPuzzle.PutGhostBomb(GhostMineNumber);
        PutPuzzle(newPuzzle);

        m_pPages[(int)EPage.EP_Start].SetActive(false);
        m_pPages[(int)EPage.EP_Game].SetActive(true);

        m_bOver = false;
        m_pRes.enabled = false;
    }

    #endregion

    #region Game

    public const int MineNumber = 25;
    public const int GhostMineNumber = 2;
    public const int WallNumber = 30;
    public const int EmptyNumber = 10;
    public const int TestWidth = 9;
    public const int TestHeight = 9;
    public AGridToggle m_pOneGrid;
    public GameObject m_pOneLine;
    public AGridToggle[,] m_pGrids;
    public GameObject[] m_pLines;
    public CPuzzle m_pPuzzle;

    public Text m_pMineNumber;
    public Text m_pRes;
    public bool m_bOver = false;

    public int m_iBombNumber = 0;
    public int m_iGoldenDigCount = 3;
    public int m_iMouseCount = 1;

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
                else
                {
                    newG.SetGridState(EGridState.EGS_Close, EPressType.EPT_Max);    
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
                if (1 == m_pGrids[i, j].m_pStartLines.Count && 1== m_pGrids[i, j].m_pLines.Count)
                {
                    m_pGrids[i, j].SetGridState(EGridState.EGS_CloseSafe, EPressType.EPT_Max);
                }
            }
        }
        m_iBombNumber = puzzle.GetBombNumber();
        m_pMineNumber.text = m_iBombNumber.ToString();
        m_iGoldenDigCount = 3;
        m_iMouseCount = 1;
        m_ePressNow = EPressType.EPT_Dig;
        SetButtonColors();
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

        if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState())
        {
            m_pGrids[iX, iY].SetGridState(EGridState.EGS_Tag, EPressType.EPT_Max);
        }
        else if (EGridState.EGS_Tag == m_pGrids[iX, iY].GetState())
        {
            m_pGrids[iX, iY].SetGridState(EGridState.EGS_Close, EPressType.EPT_Max);
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
        int iBomb = m_iBombNumber;
        for (int i = 0; i < m_pPuzzle.m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_ushGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_Tag == m_pGrids[i, j].GetState())
                {
                    --iBomb;
                }
            }            
        }
        m_pMineNumber.text = iBomb.ToString();
    }

    private void OnDig(int iX, int iY, bool bLink)
    {
        if (m_pPuzzle.HasBomb(iX, iY))
        {
            m_pGrids[iX, iY].PlayEffect(EGridEffect.EGE_Explode);
            ShowAllBombs();
            m_bOver = true;
            m_pRes.text = "You Lose!";
            m_pRes.enabled = true;
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

        for (int i = 0; i < m_pPuzzle.m_ushGrids.GetLength(0); ++i)
        {
            for (int j = 0; j < m_pPuzzle.m_ushGrids.GetLength(1); ++j)
            {
                if (EGridState.EGS_OpenNoBomb == m_pGrids[i, j].GetState())
                {
                    for (int k = 0; k < (int) EDir.Max; ++k)
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

        //Check Win
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
            m_bOver = true;
            m_pRes.text = "You Win!";
            m_pRes.enabled = true;
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
                    --m_iBombNumber;
                    m_pPuzzle.RemoveBomb(iCheckPosX, iCheckPosY);
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
                 || EGridState.EGS_Tag == m_pGrids[iX, iY].GetState())
                {
                    OnTagMine(iX, iY);
                }
                break;
            case EPressType.EPT_Dig:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_CloseSafe == m_pGrids[iX, iY].GetState())
                {
                    OnDig(iX, iY, false);
                }
                break;
            case EPressType.EPT_GoldenDig:
                if (EGridState.EGS_Close == m_pGrids[iX, iY].GetState()
                 || EGridState.EGS_CloseSafe == m_pGrids[iX, iY].GetState())
                {
                    if (m_pPuzzle.HasBomb(iX, iY))
                    {
                        m_pPuzzle.RemoveBomb(iX, iY);
                        --m_iBombNumber;
                        int iBomb = m_iBombNumber;
                        for (int i = 0; i < m_pPuzzle.m_ushGrids.GetLength(0); ++i)
                        {
                            for (int j = 0; j < m_pPuzzle.m_ushGrids.GetLength(1); ++j)
                            {
                                if (EGridState.EGS_Tag == m_pGrids[i, j].GetState())
                                {
                                    --iBomb;
                                }
                            }
                        }
                        m_pMineNumber.text = iBomb.ToString();

                        RefreshNodesWithRemovedBomb(m_pGrids[iX, iY]);
                    }
                    OnDig(iX, iY, false);
                }
                break;
            case EPressType.EPT_Mouse:
                if (EGridState.EGS_OpenNoBomb == m_pGrids[iX, iY].GetState())
                {
                    OnMouseDig(iX, iY);
                }
                break;
        }
    }

    public void OnGameRefreshButton()
    {
        CPuzzle newPuzzle = PuzzleCreator.CreatePuzzle(TestWidth, TestHeight, WallNumber, EmptyNumber);
        Debug.Log(newPuzzle.m_pLines.Length);
        newPuzzle.PutBombs(MineNumber);
        newPuzzle.PutGhostBomb(GhostMineNumber);
        PutPuzzle(newPuzzle);

        m_pPages[(int)EPage.EP_Start].SetActive(false);
        m_pPages[(int)EPage.EP_Game].SetActive(true);

        m_bOver = false;
        m_pRes.enabled = false;
    }

    public void OnGameQuitButton()
    {
        
    }

    public Image[] m_pButtons;
    public Text m_pGoldenDig;
    public Text m_pMouseDig;
    public EPressType m_ePressNow = EPressType.EPT_Dig;

    public static readonly Color m_pColorToggled = new Color(216.0f / 255.0f, 252.0f / 255.0f, 255.0f / 255.0f);
    public static readonly Color m_pColorUnToggled = new Color(255.0f / 255.0f, 245.0f / 255.0f, 188.0f / 255.0f);
    public static readonly Color m_pColorUseOut = new Color(133.0f / 255.0f, 133.0f / 255.0f, 133.0f / 255.0f);

    private void SetButtonColors()
    {
        for (int i = 0; i < (int)EPressType.EPT_Max; ++i)
        {
            if (i == (int) m_ePressNow)
            {
                m_pButtons[i].color = m_pColorToggled;
            }
            else
            {
                m_pButtons[i].color = m_pColorUnToggled;
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
        m_ePressNow = EPressType.EPT_GoldenDig;
        SetButtonColors();
    }

    public void OnGameB4()
    {
        m_ePressNow = EPressType.EPT_Mouse;
        SetButtonColors();
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

    #endregion
}

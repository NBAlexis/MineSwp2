using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class APuzzleCreator : MonoBehaviour
{

    public bool CreatePuzzles = false;
	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (CreatePuzzles)
	    {
	        CreatePuzzles = false;
            CreatePuzzleFunc();
	    }
	}

    public void CreatePuzzleFunc()
    {
        for (int i = 0; i < CConst.LevelCount; ++i)
        {
            int iMineNumber = CConst.TR_MineNumber;
            int iGhostMineNumber = CConst.TR_GhostMineNumber;
            int iWallNumber = CConst.TR_WallNumber;
            int iEmptyNumber = CConst.TR_EmptyNumber;
            int iFrozenNumber = CConst.TR_FrozenNumber;
            int iTestWidth = CConst.TR_TestWidth;
            int iTestHeight = CConst.TR_TestHeight;
            int iGold = CConst.TR_Gold;
            int iMouse = CConst.TR_Mouse;

            int iChoose = i/8;
            if (iChoose >= 8)
            {
                iChoose = Random.Range(3, 8);
            }

            switch (iChoose)
            {
                case 0:
                    if (i >= 2)
                    {
                        iMineNumber = CConst.E_MineNumber;
                        iGhostMineNumber = CConst.E_GhostMineNumber;
                        iWallNumber = CConst.E_WallNumber;
                        iEmptyNumber = CConst.E_EmptyNumber;
                        iFrozenNumber = CConst.E_FrozenNumber;
                        iTestWidth = CConst.E_TestWidth;
                        iTestHeight = CConst.E_TestHeight;
                        iGold = CConst.E_Gold;
                        iMouse = CConst.E_Mouse;
                    }
                    break;
                case 1:
                        iMineNumber = CConst.EM_MineNumber;
                        iGhostMineNumber = CConst.EM_GhostMineNumber;
                        iWallNumber = CConst.EM_WallNumber;
                        iEmptyNumber = CConst.EM_EmptyNumber;
                        iFrozenNumber = CConst.EM_FrozenNumber;
                        iTestWidth = CConst.EM_TestWidth;
                        iTestHeight = CConst.EM_TestHeight;
                        iGold = CConst.EM_Gold;
                        iMouse = CConst.EM_Mouse;
                    break;
                case 2:
                        iMineNumber = CConst.M_MineNumber;
                        iGhostMineNumber = CConst.M_GhostMineNumber;
                        iWallNumber = CConst.M_WallNumber;
                        iEmptyNumber = CConst.M_EmptyNumber;
                        iFrozenNumber = CConst.M_FrozenNumber;
                        iTestWidth = CConst.M_TestWidth;
                        iTestHeight = CConst.M_TestHeight;
                        iGold = CConst.M_Gold;
                        iMouse = CConst.M_Mouse;
                    break;
                case 3:
                        iMineNumber = CConst.MH_MineNumber;
                        iGhostMineNumber = CConst.MH_GhostMineNumber;
                        iWallNumber = CConst.MH_WallNumber;
                        iEmptyNumber = CConst.MH_EmptyNumber;
                        iFrozenNumber = CConst.MH_FrozenNumber;
                        iTestWidth = CConst.MH_TestWidth;
                        iTestHeight = CConst.MH_TestHeight;
                        iGold = CConst.MH_Gold;
                        iMouse = CConst.MH_Mouse;
                    break;
                case 4:
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
                case 5:
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
                case 6:
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
                case 7:
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

            string sFile = Application.dataPath + "/Resources/puzzle_" + (i + 1) + ".bytes";
            File.WriteAllBytes(sFile, newPuzzle.ToByteArray());            
        }
    }
}

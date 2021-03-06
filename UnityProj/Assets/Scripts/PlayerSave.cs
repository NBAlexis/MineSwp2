﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSave
{
    public static bool m_bInitialed = false;
    public static bool[] m_bLevelPassed;

    public static void Initial()
    {
        if (m_bInitialed)
        {
            return;
        }
        m_bInitialed = true;
        m_bLevelPassed = new bool[CConst.LevelCount];
        for (int i = 0; i < CConst.LevelCount; ++i)
        {
            m_bLevelPassed[i] = (1 == PlayerPrefs.GetInt("LVP_" + (i + 1), 0));
        }
    }

    public static void Save()
    {
        for (int i = 0; i < CConst.LevelCount; ++i)
        {
            PlayerPrefs.SetInt("LVP_" + (i + 1), m_bLevelPassed[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum EGridState
{
    EGS_Close,
    EGS_CloseFrozen,
    EGS_CloseSafe,
    EGS_Tag,
    EGS_OpenNoBomb,
    EGS_OpenGhost,
    EGS_OpenBomb,
    EGS_Empty,
}

public enum EGridEffect
{
    EGE_Dig,
    EGE_DigGolden,
    EGE_Explode,
    EGE_DigLink,
    EGE_MouseLink,
    EGE_FrozenLink,
}

public class AGridToggle : MonoBehaviour
{
    public Button m_pButton;
    public Image m_pButtonIcon;
    public Image m_pButtonEdge;

    public AUI m_pOwner;
    public int m_iX;
    public int m_iY;

    public Text[] m_pNumbers;

    public List<CLines> m_pLines;
    public List<CLines> m_pStartLines;

    public Image m_pBomb;
    public Image m_pFlag;
    public Text m_pGhost;

    public ParticleSystem[] m_pParticles;

    private EGridState m_eOldState;
    private float m_fPlay = -1.0f;
    private EPressType m_ePressType;

    public static readonly Color[][] m_cCoverColors = new []
    {
        new []
        {
            new Color(255.0f / 255.0f, 195.0f / 255.0f, 65.0f / 255.0f), 
            new Color(253.0f / 255.0f, 164.0f / 255.0f, 30.0f / 255.0f), 
            new Color(255.0f / 255.0f, 228.0f / 255.0f, 30.0f / 255.0f)
        }, 
        new []
        {
            new Color(171.0f / 255.0f, 171.0f / 255.0f, 171.0f / 255.0f),
            new Color(208.0f / 255.0f, 208.0f / 255.0f, 208.0f / 255.0f),
            new Color(146.0f / 255.0f, 146.0f / 255.0f, 146.0f / 255.0f),
        }, 
        new []
        {
            new Color(30.0f/ 255.0f, 251.0f / 255.0f, 31.0f / 255.0f),
            new Color(30.0f/ 255.0f, 251.0f / 255.0f, 141.0f / 255.0f),
            new Color(114.0f/ 255.0f, 255.0f / 255.0f, 0.0f / 255.0f),
        }, 
        new []
        {
            new Color(255.0f / 255.0f, 113.0f / 255.0f, 145.0f / 255.0f),
            new Color(255.0f / 255.0f, 6.0f / 255.0f, 114.0f / 255.0f),
            new Color(255.0f / 255.0f, 89.0f / 255.0f, 69.0f / 255.0f),
        }, 
        new []
        {
            new Color(69.0f / 255.0f, 224.0f / 255.0f, 255.0f / 255.0f),
            new Color(139.0f / 255.0f, 192.0f / 255.0f, 255.0f / 255.0f),
            new Color(25.0f / 255.0f, 154.0f / 255.0f, 246.0f / 255.0f),
        }, 
        new []
        {
            new Color(69.0f / 255.0f, 224.0f / 255.0f, 255.0f / 255.0f),
            new Color(139.0f / 255.0f, 192.0f / 255.0f, 255.0f / 255.0f),
            new Color(25.0f / 255.0f, 154.0f / 255.0f, 246.0f / 255.0f),
        }, 
        new []
        {
            new Color(80.0f / 255.0f, 80.0f / 255.0f, 80.0f / 255.0f),
            new Color(60.0f / 255.0f, 60.0f / 255.0f, 60.0f / 255.0f),
            new Color(70.0f / 255.0f, 70.0f / 255.0f, 70.0f / 255.0f),
        }, 
        new [] {new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f), }, 
    };

    public Image[] m_pWalls;
    private readonly static Vector3 m_vHorizen = new Vector3(0.62f, 0.45f, 0.0f);
    private readonly static Vector3 m_vVertical = new Vector3(0.62f, -0.45f, 0.0f);
    public void PutToGridWithWH(int iW, int iH)
    {
        float fScale;
        switch (iW)
        {
            case 1:
            case 2:
            case 3:
            case 4:
                fScale = 2.0f;
                break;
            case 5:
            case 6:
                fScale = 1.5f;
                break;
            case 7:
            case 8:
            case 9:
                fScale = 1.0f;
                break;
            default:
                fScale = 0.7f;
                break;
        }
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            //rectTransform.sizeDelta = new Vector2(60.0f, 60.0f);
            float fX = (m_iX - iW/2.0f + 0.5f)*75.0f*fScale;// + Random.Range(-5.0f, 5.0f) * fScale;
            float fY = (m_iY - iH/2.0f + 0.5f)*75.0f*fScale;// + Random.Range(-5.0f, 5.0f) * fScale;

            rectTransform.localPosition = fX * m_vHorizen + fY * m_vVertical;
            rectTransform.localScale = new Vector3(fScale, fScale, fScale);
        }
        gameObject.name = "Grid_" + m_iX + "_" + m_iY;
    }

    public void Update()
    {
        if (m_fPlay > 0.0f)
        {
            m_fPlay -= Time.deltaTime;
            if (m_fPlay <= 0.0f)
            {
                m_fPlay = -1.0f;
                OnOpen();
            }
        }
    }

    public void SetOwner(AUI pOwner)
    {
        m_pOwner = pOwner;
    }

    /*
    public void ShowWall(int iDir)
    {
        if (-1 == iDir)
        {
            for (int i = 0; i < (int)EDir.Max; ++i)
            {
                m_pWalls[i].enabled = false;
            }
        }
        else
        {
            m_pWalls[iDir].enabled = true;
        }

    }
     *          */

    public void SetNumber(string[] numbers)
    {
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            m_pNumbers[i].text = numbers[i];
        }
    }

    private EGridState m_eState;

    public EGridState GetState()
    {
        return m_eState;
    }

    public void PlayEffect(EGridEffect eEffect)
    {
        m_pParticles[(int)eEffect].Play();
    }

    public void SetGridState(EGridState eState, EPressType ePress, float fPlay = -1.0f)
    {
        m_eOldState = m_eState;
        m_eState = eState;
        if (EGridState.EGS_Empty == eState)
        {
            for (int i = 0; i < (int)EDir.Max; ++i)
            {
                m_pWalls[i].enabled = false;
            }
            for (int i = 0; i < (int)EDir.Max; ++i)
            {
                m_pNumbers[i].text = "";
            }
            m_pButtonIcon.enabled = false;
            m_pButton.enabled = false;
            m_pButtonEdge.enabled = false;
            m_pBomb.enabled = false;
            m_pFlag.enabled = false;
            m_pGhost.enabled = false;
            return;
        }

        if (EGridState.EGS_OpenNoBomb == m_eState
         || EGridState.EGS_OpenGhost == m_eState)
        {
            if (fPlay > 0.0f)
            {
                m_ePressType = ePress;
                m_fPlay = fPlay;
                return;
            }
        }

        m_pButtonEdge.enabled = true;
        m_pButtonIcon.color = m_cCoverColors[(int)eState][Random.Range(0, m_cCoverColors[(int)eState].Length)];
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            m_pWalls[i].enabled = false;
        }
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            m_pNumbers[i].enabled = (EGridState.EGS_OpenNoBomb == m_eState);
        }

        m_pBomb.enabled = EGridState.EGS_OpenBomb == m_eState;
        m_pFlag.enabled = EGridState.EGS_Tag == m_eState;
        m_pGhost.enabled = EGridState.EGS_OpenGhost == m_eState;
    }

    private void OnOpen()
    {
        m_pButtonEdge.enabled = true;
        m_pButtonIcon.color = m_cCoverColors[(int)m_eState][Random.Range(0, m_cCoverColors[(int)m_eState].Length)];
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            m_pWalls[i].enabled = false;
        }
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            m_pNumbers[i].enabled = (EGridState.EGS_OpenNoBomb == m_eState);
        }

        m_pBomb.enabled = EGridState.EGS_OpenBomb == m_eState;
        m_pFlag.enabled = EGridState.EGS_Tag == m_eState;
        m_pGhost.enabled = EGridState.EGS_OpenGhost == m_eState;

        switch (m_ePressType)
        {
            case EPressType.EPT_Max:
                if (EGridState.EGS_CloseFrozen == m_eOldState)
                {
                    PlayEffect(EGridEffect.EGE_FrozenLink);
                }
                else
                {
                    PlayEffect(EGridEffect.EGE_DigLink);
                }
                break;
            case EPressType.EPT_GoldenDig: 
                PlayEffect(EGridEffect.EGE_DigGolden);
                break;
            case EPressType.EPT_Mouse:
                PlayEffect(EGridEffect.EGE_MouseLink);
                break;
            case EPressType.EPT_Dig:
                PlayEffect(EGridEffect.EGE_Dig);
                break;
        }
    }

    public void SetEdge(EPressType ept)
    {
        switch (ept)
        {
            case EPressType.EPT_Dig:
                if (m_eState == EGridState.EGS_Close || m_eState == EGridState.EGS_CloseSafe)
                {
                    m_pButtonEdge.color = new Color(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    m_pButtonEdge.color = new Color(0.5f, 0.5f, 0.5f);
                }
                break;
            case EPressType.EPT_Tag:
                if (m_eState == EGridState.EGS_Close || m_eState == EGridState.EGS_CloseFrozen || m_eState == EGridState.EGS_Tag)
                {
                    m_pButtonEdge.color = new Color(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    m_pButtonEdge.color = new Color(0.5f, 0.5f, 0.5f);
                }
                break;
            case EPressType.EPT_GoldenDig:
                if (m_eState == EGridState.EGS_Close || m_eState == EGridState.EGS_CloseSafe || m_eState == EGridState.EGS_CloseFrozen)
                {
                    m_pButtonEdge.color = new Color(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    m_pButtonEdge.color = new Color(0.5f, 0.5f, 0.5f);
                }
                break;
            case EPressType.EPT_Mouse:
                if (m_eState == EGridState.EGS_OpenNoBomb || m_eState == EGridState.EGS_OpenGhost)
                {
                    m_pButtonEdge.color = new Color(1.0f, 1.0f, 1.0f);
                }
                else
                {
                    m_pButtonEdge.color = new Color(0.5f, 0.5f, 0.5f);
                }
                break;
        }
    }
}

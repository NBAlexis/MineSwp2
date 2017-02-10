using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum EGridState
{
    EGS_Close,
    EGS_Tag,
    EGS_OpenNoBomb,
    EGS_OpenBomb,
    EGS_Empty,
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
    public static readonly Color[] m_cCoverColors = new []
    {
        new Color(255.0f / 255.0f, 195.0f / 255.0f, 65.0f / 255.0f), 
        new Color(248.0f / 255.0f, 103.0f / 255.0f, 86.0f / 255.0f), 
        new Color(53.0f/ 255.0f, 215.0f / 255.0f, 255.0f / 255.0f), 
        new Color(80.0f / 255.0f, 80.0f / 255.0f, 80.0f / 255.0f), 
    };

    public Image[] m_pWalls;
    private readonly static Vector3 m_vHorizen = new Vector3(0.8f, 0.45f, 0.0f);
    private readonly static Vector3 m_vVertical = new Vector3(0.8f, -0.45f, 0.0f);
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

    public EGridState m_eState;
    public void SetGridState(EGridState eState)
    {
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
            return;
        }

        m_pButtonEdge.enabled = true;
        m_pButtonIcon.color = m_cCoverColors[(int) eState];
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            m_pWalls[i].enabled = false;
        }
        for (int i = 0; i < (int)EDir.Max; ++i)
        {
            m_pNumbers[i].text = "";
        }

        m_pButton.interactable = (EGridState.EGS_Close == eState);
    }
}

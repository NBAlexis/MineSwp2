using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum EGridState
{
    EGS_Close,
    EGS_Tag,
    EGS_OpenNoBomb,
    EGS_OpenBomb,
}

public class AGridToggle : MonoBehaviour
{
    public Button m_pButton;
    public Image m_pButtonIcon;

    public AUI m_pOwner;
    public int m_iX;
    public int m_iY;

    public Text[] m_pNumbers;
    public Sprite[] m_pCoverImages;
    public Image[] m_pWalls;

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
                fScale = 0.8f;
                break;
        }
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            //rectTransform.sizeDelta = new Vector2(60.0f, 60.0f);
            rectTransform.localPosition = new Vector3(
                (m_iX - iW / 2.0f + 0.5f) * 60.0f * fScale,
                (m_iY - iH / 2.0f + 0.5f) * 60.0f * fScale,
                0.0f);
            rectTransform.localScale = new Vector3(fScale, fScale, fScale);
        }
    }

    public void SetOwner(AUI pOwner)
    {
        m_pOwner = pOwner;
    }

    public void ShowWall(int iDir)
    {
        if (-1 == iDir)
        {
            for (int i = 0; i < 4; ++i)
            {
                m_pWalls[i].enabled = false;
            }
        }
        else
        {
            m_pWalls[iDir].enabled = true;
        }
    }

    public void SetNumber(string[] numbers)
    {
        for (int i = 0; i < 8; ++i)
        {
            m_pNumbers[i].text = numbers[i];
        }
    }

    public EGridState m_eState;
    public void SetGridState(EGridState eState)
    {
        m_eState = eState;
        m_pButtonIcon.sprite = m_pCoverImages[(int) eState];
        for (int i = 0; i < 4; ++i)
        {
            m_pWalls[i].enabled = false;
        }
        for (int i = 0; i < 8; ++i)
        {
            m_pNumbers[i].text = "";
        }

        m_pButton.interactable = (EGridState.EGS_Close == eState);
    }

    public void OnClicked()
    {
        m_pOwner.OnPressMine(m_iX, m_iY);
    }
}

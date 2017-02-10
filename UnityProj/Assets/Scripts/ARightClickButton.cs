using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ARightClickButton : MonoBehaviour, IPointerClickHandler
{
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            AGridToggle bt = GetComponent<AGridToggle>();
            bt.m_pOwner.OnPressMine(bt.m_iX, bt.m_iY, EPressType.EPT_Dig);
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            //Debug.Log("Middle click");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Debug.Log("Right click");
            AGridToggle bt = GetComponent<AGridToggle>();
            bt.m_pOwner.OnPressMine(bt.m_iX, bt.m_iY, EPressType.EPT_Tag);
        }
    }
}

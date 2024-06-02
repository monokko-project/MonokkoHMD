using UnityEngine;
using UnityEngine.EventSystems;


public class ChangeCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.SetCursor(MyAppSetting.CursorType.OnPointing);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.SetCursor(MyAppSetting.CursorType.Default);
    }

}

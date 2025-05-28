using UnityEngine;
using UnityEngine.EventSystems;

public class PetUIDragAndClick : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

   
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("🔧 开始拖动宠物");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // 拖动 Image
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}


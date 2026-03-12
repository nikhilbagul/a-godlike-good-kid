using UnityEngine;
using UnityEngine.EventSystems;

public class UIImageDropSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        // This runs when you release the drag over THIS object
        if (eventData.pointerDrag != null)
        {
            Debug.Log("Dropped " + eventData.pointerDrag.name + " onto " + gameObject.name);
            // Snap the dragged item to this slot's position
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        }
    }
}
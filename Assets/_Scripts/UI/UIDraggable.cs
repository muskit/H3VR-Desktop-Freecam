using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// For use only with the Options panel for now
public class UIDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public List<GameObject> grabElements;

    private bool dragging;
    private Vector2 offset;

    public void Update()
    {
        if (dragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - offset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject hitObj = eventData.pointerCurrentRaycast.gameObject;
        if (eventData.button == PointerEventData.InputButton.Left && grabElements.Contains(hitObj))
        {
            dragging = true;
            offset = eventData.position - new Vector2(transform.position.x, transform.position.y);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDraggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public List<GameObject> grabElements;
    public bool lockX = false;
    public bool lockY = false;

    private bool dragging;
    private Vector2 offset;

    public void Update()
    {
        if (dragging)
        {
            float newX = lockX ? transform.position.x : Input.mousePosition.x - offset.x;
            float newY = lockY ? transform.position.y : Input.mousePosition.y - offset.y;
            transform.position = new Vector2(newX, newY);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject hitObj = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log(hitObj);
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

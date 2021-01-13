using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool Down { get; private set; }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag " + eventData.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Down = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Down = false;
    }
}

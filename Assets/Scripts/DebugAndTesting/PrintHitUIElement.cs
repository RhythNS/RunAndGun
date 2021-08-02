using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrintHitUIElement : MonoBehaviour
{
    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
            {

                Debug.Log("Mouse hit: " + result.gameObject.name);
            }
        }
    }
}

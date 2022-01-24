using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 dragPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.gameObject.GetComponentInChildren<RectTransform>(), eventData.position,null, out Vector2 localPoint);
            dragPosition = localPoint;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            this.gameObject.GetComponentInChildren<RectTransform>().anchoredPosition = eventData.position - dragPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 snapPosition = GetNearestSnapPosition(eventData.position - dragPosition);
        this.gameObject.GetComponentInChildren<RectTransform>().anchoredPosition = snapPosition;
        dragPosition = new Vector2(0f, 0f);
    }

    private Vector2 GetNearestSnapPosition(Vector2 currentPosition)
    {
        Vector2 topLeftPosition = new Vector2(40f, (Screen.height - 280));
        Vector2 topRightPosition = new Vector2((Screen.width - 400f), (Screen.height - 280));
        Vector2 bottomRightPosition = new Vector2((Screen.width - 400f), 40f);
        Vector2 bottomLeftPosition = new Vector2(40f, 40f);

        Vector2[] possibleSnapPositions = { bottomLeftPosition, topLeftPosition, topRightPosition, bottomRightPosition };

        Vector2 snapPosition = bottomLeftPosition;

        for(int i = 0; i<4; i++)
        {
            float currentSnapDistance = (currentPosition - snapPosition).magnitude;
            float potentialSnapDistance = (currentPosition - possibleSnapPositions[i]).magnitude;

            if (potentialSnapDistance < currentSnapDistance)
            {
                snapPosition = possibleSnapPositions[i];
            }
        }

        return snapPosition;
    }
}

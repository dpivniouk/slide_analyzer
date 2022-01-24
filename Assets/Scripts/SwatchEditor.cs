using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SwatchEditor : MonoBehaviour,IPointerClickHandler
{
    private bool isSwatchColorSet;
    private Color32 swatchColor;

    void Start()
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSwatchColorSet && ApplicationController.instance.activeSlideImage!=null)
            {
                SetCurrentColor();
            }
            else if(ApplicationController.instance.activeSlideImage!=null)
            {
                SetSwatchColor();
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if (isSwatchColorSet)
            {
                ClearSwatchColor();
            }
        }

    }

    public void SetCurrentColor()
    {
        ApplicationController.instance.SetCurrentColor(swatchColor);
        ApplicationController.instance.activeImageOperations.HighlightToleranceArea();
    }

    public void SetSwatchColor()
    {
        swatchColor = ApplicationController.instance.currentColor;
        this.gameObject.GetComponent<Image>().color = swatchColor;
        isSwatchColorSet = true;
    }

    public void ClearSwatchColor()
    {
        swatchColor = Color.white;
        this.gameObject.GetComponent<Image>().color = swatchColor;
        isSwatchColorSet = false;
    }

}

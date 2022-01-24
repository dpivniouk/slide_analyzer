using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorState : BaseState
{
    public override void OnStateEnter(ApplicationController appController)
    {
        foreach (Button button in appController.allSideMenuButtons)
        {
            button.interactable = true;
        }
        appController.cursorButton.interactable = false;

        if (appController.activeImageOperations == null)
        {
            return;
        }
    }

    public override void Update(ApplicationController appController)
    {
        if(appController.activeImageOperations == null)
        {
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Equals))
        {
            appController.activeImageOperations.ZoomImageIn();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Minus))
        {
            appController.activeImageOperations.ZoomImageOut();
        }
        else if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Alpha0))
        {
            appController.activeImageOperations.ResetImage();
        }

        if (!appController.isColorSelected && appController.activeImageOperations.isMouseOver && !appController.activeImageOperations.isDragging)
        {
            appController.activeImageOperations.ShowSelectedColors();
        }

        if(Input.GetMouseButtonDown(0) && appController.activeImageOperations.isMouseOver && !appController.activeImageOperations.isDragging)
        {
            appController.isColorSelected = true;
            appController.activeImageOperations.ShowSelectedColors();
            appController.activeImageOperations.HighlightToleranceArea();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (appController.activeImageOperations.lassoPoints.Count > 0)
            {
                appController.isAreaSelected = false;
                appController.activeImageOperations.DeselectArea();
                appController.activeImageOperations.lassoRenderer.positionCount++;

            }
            else if (appController.isColorSelected)
            {
                appController.isColorSelected = false;
                appController.activeImageOperations.DeselectPixels();
            }
            else
            {
                appController.quitPanel.SetActive(true);
            }
        }

    }
}

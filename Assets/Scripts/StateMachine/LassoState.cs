using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LassoState : BaseState
{
    public override void OnStateEnter(ApplicationController appController)
    {
        foreach (Button button in appController.allSideMenuButtons)
        {
            button.interactable = true;
        }
        appController.lassoButton.interactable = false;

        if (appController.activeImageOperations == null)
        {
            return;
        }
        appController.activeImageOperations.DeselectArea();
        appController.activeImageOperations.lassoRenderer.positionCount++;
    }

    public override void Update(ApplicationController appController)
    {
        if (appController.activeImageOperations == null)
        {
            return;
        }

        LineRenderer lassoRenderer = appController.activeImageOperations.lassoRenderer;

        if (!lassoRenderer.loop)
        {
            Vector3 drawPosition = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, -5f);
            lassoRenderer.SetPosition(lassoRenderer.positionCount - 1, drawPosition);

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

        if (Input.GetMouseButtonDown(0) && appController.activeImageOperations.isMouseOver)
        {
            if (!appController.activeImageOperations.lassoRenderer.loop)
            {
                appController.activeImageOperations.SaveLassoPoint();
            }
            else
            {
                appController.isAreaSelected = false;
                appController.activeImageOperations.DeselectArea();
                appController.activeImageOperations.lassoRenderer.positionCount++;
                appController.activeImageOperations.SaveLassoPoint();
            }
        }


        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && appController.activeImageOperations.lassoPoints.Count>0 && !Camera.main.GetComponentInChildren<DrawSelectionLines>().completeSelection)
        {
            appController.activeImageOperations.FinishSelectingArea();
        }

    }
}

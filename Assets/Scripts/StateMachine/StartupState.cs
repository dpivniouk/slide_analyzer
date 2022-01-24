using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartupState : BaseState
{
    public override void OnStateEnter(ApplicationController appController)
    {
        //set default values
        Application.targetFrameRate = 30;
        appController.toleranceSetting = 32;
        appController.toleranceInput.placeholder.GetComponent<TextMeshProUGUI>().text = appController.toleranceSetting.ToString();
        appController.SetCurrentColor(Color.white);
        appController.TransitionToState(appController.cursorState);
    }

    public override void Update(ApplicationController appController)
    {
    }
}

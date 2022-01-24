using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonExtensions : MonoBehaviour
{
    public void DisableButton(Button button)
    {
        button.interactable = false;
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = new Color32(102, 98, 98, 255);

    }

    public void EnableButton(Button button)
    {
        button.interactable = true;
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.color = new Color32(53, 53, 53, 255);

    }
}

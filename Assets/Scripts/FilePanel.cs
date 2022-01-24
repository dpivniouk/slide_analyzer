using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FilePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button imageSaveButton;
    public Button dataSaveButton;
    public bool isMouseOver = false;

    void OnEnable()
    {
        if (ApplicationController.instance.activeSlideImage != null)
        {
            imageSaveButton.gameObject.GetComponentInChildren<ButtonExtensions>().EnableButton(imageSaveButton);
            dataSaveButton.gameObject.GetComponentInChildren<ButtonExtensions>().EnableButton(dataSaveButton);
        }
        else
        {
            imageSaveButton.gameObject.GetComponentInChildren<ButtonExtensions>().DisableButton(imageSaveButton);
            dataSaveButton.gameObject.GetComponentInChildren<ButtonExtensions>().DisableButton(dataSaveButton);
        }
    }

    void Update()
    {
        if ((Input.anyKeyDown && !isMouseOver) || Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
}

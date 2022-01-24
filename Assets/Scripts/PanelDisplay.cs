using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelDisplay : MonoBehaviour
{
    public GameObject controlPanel;
    public Button showButton;

    private Animator controlPanelAnimator;

    private void Awake()
    {
        controlPanelAnimator = controlPanel.GetComponentInChildren<Animator>();
    }

    public void HideControlPanel()
    {
        StartCoroutine(SlidePanelOut());
    }

    public void ShowControlPanel()
    {
        StartCoroutine(SlidePanelIn());
    }


    public IEnumerator SlidePanelOut()
    {
        controlPanelAnimator.SetBool("isHidden", true);
        yield return new WaitForSeconds(.5f);
        showButton.gameObject.SetActive(true);
    }

    public IEnumerator SlidePanelIn()
    {
        showButton.gameObject.SetActive(false);
        controlPanelAnimator.SetBool("isHidden", false);
        yield return new WaitForSeconds(.5f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSidebar : MonoBehaviour
{
    public GameObject sideMenuPanel;

    private Animator sidebarAnimator;

    void Start()
    {
        sidebarAnimator = sideMenuPanel.GetComponentInChildren<Animator>();
    }

    public void ToggleSidebar()
    {
        bool isSidebarOpen = sidebarAnimator.GetBool("isSidebarOpen");
        sidebarAnimator.SetBool("isSidebarOpen", !isSidebarOpen);
    }
}

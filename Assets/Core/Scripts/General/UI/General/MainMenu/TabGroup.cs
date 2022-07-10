using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    private List<TabButton> tabButtons;
    private TabButton selectedTab;
    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabHover;
    [SerializeField] private Sprite tabActive;
    [SerializeField] private List<GameObject> tabPages;

    private void OnEnable()
    {
       InitialEnable();
    }

    public void InitialEnable()
    {
        for (int i = 1; i < tabPages.Count; i++)
        {
            tabPages[i].SetActive(false);           
        }
        tabPages[0].SetActive(true);
        OnTabSelected(tabButtons[1]);
    }

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (selectedTab == null || button != selectedTab)
        {
            button.ChangeTabImage(tabHover);
        }       
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }
        selectedTab = button;
        selectedTab.Select();
        ResetTabs();
        button.ChangeTabImage(tabActive);
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < tabPages.Count; i++)
        {
            if (i == index)
            {
                tabPages[i].SetActive(true);
            }
            else
            {
                tabPages[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (var button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.ChangeTabImage(tabIdle);
        }
    }
}

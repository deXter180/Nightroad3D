using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : Singleton<TooltipSystem>
{
    #region Variables

    private Tooltip tooltip;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        tooltip = GetComponentInChildren<Tooltip>();
        tooltip.gameObject.SetActive(false);
    }

    public static void Show(string content1 = "", string content2 = "", string content3 = "", string header = "")
    {
        Instance.tooltip.SetText(content1, content2, content3, header);
        Instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        Instance.tooltip.gameObject.SetActive(false);
    }

    #endregion
}

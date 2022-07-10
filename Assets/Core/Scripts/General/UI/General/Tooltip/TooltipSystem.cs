using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : Singleton<TooltipSystem>
{
    private Tooltip tooltip;

    protected override void Awake()
    {
        base.Awake();
        tooltip = GetComponentInChildren<Tooltip>();
        tooltip.gameObject.SetActive(false);
    }

    public static void Show(string content, string header = "")
    {
        Instance.tooltip.SetText(content, header);
        Instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        Instance.tooltip.gameObject.SetActive(false);
    }
}
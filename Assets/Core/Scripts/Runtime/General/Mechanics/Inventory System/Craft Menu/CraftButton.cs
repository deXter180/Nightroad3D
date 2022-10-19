using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CraftButton : HighlightedButton
{
    #region Variables

    private CraftMenuControl craftMenu;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        craftMenu = GetComponentInParent<CraftMenuControl>();
    }

    protected override void OnClickMethod()
    {
        craftMenu.Craft();

        //StartCoroutine(Delay());

        //IEnumerator Delay()
        //{
        //    yield return Helpers.GetWait(0.5f);
        //    buttonText.color = Color.white;
        //}
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RecipeCloseButton : HighlightedButton
{
    #region Variables

    public event Action OnRecipeDecsClosed;

    #endregion

    #region General

    protected override void Awake()
    {
        base.Awake();
        isDelayNeeded = false;
    }

    protected override void OnClickMethod()
    {
        OnRecipeDecsClosed?.Invoke();
    }

    #endregion
}

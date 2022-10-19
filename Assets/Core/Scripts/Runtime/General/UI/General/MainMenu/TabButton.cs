using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    #region Variables

    private TabGroup tabGroup;
    private Image image;
    public static event Action<TabButton> OnTabSelected;
    public static event Action<TabButton> OnTabDeselected;

    #endregion

    #region General

    private void Awake()
    {
        tabGroup = GetComponentInParent<TabGroup>();
        image = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void ChangeTabImage(Sprite tabSprite)
    {
        image.sprite = tabSprite;
    }

    public void Select()
    {
        OnTabSelected?.Invoke(this);
    }

    public void Deselect()
    {
        OnTabDeselected?.Invoke(this);
    }

    #endregion

    //~~~~~~~~~~~~~~~~~~~~~~ Callbacks ~~~~~~~~~~~~~~~~~~~~~~~~~

    #region Callbacks

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    #endregion
}

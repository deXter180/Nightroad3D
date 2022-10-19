using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StashClose : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Variables

    private Image image;
    private Color defaultColor;
    private GameController gameController;

    #endregion

    #region General

    private void Awake()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    private void OnEnable()
    {
        if (gameController == null)
            gameController = GameController.Instance;
        image.color = defaultColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameController.IsStashActive)
        {
            gameController.CloseStash();
        }
        else if (gameController.IsCraftingActive)
        {
            gameController.CloseCrafting();
        }
    }

    #endregion
}

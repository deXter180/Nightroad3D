using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StashClose : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image image;
    private Color defaultColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
    }

    private void OnEnable()
    {
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
}

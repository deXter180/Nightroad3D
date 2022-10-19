using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class HighlightedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region Variables

    protected TextMeshProUGUI buttonText;
    protected Image buttonImage;
    protected Color imageDefaultColor;
    protected Color textDefaultColor;
    protected bool isDelayNeeded;

    #endregion

    #region General

    protected virtual void Awake()
    {
        isDelayNeeded = true;
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        imageDefaultColor = buttonImage.color;
        textDefaultColor = buttonText.color;
    }

    private void OnEnable()
    {
        buttonText.color = textDefaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonText.color = Color.red;
        OnClickMethod();
        if (isDelayNeeded)
        {
            StartCoroutine(Delay());
        }
        IEnumerator Delay()
        {
            yield return Helpers.GetWait(0.5f);
            buttonText.color = Color.white;
        }
    }

    protected virtual void OnClickMethod()
    {
        
    }   

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = imageDefaultColor;
    }

    #endregion
}

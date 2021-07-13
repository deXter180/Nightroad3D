using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipCanvas : MonoBehaviour {

    public static TooltipCanvas Instance { get; private set; }
    private Input input;

    [SerializeField]
    private RectTransform canvasRectTransform = null;

    private TextMeshProUGUI textMeshPro;
    private RectTransform backgroundRectTransform;
    private Func<string> getTooltipStringFunc;

    private void Awake() {
        Instance = this;
        backgroundRectTransform = transform.Find("background").GetComponent<RectTransform>();
        textMeshPro = transform.Find("text").GetComponent<TextMeshProUGUI>();
        input = FindObjectOfType<InputControl>();
        if (canvasRectTransform == null) 
        {
            Debug.LogError("Need to set Canvas Rect Transform!");
        }

        HideTooltip();
    }

    private void Update() {
        SetText(getTooltipStringFunc());

        Vector2 anchoredPosition = input.GetMousePos() / canvasRectTransform.localScale.x;
        if (anchoredPosition.x + backgroundRectTransform.rect.width > canvasRectTransform.rect.width) {
            anchoredPosition.x = canvasRectTransform.rect.width - backgroundRectTransform.rect.width;
        }
        if (anchoredPosition.y + backgroundRectTransform.rect.height > canvasRectTransform.rect.height) {
            anchoredPosition.y = canvasRectTransform.rect.height - backgroundRectTransform.rect.height;
        }
        transform.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
    }

    private void ShowTooltip(string tooltipString) {
        ShowTooltip(() => tooltipString);
    }

    private void ShowTooltip(Func<string> getTooltipStringFunc) {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
        this.getTooltipStringFunc = getTooltipStringFunc;
        Update();
    }

    private void SetText(string tooltipString) {
        textMeshPro.SetText(tooltipString);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(10f, 6f);
        backgroundRectTransform.sizeDelta = textSize + padding;
    }

    private void HideTooltip() {
        gameObject.SetActive(false);
    }

    public static void ShowTooltip_Static(string tooltipString) {
        Instance.ShowTooltip(tooltipString);
    }

    public static void ShowTooltip_Static(Func<string> getTooltipStringFunc) {
        Instance.ShowTooltip(getTooltipStringFunc);
    }

    public static void HideTooltip_Static() {
        Instance.HideTooltip();
    }
}

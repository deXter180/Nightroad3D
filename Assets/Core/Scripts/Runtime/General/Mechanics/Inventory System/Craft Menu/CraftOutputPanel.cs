using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftOutputPanel : MonoBehaviour
{
    #region Variables

    [SerializeField] private Image outputItemImage;
    private RectTransform rectTransform;
    private Vector3 defaultScale = new Vector3(0.1f, 0.1f, 1);
    private Vector3 newScale = new Vector3(0.5f, 0.5f, 1);

    #endregion

    #region General

    private void OnEnable()
    {
        transform.localScale = defaultScale;
    }

    public void SetOutputItemImage(Sprite outputImage)
    {        
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }            
        outputItemImage.sprite = outputImage;
        if (!LeanTween.isTweening(gameObject))
        {
            LeanTween.scale(rectTransform, newScale, 0.5f);
            var temp = LeanTween.moveY(rectTransform, 500f, 0.5f);
            temp.setOnComplete(() => { gameObject.SetActive(false); });
        }
    }

    #endregion
}

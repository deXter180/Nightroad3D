using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ReturnToMenuButton : MonoBehaviour
{  
    [SerializeField] private Sprite tabHoverImage;
    private RectTransform rectTransform;
    private Image image;
    private Sprite tabIdleImage;
    private Color defaultColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        tabIdleImage = image.sprite;
        defaultColor = image.color;
    }

    public void AffectReturnButton(Vector2 mousePos, bool isTriggered)
    {
        if (gameObject.activeSelf)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos))
            {
                image.sprite = tabHoverImage;
                image.color = Color.white;
                if (isTriggered)
                {
                    HeadUpDisplayHandler.Instance.AfterReturningToMainScene();
                    SceneLoader.LoadMainScene();
                }
            }
            else
            {
                image.sprite = tabIdleImage;
                image.color = defaultColor;
            }
        }
    }

}

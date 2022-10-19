using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    #region SerializedVariables

    [SerializeField] private int characterWrapLimit;
    [SerializeField] private TextMeshProUGUI headerField;
    [SerializeField] private TextMeshProUGUI contentField1;
    [SerializeField] private TextMeshProUGUI contentField2;
    [SerializeField] private TextMeshProUGUI contentField3;
    [SerializeField] private LayoutElement layoutElement;

    #endregion

    #region Variables

    private PlayerInputAsset inputs;
    private RectTransform rectTransform;

    #endregion

    #region General

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        StartCoroutine(InputDone());
    }

    private void Update()
    {
        if (inputs != null)
        {
            Vector2 position = inputs.BasicControls.MousePosition.ReadValue<Vector2>();
            float pivotX = position.x / Screen.width;
            float pivotY = position.y / Screen.height;
            rectTransform.pivot = new Vector2(pivotX, pivotY);
            transform.position = position;
        }
    }

    private IEnumerator InputDone()
    {
        if (inputs == null)
        {
            yield return new WaitUntil(() => InputManager.InputReady);
            inputs = InputManager.InputActions;
        }
    }

    #endregion

    #region Mechanics

    public void SetText(string content1 = "", string content2 = "", string content3 = "", string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField1.gameObject.SetActive(false);
        contentField2.gameObject.SetActive(false);
        contentField3.gameObject.SetActive(false);
        if (string.IsNullOrEmpty(content1))
        {
            if (string.IsNullOrEmpty(content2))
            {
                if (!string.IsNullOrEmpty(content3))
                {
                    contentField1.gameObject.SetActive(true);
                    contentField1.text = content3;
                }
            }
            else
            {
                contentField1.gameObject.SetActive(true);
                contentField1.text = content2;
                if (!string.IsNullOrEmpty(content3))
                {
                    contentField2.gameObject.SetActive(true);
                    contentField2.text = content3;
                }

            }
        }
        else
        {
            contentField1.gameObject.SetActive(true);
            contentField1.text = content1;
            if (string.IsNullOrEmpty(content2))
            {
                if (!string.IsNullOrEmpty(content3))
                {
                    contentField2.gameObject.SetActive(true);
                    contentField2.text = content3;
                }
            }
            else
            {
                contentField2.gameObject.SetActive(true);
                contentField2.text = content2;
                if (!string.IsNullOrEmpty(content3))
                {
                    contentField3.gameObject.SetActive(true);
                    contentField3.text = content3;
                }

            }
        }
        int headerLength = headerField.text.Length;
        int content1Length = contentField1.text.Length;
        int content2Length = contentField2.text.Length;
        int content3Length = contentField3.text.Length;
        layoutElement.enabled = (headerLength > characterWrapLimit || content1Length > characterWrapLimit || content2Length > characterWrapLimit || content2Length > characterWrapLimit) ? true : false;
    }

    #endregion
}

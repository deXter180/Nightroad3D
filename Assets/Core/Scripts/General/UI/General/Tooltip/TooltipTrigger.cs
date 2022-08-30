using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region SerializedVariables

    [SerializeField] [TextArea()] private string header;
    [SerializeField] [TextArea()] private string content;

    #endregion

    #region Properties

    public string Header { get => header; set => header = value; }
    public string Content { get => content; set => content = value; }

    #endregion

    #region Callbacks

    private IEnumerator OnDelay()
    {
        yield return Helpers.GetWait(0.5f);
        TooltipSystem.Show(content, header);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(OnDelay());       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }

    #endregion
}

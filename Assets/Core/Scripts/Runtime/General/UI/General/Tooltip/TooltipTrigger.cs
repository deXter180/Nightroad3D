using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region Variables

    [SerializeField] [TextArea()] private string header;
    [SerializeField] [TextArea()] private string content1;
    private string content2;
    private string content3;
    private bool isStop = false;


    #endregion

    #region Properties

    public string Header { get => header; set => header = value; }
    public string Content1 { get => content1; set => content1 = value; }
    public string Content2 { get => content2; set => content2 = value; }
    public string Content3 { get => content3; set => content3 = value; }

    #endregion

    #region General

    public void SetStop(bool stop)
    {
        isStop = stop;
    }

    private IEnumerator OnDelay()
    {
        yield return Helpers.GetWait(0.5f);
        TooltipSystem.Show(content1, content2, content3, header);
    }

    public void UpdateTooltip()
    {
        TooltipSystem.Show(content1, content2, content3, header);
    }

    #endregion

    #region Callbacks

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isStop)
        {
            StartCoroutine(OnDelay());
        }            
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }

    #endregion
}

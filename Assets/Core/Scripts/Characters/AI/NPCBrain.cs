using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCBrain : MonoBehaviour
{
    #region SerializedVariables

    [SerializeField] private float speechOffsetX = 1f;
    [SerializeField] private float speechOffsetY = 10f;
    [SerializeField] private TextAsset storyText;
    [SerializeField][TextArea(2, 6)] private List<string> npcDialogueList;

    #endregion

    #region Variables

    private bool storySetupDone;
    private DialogueManager dialogueManager;
    public Vector3 SpeechBubblePos { get; private set; }

    #endregion

    #region General

    private void OnEnable()
    {
        storySetupDone = false;
        SpeechBubblePos = new Vector3(transform.position.x + speechOffsetX, transform.position.y + speechOffsetY, transform.position.z);
    }

    public void StartConversion()
    {
        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;
        if (!storySetupDone)
        {
            dialogueManager.SetupStory(storyText);
            storySetupDone = true;
        }
        else
        {
            dialogueManager.ContinueStory(storyText);
        }
        
    }

    public string GetDialogueText()
    {
        return npcDialogueList[Random.Range(0, npcDialogueList.Count - 1)];
    }

    #endregion
}

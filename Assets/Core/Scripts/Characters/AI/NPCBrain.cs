using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCBrain : MonoBehaviour
{
    [SerializeField] private TextAsset storyText;
    private bool storySetupDone;
    private DialogueManager dialogueManager;

    private void OnEnable()
    {
        storySetupDone = false;
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

}

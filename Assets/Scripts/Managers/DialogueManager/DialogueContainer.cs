using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueContainer : MonoBehaviour
{
    public bool triggerOnStart = false;
    public Text nameText;
    public Text dialogueText;
    public Dialogue dialogue;

    private bool speedUpTyping = false;
    private bool typingSpeedIncreased = false;

    public bool finishedTyping;


    private void Start()
    {
        DialogueManager.OnSentenceFinished += FinishedTyping;
        if (triggerOnStart)
        {
            TriggerDialogue();
        }
    }

    private void Update()
    {
        if (finishedTyping)
        {
            if (Input.GetKeyUp(KeyCode.G))
            {
                finishedTyping = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.G))
            {
                speedUpTyping = true;
            }
            else
            {
                speedUpTyping = false;
            }

            if (speedUpTyping)
            {
                if (!typingSpeedIncreased)
                {
                    DialogueManager.Instance.letterTypingSpeed /= 1.5f;
                    DialogueManager.Instance.symbolTypingSpeed /= 1.5f;
                    typingSpeedIncreased = true;
                }
            }
            else
            {
                if (typingSpeedIncreased)
                {
                    DialogueManager.Instance.letterTypingSpeed *= 1.5f;
                    DialogueManager.Instance.symbolTypingSpeed *= 1.5f;
                    typingSpeedIncreased = false;
                }
            }
        }

    }

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, nameText, dialogueText);
    }

    public void FinishedTyping()
    {
        finishedTyping = true;
    }
}


[System.Serializable]
public class Dialogue
{
    public string name;
    [TextArea(3, 10)]
    public string[] sentences;
}

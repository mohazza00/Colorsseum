using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : Singleton<DialogueManager>
{

    private Queue<string> sentences = new Queue<string>();
    public float letterTypingSpeed = 0.05f;
    public float symbolTypingSpeed = 0.15f;

    public delegate void EndSentence();
    public static event EndSentence OnSentenceFinished;

    private const string SpecialCharacters = ". ,!'";




    public void StartDialogue(Dialogue dialogue, Text nameText, Text dialogueText)
    {
        letterTypingSpeed = 0.05f;
        symbolTypingSpeed = 0.15f;

        if (nameText != null)
            nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence(dialogueText);
    }

    public void DisplayNextSentence(Text dialogueText)
    {
        if (sentences.Count == 0)
        {
            SentenceFinished();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence, dialogueText));
    }

    IEnumerator TypeSentence(string sentence, Text dialogueText)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {

            dialogueText.text += letter;
            if (IsSpecialCharacter(letter.ToString()))
            {
                yield return new WaitForSecondsRealtime(symbolTypingSpeed);
            }
            else
            {
                yield return new WaitForSecondsRealtime(letterTypingSpeed);
            }
        }
        if (dialogueText.text == sentence || sentences.Count == 0)
        {
            SentenceFinished();
        }
    }

    public void SentenceFinished()
    {
        OnSentenceFinished();
    }


    public static bool IsSpecialCharacter(string text)
    {
        return text.IndexOfAny(SpecialCharacters.ToCharArray()) >= 0;
    }
}

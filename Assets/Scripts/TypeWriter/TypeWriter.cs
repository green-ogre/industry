
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    public String text;
    public float speed;
    private int charIndex;
    private TMP_Text textUI;

    void Start()
    {
        textUI = GetComponent<TMP_Text>();
        ReadText("Hello world! This is my first ever string");
    }

    public void ReadText(String textToRead)
    {
        textUI.text = textToRead;
        text = textToRead;
        charIndex = 0;
        textUI.maxVisibleCharacters = 0;
        StartCoroutine(ScrollText());
    }

    private IEnumerator ScrollText()
    {
        for (int i = 0; i <= text.Length; i++)
        {
            textUI.maxVisibleCharacters = charIndex;
            charIndex++;
            yield return new WaitForSeconds(speed);
        }
    }
}

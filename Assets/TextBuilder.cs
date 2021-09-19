using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBuilder : MonoBehaviour
{
    private string textHolder;
    public float textDelay;
    public float textLingerTime;
    public bool lastText;
    public GameObject activeSprite;
    public Sprite[] sprites;

    void Start()
    {
        textHolder = GetComponent<Text>().text;
        activeSprite.SetActive(true);
        GetComponent<Text>().text = "";
        StartCoroutine("BringOnText");
    }

    bool awaitInput = false;
    bool finalInput = false;
    bool skipBlock = false;
    private void Update()
    {
        if (awaitInput && Input.GetKeyDown(KeyCode.Space))
        {
            awaitInput = false;
        }
        else if (finalInput && Input.GetKeyDown(KeyCode.Space))
        {
            GlobalGameController.control.EnableControl();
            transform.parent.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            skipBlock = true;
        }
    }
    IEnumerator BringOnText() 
    {
        for(int i = 0; i < textHolder.Length; i++)
        {
            
            if (textHolder[i] == '%')
            {
                if (i != 0)
                {
                    awaitInput = true;
                    while (awaitInput)
                    {
                        yield return null;
                    }
                }
                if (skipBlock)
                {
                    skipBlock = false;
                }

                GetComponent<Text>().text = "";
                i++;
                int spriteChar;
                int.TryParse(("" + textHolder[i] + textHolder[++i]),out spriteChar);
                activeSprite.GetComponent<Image>().sprite = sprites[spriteChar];
            }
            else
            {
                if(!skipBlock)
                    yield return new WaitForSeconds(textDelay);
                GetComponent<Text>().text += textHolder[i];
            }
        }
        activeSprite.SetActive(false);
        yield return new WaitForSeconds(textDelay);
        if (lastText)
        {
            GlobalGameController.control.LoadNextLevel();
        }
        else
            finalInput = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDisplays : MonoBehaviour
{
    public GameObject FirstDisplay;
    public GameObject LastDisplay;
    public void DisplayOpeningSceneText()
    {
        FirstDisplay.SetActive(true);
    }
    public void DisplayClosingSceneText()
    {    
       LastDisplay.SetActive(true);
    }
}

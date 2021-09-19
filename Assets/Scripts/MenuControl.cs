using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  

public class MenuControl : MonoBehaviour
{
    public GameObject creditsText;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            
        if(Input.GetKeyDown(KeyCode.C))
            creditsText.SetActive(!creditsText.activeSelf);
        
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}

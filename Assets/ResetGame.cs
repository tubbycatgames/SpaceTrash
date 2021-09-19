using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GlobalGameController.control.ResetStats();
        SceneManager.LoadScene(0);
    }   
}

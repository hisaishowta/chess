using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeScript : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        Screen.SetResolution(1024, 768,  false, 60);

    }

    public void OnClick()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            SceneManager.LoadScene("Scene/Room");
        }
        else if (SceneManager.GetActiveScene().name == "Room")
        {
            SceneManager.LoadScene("Scene/GameMain");
        }
        else if (SceneManager.GetActiveScene().name == "GameMain")
        {
            SceneManager.LoadScene("Scene/Result");
        }
        else if (SceneManager.GetActiveScene().name == "Result")
        {
            SceneManager.LoadScene("Scene/Title");
        }
        else
        {
            SceneManager.LoadScene("Scene/Title");
        }
    }
    public void OnClickExit()
    {
        Application.Quit();
    }
}
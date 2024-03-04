using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameStartMenu : MonoBehaviour
{
    private string filePath = "Assets/Resources/GameData.json";

    public void NewGame()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        SceneManager.LoadScene(1);
    }

    public void Continue()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

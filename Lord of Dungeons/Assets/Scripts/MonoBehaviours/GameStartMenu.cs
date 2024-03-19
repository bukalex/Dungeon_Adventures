using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameStartMenu : MonoBehaviour
{
    private string filePath = "Assets/Resources/GameData.json";
    [SerializeField] private List<Image> buttons;
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        foreach (Image button in buttons) button.alphaHitTestMinimumThreshold = 0.5f;
    }

    public void NewGame()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        if (toggle.isOn) SceneManager.LoadScene("Education");
        else SceneManager.LoadScene("HUB");
    }

    public void Continue()
    {
        SceneManager.LoadScene("HUB");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

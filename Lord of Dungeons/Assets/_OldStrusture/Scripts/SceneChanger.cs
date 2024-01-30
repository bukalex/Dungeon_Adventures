using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] GameObject[] moveObjects;
    [SerializeField] string sceneName;
    [SerializeField] Vector2 playerPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            StartCoroutine(LoadAsyncScene());
            foreach (GameObject obj in moveObjects) 
            {
                if (obj.tag == "Player")
                {
                    obj.transform.position = playerPos;
                }
            }
        }
    }

    IEnumerator LoadAsyncScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        foreach (GameObject obj in moveObjects)
        {
            SceneManager.MoveGameObjectToScene(obj, SceneManager.GetSceneByName(sceneName));
        }
        SceneManager.UnloadSceneAsync(currentScene);
    }
}                                                                           

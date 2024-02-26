using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private int SceneToChange;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SceneToChange = SceneManager.GetActiveScene().buildIndex + 1;
            Debug.Log(SceneToChange);
            CheckpointManager.Instance.ChangeLevel(SceneToChange);
            SceneManager.LoadScene(SceneToChange);
        }
    }
}

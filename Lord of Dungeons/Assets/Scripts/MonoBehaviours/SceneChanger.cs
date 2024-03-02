using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    private int SceneToChange;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            SceneToChange = SceneManager.GetActiveScene().buildIndex + 1;
            CheckpointManager.Instance.ChangeLevel(SceneToChange);
            SceneManager.LoadScene(SceneToChange);
        }
    }
}

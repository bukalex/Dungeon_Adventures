using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private int SceneToChange;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CheckpointManager.Instance.ChangeLevel(SceneToChange);
            SceneManager.LoadScene(SceneToChange);
        }
    }
}

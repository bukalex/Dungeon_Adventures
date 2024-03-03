using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    public Button ContinueButton;
    public Button CreditsButton;
    public Button CrossButton;
    public GameObject MenuUI;
    public GameObject CreditsUI;
    private int SceneToChange;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        CreditsButton.onClick.AddListener(() => Credits());
        CrossButton.onClick.AddListener(() => Credits());
        ContinueButton.onClick.AddListener(() => ContinueGame());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
            animator.SetTrigger("FadeOut");
    }
    public void LoadNextScene()
    {
        SceneToChange = SceneManager.GetActiveScene().buildIndex + 1;
        CheckpointManager.Instance.ChangeLevel(SceneToChange);
        SceneManager.LoadScene(SceneToChange);
    }
    public void NewGame()
    {

    }
    public void ContinueGame()
    {
        animator.SetTrigger("FadeOut");
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Credits()
    {
        MenuUI.SetActive(!MenuUI.activeSelf);
        CreditsUI.SetActive(!MenuUI.activeSelf);
    }


}

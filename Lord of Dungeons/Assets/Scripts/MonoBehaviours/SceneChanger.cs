using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour, IInteractable
{
    private int SceneToChange;

    [SerializeField]
    private GameObject interactIconPrefab;
    private GameObject interactIcon;

    private void Start()
    {
        interactIcon = Instantiate(interactIconPrefab, Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2), Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        interactIcon.transform.SetSiblingIndex(0);
        interactIcon.SetActive(false);
    }

    void Update()
    {
        if (interactIcon.activeSelf && Input.GetKeyDown(UIManager.Instance.keyCodes[15]))
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (obj.GetComponent<EnemyController>() != null && obj.GetComponent<EnemyController>().IsAlive()) return;
            }

            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PopUpUI"))
        {
            Destroy(obj);
        }

        SceneToChange = SceneManager.GetActiveScene().buildIndex + 1;
        CheckpointManager.Instance.ChangeLevel(SceneToChange);
        SceneManager.LoadScene(SceneToChange);
    }

    public void ShowButton()
    {
        interactIcon.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
        interactIcon.GetComponentInChildren<TMP_Text>().text = UIManager.Instance.textKeys[15].text;
        interactIcon.SetActive(true);
    }

    public void HideButton()
    {
        interactIcon.SetActive(false);
    }
}

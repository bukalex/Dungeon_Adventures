using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour, IInteractable
{
    public Animator anim;
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
        if (interactIcon != null && interactIcon.activeSelf && Input.GetKeyDown(UIManager.Instance.keyCodes[15]))
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                if (obj.GetComponent<EnemyController>() != null && obj.GetComponent<EnemyController>().enemyParameters.isBoss && obj.GetComponent<EnemyController>().IsAlive()) return;
            }

            StartCoroutine(waitForAnimation());
        }
    }

    public IEnumerator waitForAnimation()
    {
        anim.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1);
        ChangeScene();
    }
    public void ChangeScene()
    {
        List<GameObject> popUps = new List<GameObject>();
        for (int i = 0; i < GameObject.FindGameObjectWithTag("MainCanvas").transform.childCount; i++)
        {
            GameObject popUp = GameObject.FindGameObjectWithTag("MainCanvas").transform.GetChild(i).gameObject;
            if (popUp.tag == "PopUpUI") popUps.Add(popUp);
        }
        foreach (GameObject popUp in popUps)
        {
            Destroy(popUp);
        }

        if (!DataManager.Instance.isEducating)
        {
            if (CheckpointManager.Instance.levelsPassed % 5 < 3)
            {
                SceneToChange = CheckpointManager.Instance.commonLevels[Random.Range(0, CheckpointManager.Instance.commonLevels.Count)];
            }
            else if (CheckpointManager.Instance.levelsPassed % 5 == 3)
            {
                SceneToChange = CheckpointManager.Instance.bossLevels[Random.Range(0, CheckpointManager.Instance.bossLevels.Count)];
            }
            else if (CheckpointManager.Instance.levelsPassed % 5 == 4)
            {
                SceneToChange = CheckpointManager.Instance.checkpoints[Random.Range(0, CheckpointManager.Instance.checkpoints.Count)];
            }
            CheckpointManager.Instance.ChangeLevel();
            UIManager.Instance.bossCounter.text = "0";
            UIManager.Instance.enemyCounter.text = "0";
            UIManager.Instance.levelCounter.text = "Level " + CheckpointManager.Instance.levelsPassed.ToString();
            SceneManager.LoadScene(SceneToChange);
        }
        else
        {
            DataManager.Instance.isEducating = false;
            UIManager.Instance.bossCounter.text = "0";
            UIManager.Instance.enemyCounter.text = "0";
            UIManager.Instance.levelCounter.text = "HUB";
            SceneManager.LoadScene("HUB");
        }
    }

    public void ShowButton()
    {
        if (interactIcon != null && !DataManager.Instance.isEducating)
        {
            interactIcon.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.5f);
            interactIcon.GetComponentInChildren<TMP_Text>().text = UIManager.Instance.textKeys[15].text;
            interactIcon.SetActive(true);
        }
    }

    public void HideButton()
    {
        if (interactIcon != null && !DataManager.Instance.isEducating) interactIcon.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (DataManager.Instance.isEducating && collision.tag == "Player")
        {
            StartCoroutine(waitForAnimation());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingManager : MonoBehaviour
{
    public bool movementBlocked = true;
    public bool attacksBlocked = true;
    public bool uiBlocked = true;
    public bool isTyping = false;
    public bool isRemovingTasks = false;
    public bool itemWasDraggedAndMoved = false;
    public bool itemWasClickedAndMoved = false;
    public GameObject taskPrefab;
    [SerializeField] private List<MonoBehaviour> trainables;

    public Transform taskList;

    public Transform bankerHouse, traiderHouse, blacksmithHouse, wizardHouse;

    public GameObject dialogPanel;
    public TMP_Text textFieldObject, nameText;

    public static TrainingManager Instance { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            StartCoroutine(StartTraining());
        }
    }

    private IEnumerator StartTraining()
    {
        yield return new WaitForSeconds(1.0f);
        foreach (ITrainable trainable in trainables)
        {
            StartCoroutine(trainable.StartTraining());
            yield return new WaitWhile(() => trainable.IsTraining());
            yield return new WaitForSeconds(1.0f);
        }
    }

    public IEnumerator StartTyping(string text, TMP_Text textField)
    {
        isTyping = true;
        textField.text = "";
        foreach (char symbol in text)
        {
            textField.text += symbol;
            yield return new WaitForSeconds(0.025f);
        }
        isTyping = false;
    }

    public bool HasUndoneTasks()
    {
        foreach (Toggle task in taskList.GetComponentsInChildren<Toggle>()) if (!task.isOn) return true;
        return false;
    }

    public void AddTask(string description)
    {
        TMP_Text task = Instantiate(taskPrefab, taskList).GetComponentInChildren<TMP_Text>();
        task.text = description;
    }

    public IEnumerator RemoveTasks(bool closeUI = false)
    {
        isRemovingTasks = true;
        yield return new WaitForSeconds(1.0f);
        if (closeUI)
        {
            UIManager.Instance.SwitchInventory();
        }

        for (int i = taskList.childCount-1; i >= 0; i--)
        {
            for (int j = 0; j < 100; j++)
            {
                taskList.GetChild(i).Translate(new Vector3(20.0f, 0, 0));
                yield return new WaitForEndOfFrame();
            }
            Destroy(taskList.GetChild(i).gameObject);
        }
        isRemovingTasks = false;
    }
}

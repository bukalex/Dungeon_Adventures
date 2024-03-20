using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrainingManager : MonoBehaviour
{
    public bool movementBlocked = false;
    public bool attacksBlocked = true;
    public bool uiBlocked = false;
    public bool isTyping = false;
    public GameObject taskPrefab;
    [SerializeField] private List<MonoBehaviour> trainables;

    public ToggleGroup taskList;

    public Transform bankerHouse, traiderHouse, blacksmithHouse, wizardHouse;

    public Transform dialogPanel;
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
        yield return new WaitForSeconds(2.0f);
        foreach (ITrainable trainable in trainables)
        {
            StartCoroutine(trainable.StartTraining());
            yield return new WaitWhile(() => trainable.IsTraining());
            yield return new WaitForSeconds(1.0f);
        }
    }

    public IEnumerator StartTyping(string text, TMP_Text textField)
    {
        textField.text = "";
        foreach (char symbol in text)
        {
            textField.text += symbol;
            yield return new WaitForSeconds(0.025f);
        }
        isTyping = false;
    }
}

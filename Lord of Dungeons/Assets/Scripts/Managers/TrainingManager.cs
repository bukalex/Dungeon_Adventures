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
    [SerializeField] private List<MonoBehaviour> trainables;

    public Image movementUI;
    public TMP_Text movementDescription;

    public Transform bankerHouse, traiderHouse, blacksmithHouse, wizardHouse;

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
}

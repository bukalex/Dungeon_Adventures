using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour
{
    public bool movementBlocked = false;
    [SerializeField] private List<ITrainable> trainables;

    public static TrainingManager Instance { get; set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            StartCoroutine(StartTraining());
        }
    }

    private IEnumerator StartTraining()
    {
        foreach (ITrainable trainable in trainables)
        {
            StartCoroutine(trainable.StartTraining());
            yield return new WaitWhile(() => trainable.IsTraining());
            yield return new WaitForSeconds(2.0f);
        }
    }
}
